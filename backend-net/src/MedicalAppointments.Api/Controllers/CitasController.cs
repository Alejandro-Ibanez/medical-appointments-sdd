#pragma warning disable CS8765 // Nullability of parameter type doesn't match the NSwag-generated abstract member (query/body params are optional by contract).

using MediatR;
using MedicalAppointments.Api.Controllers.Generated;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Citas.Commands;
using MedicalAppointments.Application.Features.Citas.Queries;
using MedicalAppointments.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

public class CitasController : CitasControllerBase
{
    private readonly IMediator _mediator;

    public CitasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "citas.escribir")]
    public override async Task<CitaDetalleResponse> CrearCita([FromBody] CrearCitaRequest body, CancellationToken cancellationToken = default)
    {
        var command = new CrearCitaCommand(
            body.PacienteId,
            body.MedicoId,
            body.FechaHora.DateTime,
            body.MotivoConsulta);

        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = "citas.escribir")]
    public override async Task<CitaDetalleResponse> CancelarCita(long id, [FromBody] CancelarCitaRequest body, CancellationToken cancellationToken = default)
    {
        var command = new CancelarCitaCommand(id, body.MotivoCancelacion);
        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = "citas.leer")]
    public override async Task<CitasPagedResponse> BuscarCitas(
        string? palabraClave = null,
        long? medicoId = null,
        long? pacienteId = null,
        DateTimeOffset? fechaInicio = null,
        DateTimeOffset? fechaFin = null,
        int? pageNumber = 1,
        int? pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new BuscarCitasQuery(
            palabraClave,
            medicoId,
            pacienteId,
            fechaInicio?.Date,
            fechaFin?.Date,
            pageNumber ?? 1,
            pageSize ?? 10);

        return await _mediator.Send(query, cancellationToken);
    }

    [Authorize(Policy = "reportes.descargar")]
    public override async Task<FileResult> GenerarReporteExcelCitas(
        string? palabraClave = null,
        long? medicoId = null,
        long? pacienteId = null,
        DateTimeOffset? fechaInicio = null,
        DateTimeOffset? fechaFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GenerarReporteExcelQuery(
            palabraClave,
            medicoId,
            pacienteId,
            fechaInicio?.Date,
            fechaFin?.Date);

        var contenido = await _mediator.Send(query, cancellationToken);

        const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        const string nombreArchivo = "reporte_citas.xlsx";

        return File(contenido, contentType, nombreArchivo);
    }

    [Authorize(Policy = "reportes.descargar")]
    public override async Task<FileResult> GenerarReportePdfCitas(
        string? palabraClave = null,
        long? medicoId = null,
        long? pacienteId = null,
        DateTimeOffset? fechaInicio = null,
        DateTimeOffset? fechaFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GenerarReportePdfQuery(
            palabraClave,
            medicoId,
            pacienteId,
            fechaInicio?.Date,
            fechaFin?.Date);

        var contenido = await _mediator.Send(query, cancellationToken);

        const string contentType = "application/pdf";
        const string nombreArchivo = "reporte_citas.pdf";

        return File(contenido, contentType, nombreArchivo);
    }

    [Authorize(Policy = "dashboard.ver")]
    public override async Task<DashboardStats> ObtenerEstadisticasDashboard(CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
    }

    [Authorize(Policy = "dashboard.ver")]
    public override async Task<DashboardMetricas> ObtenerMetricasDashboard(CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetDashboardMetricasQuery(), cancellationToken);
    }

    [Authorize(Policy = "citas.leer")]
    public override async Task<ICollection<CitaCalendarioResponse>> ObtenerCitasCalendario(
        DateTimeOffset? start = null,
        DateTimeOffset? end = null,
        CancellationToken cancellationToken = default)
    {
        long? medicoIdDelUsuario = null;
        if (User.IsInRole("Medico"))
        {
            var medicoIdClaim = User.FindFirst("MedicoId")?.Value;
            if (long.TryParse(medicoIdClaim, out var medicoId))
            {
                medicoIdDelUsuario = medicoId;
            }
        }

        var query = new GetCitasCalendarioQuery(start?.DateTime, end?.DateTime, medicoIdDelUsuario);
        return await _mediator.Send(query, cancellationToken);
    }
}
