using MediatR;
using MedicalAppointments.Api.Controllers.Generated.Consultorios;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Consultorios.Commands;
using MedicalAppointments.Application.Features.Consultorios.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[Authorize(Policy = "consultorios.administrar")]
public class ConsultoriosController : ConsultoriosControllerBase
{
    private readonly IMediator _mediator;

    public ConsultoriosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ICollection<ConsultorioResponse>> ListarConsultorios(CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetConsultoriosQuery(), cancellationToken);
    }

    public override async Task<ConsultorioResponse> CrearConsultorio([FromBody] ConsultorioRequest body, CancellationToken cancellationToken = default)
    {
        var command = new CrearConsultorioCommand(body.Nombre, body.Ubicacion);
        return await _mediator.Send(command, cancellationToken);
    }

    public override async Task<ConsultorioResponse> ActualizarConsultorio([FromBody] ConsultorioRequest body, long id, CancellationToken cancellationToken = default)
    {
        var command = new ActualizarConsultorioCommand(id, body.Nombre, body.Ubicacion);
        return await _mediator.Send(command, cancellationToken);
    }

    public override async Task EliminarConsultorio(long id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new EliminarConsultorioCommand(id), cancellationToken);
        Response.StatusCode = StatusCodes.Status204NoContent;
    }
}
