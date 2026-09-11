using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DomainHistorial = MedicalAppointments.Domain.Entities.HistorialClinico;
using DomainPaciente = MedicalAppointments.Domain.Entities.Paciente;

namespace MedicalAppointments.Application.Common.Reports.Pdf;

public static class HistorialPacientePdfReportBuilder
{
    public static byte[] Construir(DomainPaciente paciente, IReadOnlyList<DomainHistorial> historial)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(encabezado =>
                        {
                            encabezado.Item().Text("Sistema de Citas Médicas")
                                .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                            encabezado.Item().Text("Historial Clínico del Paciente")
                                .FontSize(12).FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(150).AlignRight().Text(
                            $"Generado el\n{DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(8).FontColor(Colors.Grey.Darken1);
                    });

                    column.Item().PaddingTop(10).LineHorizontal(1.5f).LineColor(Colors.Blue.Darken2);
                });

                page.Content().PaddingTop(15).Column(contenido =>
                {
                    // Cuadricula de datos del paciente.
                    contenido.Item().Background(Colors.Grey.Lighten4).Padding(12).Column(datos =>
                    {
                        datos.Item().Text(paciente.NombreCompleto).FontSize(14).Bold();

                        datos.Item().PaddingTop(6).Row(fila =>
                        {
                            fila.RelativeItem().Text($"Documento: {paciente.DocumentoIdentidad}");
                            fila.RelativeItem().Text($"Género: {paciente.Genero}");
                            fila.RelativeItem().Text($"Nacimiento: {paciente.FechaNacimiento:dd/MM/yyyy}");
                        });

                        datos.Item().PaddingTop(4).Row(fila =>
                        {
                            fila.RelativeItem().Text($"Teléfono: {(string.IsNullOrWhiteSpace(paciente.Telefono) ? "-" : paciente.Telefono)}");
                            fila.RelativeItem(2).Text($"Email: {(string.IsNullOrWhiteSpace(paciente.Email) ? "-" : paciente.Email)}");
                        });

                        if (!string.IsNullOrWhiteSpace(paciente.Direccion))
                        {
                            datos.Item().PaddingTop(4).Text($"Dirección: {paciente.Direccion}");
                        }
                    });

                    // Seccion de alergias, destacada en negrita.
                    contenido.Item().PaddingTop(10).Background(Colors.Red.Lighten4)
                        .Border(1).BorderColor(Colors.Red.Medium)
                        .Padding(10)
                        .Row(fila =>
                        {
                            fila.RelativeItem().Column(alergiaColumna =>
                            {
                                alergiaColumna.Item().Text("ALERGIAS").Bold().FontColor(Colors.Red.Darken3);
                                alergiaColumna.Item().Text(
                                    string.IsNullOrWhiteSpace(paciente.Alergias)
                                        ? "Sin alergias registradas."
                                        : paciente.Alergias
                                ).Bold().FontColor(Colors.Red.Darken3);
                            });
                        });

                    // Listado cronologico de evoluciones.
                    contenido.Item().PaddingTop(18).Text("Evoluciones Clínicas").FontSize(13).Bold();
                    contenido.Item().PaddingTop(2).Text($"{historial.Count} entrada(s) registrada(s)")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);

                    if (historial.Count == 0)
                    {
                        contenido.Item().PaddingTop(10).Text("Este paciente aún no tiene entradas en su historial clínico.")
                            .Italic().FontColor(Colors.Grey.Darken1);
                    }

                    foreach (var entrada in historial)
                    {
                        contenido.Item().PaddingTop(12).Column(entradaColumna =>
                        {
                            entradaColumna.Item().Row(fila =>
                            {
                                fila.RelativeItem().Text(entrada.FechaHora.ToString("dd/MM/yyyy HH:mm"))
                                    .Bold().FontColor(Colors.Blue.Darken2);
                                fila.RelativeItem().AlignRight().Text($"Dr(a). {entrada.MedicoNombre}")
                                    .FontColor(Colors.Grey.Darken2);
                            });

                            entradaColumna.Item().PaddingTop(4).Text(texto =>
                            {
                                texto.Span("Diagnóstico: ").Bold();
                                texto.Span(entrada.Diagnostico);
                            });

                            if (!string.IsNullOrWhiteSpace(entrada.Tratamiento))
                            {
                                entradaColumna.Item().PaddingTop(2).Text(texto =>
                                {
                                    texto.Span("Tratamiento: ").Bold();
                                    texto.Span(entrada.Tratamiento);
                                });
                            }

                            if (!string.IsNullOrWhiteSpace(entrada.Notas))
                            {
                                entradaColumna.Item().PaddingTop(2).Text(texto =>
                                {
                                    texto.Span("Notas: ").Bold();
                                    texto.Span(entrada.Notas);
                                });
                            }
                        });

                        contenido.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    }

                    // Espacio de firma del medico al final del documento.
                    contenido.Item().PaddingTop(40).Row(fila =>
                    {
                        fila.RelativeItem();
                        fila.RelativeItem().Column(firmaColumna =>
                        {
                            firmaColumna.Item().PaddingBottom(2).LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                            firmaColumna.Item().AlignCenter().Text("Firma del Médico Responsable")
                                .FontSize(9).FontColor(Colors.Grey.Darken2);
                        });
                    });
                });

                page.Footer().AlignCenter().Text(texto =>
                {
                    texto.Span("Documento generado automáticamente · Página ");
                    texto.CurrentPageNumber();
                    texto.Span(" de ");
                    texto.TotalPages();
                });
            });
        });

        return documento.GeneratePdf();
    }
}
