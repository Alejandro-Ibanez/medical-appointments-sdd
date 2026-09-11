using ClosedXML.Excel;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;
using DomainMedico = MedicalAppointments.Domain.Entities.Medico;

namespace MedicalAppointments.Application.Common.Reports.Excel;

public static class AgendaMedicoExcelReportBuilder
{
    public static byte[] Construir(DomainMedico medico, IReadOnlyList<DomainCita> citas)
    {
        using var workbook = new XLWorkbook();

        var resumen = workbook.Worksheets.Add("Resumen");
        resumen.Cell(1, 1).Value = "Médico";
        resumen.Cell(1, 2).Value = medico.NombreCompleto;
        resumen.Cell(2, 1).Value = "Especialidad";
        resumen.Cell(2, 2).Value = medico.Especialidad;
        resumen.Cell(3, 1).Value = "N° Colegiado";
        resumen.Cell(3, 2).Value = medico.NumeroColegiado;
        resumen.Cell(4, 1).Value = "Total de Citas";
        resumen.Cell(4, 2).Value = citas.Count;
        resumen.Column(1).Style.Font.Bold = true;

        var fila = 6;
        resumen.Cell(fila, 1).Value = "Estado";
        resumen.Cell(fila, 2).Value = "Cantidad";
        resumen.Range(fila, 1, fila, 2).Style.Font.Bold = true;
        resumen.Range(fila, 1, fila, 2).Style.Fill.BackgroundColor = XLColor.FromHtml("#1D4ED8");
        resumen.Range(fila, 1, fila, 2).Style.Font.FontColor = XLColor.White;
        fila++;

        foreach (var grupo in citas.GroupBy(c => c.Estado).OrderBy(g => g.Key))
        {
            resumen.Cell(fila, 1).Value = grupo.Key.ToString();
            resumen.Cell(fila, 2).Value = grupo.Count();
            fila++;
        }

        resumen.Columns().AdjustToContents();

        var detalle = workbook.Worksheets.Add("Detalle de Citas");
        detalle.Cell(1, 1).Value = "Fecha y Hora";
        detalle.Cell(1, 2).Value = "Paciente";
        detalle.Cell(1, 3).Value = "Motivo de Consulta";
        detalle.Cell(1, 4).Value = "Estado";

        var headerRow = detalle.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#1D4ED8");
        headerRow.Style.Font.FontColor = XLColor.White;

        var filaDetalle = 2;
        foreach (var cita in citas.OrderBy(c => c.FechaHora))
        {
            detalle.Cell(filaDetalle, 1).Value = cita.FechaHora;
            detalle.Cell(filaDetalle, 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            detalle.Cell(filaDetalle, 2).Value = cita.Paciente?.NombreCompleto ?? string.Empty;
            detalle.Cell(filaDetalle, 3).Value = cita.MotivoConsulta;
            detalle.Cell(filaDetalle, 4).Value = cita.Estado.ToString();
            filaDetalle++;
        }

        var rangoTabla = detalle.Range(1, 1, Math.Max(filaDetalle - 1, 1), 4);
        rangoTabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        rangoTabla.Style.Border.InsideBorder = XLBorderStyleValues.Hair;

        detalle.Columns().AdjustToContents();
        detalle.SheetView.FreezeRows(1);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
