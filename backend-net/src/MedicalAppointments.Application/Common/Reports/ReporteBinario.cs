namespace MedicalAppointments.Application.Common.Reports;

/// <summary>
/// Resultado de un handler que genera un reporte binario (PDF o Excel),
/// listo para que el controlador lo retorne via File(contenido, contentType, nombreArchivo).
/// </summary>
public record ReporteBinario(byte[] Contenido, string ContentType, string NombreArchivo);
