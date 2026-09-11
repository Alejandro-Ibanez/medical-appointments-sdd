using ClosedXML.Excel;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;

namespace MedicalAppointments.Application.Common.Reports.Excel;

public static class CitasExcelReportBuilder
{
    public static byte[] Construir(IReadOnlyList<DomainCita> citas)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Citas");

        worksheet.Cell(1, 1).Value = "Fecha y Hora";
        worksheet.Cell(1, 2).Value = "Paciente";
        worksheet.Cell(1, 3).Value = "Medico";
        worksheet.Cell(1, 4).Value = "Motivo de Consulta";
        worksheet.Cell(1, 5).Value = "Estado";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1D4ED8");
        headerRow.Style.Font.FontColor = XLColor.White;

        var fila = 2;
        foreach (var cita in citas)
        {
            worksheet.Cell(fila, 1).Value = cita.FechaHora;
            worksheet.Cell(fila, 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            worksheet.Cell(fila, 2).Value = cita.Paciente?.NombreCompleto ?? string.Empty;
            worksheet.Cell(fila, 3).Value = cita.Medico?.NombreCompleto ?? string.Empty;
            worksheet.Cell(fila, 4).Value = cita.MotivoConsulta;
            worksheet.Cell(fila, 5).Value = cita.Estado.ToString();
            fila++;
        }

        var rangoTabla = worksheet.Range(1, 1, Math.Max(fila - 1, 1), 5);
        rangoTabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        rangoTabla.Style.Border.InsideBorder = XLBorderStyleValues.Hair;

        worksheet.Columns().AdjustToContents();
        worksheet.SheetView.FreezeRows(1);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
