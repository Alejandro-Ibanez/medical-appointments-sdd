using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;
using DomainMedico = MedicalAppointments.Domain.Entities.Medico;
using EstadoCita = MedicalAppointments.Domain.Enums.EstadoCita;

namespace MedicalAppointments.Application.Common.Reports.Pdf;

public static class AgendaMedicoPdfReportBuilder
{
    public static byte[] Construir(
        DomainMedico medico,
        IReadOnlyList<DomainCita> citas,
        DateTime? fechaInicio,
        DateTime? fechaFin)
    {
        var resumenPorEstado = citas
            .GroupBy(c => c.Estado)
            .OrderBy(g => g.Key)
            .Select(g => (Estado: g.Key, Cantidad: g.Count()))
            .ToList();

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
                            encabezado.Item().Text("Reporte de Agenda y Rendimiento del Médico")
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
                    contenido.Item().Background(Colors.Grey.Lighten4).Padding(12).Column(datos =>
                    {
                        datos.Item().Text($"Dr(a). {medico.NombreCompleto}").FontSize(14).Bold();
                        datos.Item().PaddingTop(4).Row(fila =>
                        {
                            fila.RelativeItem().Text($"Especialidad: {medico.Especialidad}");
                            fila.RelativeItem().Text($"N° Colegiado: {medico.NumeroColegiado}");
                        });
                        datos.Item().PaddingTop(4).Text(
                            $"Periodo: {(fechaInicio?.ToString("dd/MM/yyyy") ?? "Inicio")} — {(fechaFin?.ToString("dd/MM/yyyy") ?? "Actualidad")}");
                    });

                    contenido.Item().PaddingTop(16).Text("Resumen por Estado").FontSize(13).Bold();
                    contenido.Item().PaddingTop(6).Row(fila =>
                    {
                        fila.RelativeItem().Text($"Total de citas: {citas.Count}").Bold();
                    });

                    contenido.Item().PaddingTop(4).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columnas =>
                        {
                            columnas.RelativeColumn(2);
                            columnas.RelativeColumn(1);
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Estado").FontColor(Colors.White).Bold();
                            header.Cell().Background(Colors.Blue.Darken2).Padding(5)
                                .Text("Cantidad").FontColor(Colors.White).Bold();
                        });

                        foreach (var (estado, cantidad) in resumenPorEstado)
                        {
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(estado.ToString());
                            tabla.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(cantidad.ToString());
                        }
                    });

                    contenido.Item().PaddingTop(20).Text("Listado Cronológico de Citas").FontSize(13).Bold();

                    if (citas.Count == 0)
                    {
                        contenido.Item().PaddingTop(10).Text("No se encontraron citas en el periodo seleccionado.")
                            .Italic().FontColor(Colors.Grey.Darken1);
                    }

                    foreach (var cita in citas.OrderBy(c => c.FechaHora))
                    {
                        contenido.Item().PaddingTop(10).Column(citaColumna =>
                        {
                            citaColumna.Item().Row(fila =>
                            {
                                fila.RelativeItem().Text(cita.FechaHora.ToString("dd/MM/yyyy HH:mm"))
                                    .Bold().FontColor(Colors.Blue.Darken2);
                                fila.RelativeItem().AlignRight().Text(cita.Estado.ToString())
                                    .FontColor(cita.Estado == EstadoCita.Cancelada ? Colors.Red.Darken1 : Colors.Grey.Darken2);
                            });
                            citaColumna.Item().PaddingTop(2).Text($"Paciente: {cita.Paciente?.NombreCompleto ?? "-"}");
                            citaColumna.Item().PaddingTop(2).Text($"Motivo: {cita.MotivoConsulta}");
                        });

                        contenido.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    }
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
