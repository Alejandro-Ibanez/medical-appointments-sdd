using MediatR;
using MedicalAppointments.Api.Controllers.Generated.Reportes;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Reportes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[Authorize(Policy = "reportes.descargar")]
public class ReportesController : ReportesControllerBase
{
    private readonly IMediator _mediator;

    public ReportesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<FileResult> GenerarReporteHistorialPdf(long id, CancellationToken cancellationToken = default)
    {
        var query = new GetHistorialPacientePdfQuery(id);
        var resultado = await _mediator.Send(query, cancellationToken);
        return File(resultado.Contenido, resultado.ContentType, resultado.NombreArchivo);
    }

    public override async Task<FileResult> GenerarReporteAgendaMedico(
        long id,
        Formato formato,
        DateTimeOffset? fechaInicio = null,
        DateTimeOffset? fechaFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAgendaMedicoReporteQuery(id, formato, fechaInicio?.Date, fechaFin?.Date);
        var resultado = await _mediator.Send(query, cancellationToken);
        return File(resultado.Contenido, resultado.ContentType, resultado.NombreArchivo);
    }

    public override async Task<FileResult> GenerarReporteOcupacionConsultorios(
        DateTimeOffset? fechaInicio = null,
        DateTimeOffset? fechaFin = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOcupacionConsultoriosQuery(fechaInicio?.Date, fechaFin?.Date);
        var resultado = await _mediator.Send(query, cancellationToken);
        return File(resultado.Contenido, resultado.ContentType, resultado.NombreArchivo);
    }
}
