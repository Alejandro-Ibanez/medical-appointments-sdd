#pragma warning disable CS8765 // Nullability of parameter type doesn't match the NSwag-generated abstract member (query params are optional by contract).

using System.IdentityModel.Tokens.Jwt;
using MediatR;
using MedicalAppointments.Api.Controllers.Generated.Pacientes;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.HistorialClinico.Commands;
using MedicalAppointments.Application.Features.HistorialClinico.Queries;
using MedicalAppointments.Application.Features.Pacientes.Commands;
using MedicalAppointments.Application.Features.Pacientes.Queries;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

public class PacientesController : PacientesControllerBase
{
    private readonly IMediator _mediator;

    public PacientesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "pacientes.leer")]
    public override async Task<PagedResponse> ListarPacientes(int? pageNumber = 1, int? pageSize = 10, string? searchQuery = null, CancellationToken cancellationToken = default)
    {
        var query = new GetPacientesQuery(pageNumber ?? 1, pageSize ?? 10, searchQuery);
        return await _mediator.Send(query, cancellationToken);
    }

    [Authorize(Policy = "pacientes.leer")]
    public override async Task<Paciente> ObtenerPacientePorId(long id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPacienteByIdQuery(id), cancellationToken);
    }

    [Authorize(Policy = "pacientes.escribir")]
    public override async Task<Paciente> CrearPaciente([FromBody] PacienteInput body, CancellationToken cancellationToken = default)
    {
        var command = new CrearPacienteCommand(
            body.NombreCompleto,
            body.DocumentoIdentidad,
            body.FechaNacimiento,
            body.Telefono,
            body.Email,
            body.Direccion,
            body.Activo,
            body.Genero,
            body.Alergias);

        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = "pacientes.escribir")]
    public override async Task<Paciente> ActualizarPaciente([FromBody] PacienteInput body, long id, CancellationToken cancellationToken = default)
    {
        var command = new ActualizarPacienteCommand(
            id,
            body.NombreCompleto,
            body.DocumentoIdentidad,
            body.FechaNacimiento,
            body.Telefono,
            body.Email,
            body.Direccion,
            body.Activo,
            body.Genero,
            body.Alergias);

        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = "pacientes.leer")]
    public override async Task<ICollection<PacienteAutocompletado>> AutocompletarPacientes(string term, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPacientesAutocompletarQuery(term), cancellationToken);
    }

    [Authorize(Policy = "historial.leer")]
    public override async Task<ICollection<HistorialClinicoResponse>> ObtenerHistorialClinico(long id, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetHistorialByPacienteIdQuery(id), cancellationToken);
    }

    [Authorize(Policy = "historial.escribir")]
    public override async Task<HistorialClinicoResponse> AgregarEntradaHistorialClinico(
        [FromBody] HistorialClinicoInput body,
        long id,
        CancellationToken cancellationToken = default)
    {
        var usuarioIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(usuarioIdClaim, out var usuarioAutenticadoId))
        {
            throw new UnauthorizedException("El token no contiene un identificador de usuario valido.");
        }

        var command = new CrearEntradaHistorialCommand(
            id,
            usuarioAutenticadoId,
            body.Diagnostico,
            body.Tratamiento,
            body.Notas,
            body.CitaId);

        return await _mediator.Send(command, cancellationToken);
    }
}
