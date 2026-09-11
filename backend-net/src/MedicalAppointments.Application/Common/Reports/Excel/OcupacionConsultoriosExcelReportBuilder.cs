using ClosedXML.Excel;
using DomainConsultorio = MedicalAppointments.Domain.Entities.Consultorio;

namespace MedicalAppointments.Application.Common.Reports.Excel;

public record OcupacionConsultorioItem(DomainConsultorio Consultorio, int CantidadCitas, double HorasOcupadas);

public static class OcupacionConsultoriosExcelReportBuilder
{
    public static byte[] Construir(
        IReadOnlyList<OcupacionConsultorioItem> ocupacion,
        DateTime? fechaInicio,
        DateTime? fechaFin)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ocupación de Consultorios");

        worksheet.Cell(1, 1).Value = "Reporte de Ocupación de Consultorios Físicos";
        worksheet.Range(1, 1, 1, 5).Merge();
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        worksheet.Cell(2, 1).Value =
            $"Periodo: {(fechaInicio?.ToString("dd/MM/yyyy") ?? "Inicio")} — {(fechaFin?.ToString("dd/MM/yyyy") ?? "Actualidad")}";
        worksheet.Range(2, 1, 2, 5).Merge();
        worksheet.Cell(2, 1).Style.Font.Italic = true;
        worksheet.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml("#6B7280");

        const int filaEncabezado = 4;
        worksheet.Cell(filaEncabezado, 1).Value = "Consultorio";
        worksheet.Cell(filaEncabezado, 2).Value = "Ubicación";
        worksheet.Cell(filaEncabezado, 3).Value = "Cantidad de Citas";
        worksheet.Cell(filaEncabezado, 4).Value = "Horas Ocupadas";
        worksheet.Cell(filaEncabezado, 5).Value = "% de Ocupación";

        var headerRow = worksheet.Row(filaEncabezado);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1D4ED8");
        headerRow.Style.Font.FontColor = XLColor.White;

        var totalHoras = ocupacion.Sum(o => o.HorasOcupadas);

        var fila = filaEncabezado + 1;
        foreach (var item in ocupacion.OrderByDescending(o => o.HorasOcupadas))
        {
            var porcentaje = totalHoras > 0 ? item.HorasOcupadas / totalHoras : 0;

            worksheet.Cell(fila, 1).Value = item.Consultorio.Nombre;
            worksheet.Cell(fila, 2).Value = item.Consultorio.Ubicacion;
            worksheet.Cell(fila, 3).Value = item.CantidadCitas;
            worksheet.Cell(fila, 4).Value = item.HorasOcupadas;
            worksheet.Cell(fila, 4).Style.NumberFormat.Format = "0.0";
            worksheet.Cell(fila, 5).Value = porcentaje;
            worksheet.Cell(fila, 5).Style.NumberFormat.Format = "0.0%";

            // Barra visual ejecutiva construida con bloques Unicode, proporcional al porcentaje.
            var bloques = (int)Math.Round(porcentaje * 20);
            worksheet.Cell(fila, 6).Value = new string('█', Math.Max(bloques, 0));
            worksheet.Cell(fila, 6).Style.Font.FontColor = XLColor.FromHtml("#1D4ED8");

            fila++;
        }

        var filaTotal = fila;
        worksheet.Cell(filaTotal, 1).Value = "TOTAL";
        worksheet.Cell(filaTotal, 3).Value = ocupacion.Sum(o => o.CantidadCitas);
        worksheet.Cell(filaTotal, 4).Value = totalHoras;
        worksheet.Cell(filaTotal, 4).Style.NumberFormat.Format = "0.0";
        worksheet.Range(filaTotal, 1, filaTotal, 4).Style.Font.Bold = true;
        worksheet.Range(filaTotal, 1, filaTotal, 4).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        var rangoTabla = worksheet.Range(filaEncabezado, 1, filaTotal, 5);
        rangoTabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        rangoTabla.Style.Border.InsideBorder = XLBorderStyleValues.Hair;

        worksheet.Columns().AdjustToContents();
        worksheet.SheetView.FreezeRows(filaEncabezado);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
