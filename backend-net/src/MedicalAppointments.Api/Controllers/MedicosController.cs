#pragma warning disable CS8765 // Nullability of parameter type doesn't match the NSwag-generated abstract member (query params are optional by contract).

using System.Globalization;
using MediatR;
using MedicalAppointments.Api.Controllers.Generated.Medicos;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Medicos.Commands;
using MedicalAppointments.Application.Features.Medicos.Queries;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

public class MedicosController : MedicosControllerBase
{
    private readonly IMediator _mediator;

    public MedicosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "medicos.administrar")]
    public override async Task<MedicosPagedResponse> ListarMedicos(int? pageNumber = 1, int? pageSize = 10, string? searchQuery = null, CancellationToken cancellationToken = default)
    {
        var query = new GetMedicosQuery(pageNumber ?? 1, pageSize ?? 10, searchQuery);
        return await _mediator.Send(query, cancellationToken);
    }

    [Authorize(Policy = "medicos.administrar")]
    public override async Task<Medico> CrearMedico([FromBody] CrearMedicoRequest body, CancellationToken cancellationToken = default)
    {
        var command = new CrearMedicoCommand(
            body.NombreCompleto,
            body.DocumentoIdentidad,
            body.Especialidad,
            body.NumeroColegiado,
            body.Telefono,
            body.Email,
            body.Username,
            body.Password);

        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = "medicos.administrar")]
    public override async Task<Medico> ActualizarMedico([FromBody] MedicoInput body, long id, CancellationToken cancellationToken = default)
    {
        var command = new ActualizarMedicoCommand(
            id,
            body.NombreCompleto,
            body.DocumentoIdentidad,
            body.Especialidad,
            body.NumeroColegiado,
            body.Telefono,
            body.Email,
            body.Activo);

        return await _mediator.Send(command, cancellationToken);
    }

    public override async Task<ICollection<MedicoAutocompletado>> AutocompletarMedicos(string term, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetMedicosAutocompletarQuery(term), cancellationToken);
    }

    [Authorize]
    public override async Task<ICollection<HorarioAtencionResponse>> ObtenerHorariosMedico(long id, CancellationToken cancellationToken = default)
    {
        var esAdminORecepcionista = User.IsInRole("Admin") || User.IsInRole("Recepcionista");

        if (!esAdminORecepcionista)
        {
            var medicoIdClaim = User.FindFirst("MedicoId")?.Value;
            var esSuPropioHorario = long.TryParse(medicoIdClaim, out var medicoIdDelUsuario) && medicoIdDelUsuario == id;

            if (!esSuPropioHorario)
            {
                throw new ForbiddenException("No tienes permiso para consultar los horarios de otro medico.");
            }
        }

        return await _mediator.Send(new GetHorariosByMedicoIdQuery(id), cancellationToken);
    }

    [Authorize(Policy = "medicos.administrar")]
    public override async Task<HorarioAtencionResponse> AgregarHorarioMedico([FromBody] HorarioAtencionRequest body, long id, CancellationToken cancellationToken = default)
    {
        var command = new AsignarHorarioCommand(
            id,
            body.DiaSemana,
            TimeOnly.ParseExact(body.HoraInicio, "HH:mm", CultureInfo.InvariantCulture),
            TimeOnly.ParseExact(body.HoraFin, "HH:mm", CultureInfo.InvariantCulture),
            body.ConsultorioId);

        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = "medicos.administrar")]
    public override async Task EliminarHorarioAtencion(long id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new EliminarHorarioCommand(id), cancellationToken);
        Response.StatusCode = StatusCodes.Status204NoContent;
    }
}
