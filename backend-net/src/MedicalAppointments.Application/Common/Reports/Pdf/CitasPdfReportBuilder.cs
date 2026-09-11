using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;

namespace MedicalAppointments.Application.Common.Reports.Pdf;

public static class CitasPdfReportBuilder
{
    public static byte[] Construir(IReadOnlyList<DomainCita> citas)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Text("Reporte de Citas Medicas")
                        .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().PaddingTop(2).Text(
                        $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm} · {citas.Count} cita(s)");
                    column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                page.Content().PaddingTop(10).Table(tabla =>
                {
                    tabla.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                    });

                    tabla.Header(header =>
                    {
                        EstiloEncabezado(header.Cell(), "Fecha y Hora");
                        EstiloEncabezado(header.Cell(), "Paciente");
                        EstiloEncabezado(header.Cell(), "Medico");
                        EstiloEncabezado(header.Cell(), "Motivo de Consulta");
                        EstiloEncabezado(header.Cell(), "Estado");
                    });

                    foreach (var cita in citas)
                    {
                        EstiloCelda(tabla.Cell(), cita.FechaHora.ToString("dd/MM/yyyy HH:mm"));
                        EstiloCelda(tabla.Cell(), cita.Paciente?.NombreCompleto ?? string.Empty);
                        EstiloCelda(tabla.Cell(), cita.Medico?.NombreCompleto ?? string.Empty);
                        EstiloCelda(tabla.Cell(), cita.MotivoConsulta);
                        EstiloCelda(tabla.Cell(), cita.Estado.ToString());
                    }
                });

                page.Footer().AlignCenter().Text(texto =>
                {
                    texto.Span("Pagina ");
                    texto.CurrentPageNumber();
                    texto.Span(" de ");
                    texto.TotalPages();
                });
            });
        });

        return documento.GeneratePdf();
    }

    private static void EstiloEncabezado(IContainer contenedor, string texto)
    {
        contenedor
            .Background(Colors.Blue.Darken2)
            .Padding(5)
            .Text(texto)
            .FontColor(Colors.White)
            .Bold();
    }

    private static void EstiloCelda(IContainer contenedor, string texto)
    {
        contenedor
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .Text(texto);
    }
}
