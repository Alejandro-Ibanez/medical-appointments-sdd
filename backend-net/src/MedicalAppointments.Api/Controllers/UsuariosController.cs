#pragma warning disable CS8765 // Nullability of parameter type doesn't match the NSwag-generated abstract member (query params are optional by contract).

using System.IdentityModel.Tokens.Jwt;
using MediatR;
using MedicalAppointments.Api.Controllers.Generated.Usuarios;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Usuarios.Commands;
using MedicalAppointments.Application.Features.Usuarios.Queries;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

public class UsuariosController : UsuariosControllerBase
{
    private readonly IMediator _mediator;

    public UsuariosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = "usuarios.administrar")]
    public override async Task<UsuariosPagedResponse> ListarUsuarios(int? pageNumber = 1, int? pageSize = 10, string? searchQuery = null, CancellationToken cancellationToken = default)
    {
        var query = new GetUsuariosQuery(pageNumber ?? 1, pageSize ?? 10, searchQuery);
        return await _mediator.Send(query, cancellationToken);
    }

    [Authorize(Policy = "usuarios.administrar")]
    public override async Task<UsuarioResponse> CrearUsuario([FromBody] CrearUsuarioRequest body, CancellationToken cancellationToken = default)
    {
        var command = new CrearUsuarioCommand(
            body.NombreCompleto,
            body.Username,
            body.Password,
            body.RolId,
            body.MedicoId);

        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize(Policy = "usuarios.administrar")]
    public override async Task<UsuarioResponse> ActualizarEstadoUsuario(long id, [FromBody] ActualizarEstadoUsuarioRequest body, CancellationToken cancellationToken = default)
    {
        var command = new CambiarEstadoUsuarioCommand(id, body.Activo);
        return await _mediator.Send(command, cancellationToken);
    }

    [Authorize]
    public override async Task CambiarPasswordUsuario([FromBody] CambiarPasswordRequest body, CancellationToken cancellationToken = default)
    {
        var usuarioIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!long.TryParse(usuarioIdClaim, out var usuarioAutenticadoId))
        {
            throw new UnauthorizedException("El token no contiene un identificador de usuario valido.");
        }

        var command = new CambiarPasswordCommand(usuarioAutenticadoId, body.PasswordActual, body.PasswordNueva);
        await _mediator.Send(command, cancellationToken);

        Response.StatusCode = StatusCodes.Status204NoContent;
    }
}
