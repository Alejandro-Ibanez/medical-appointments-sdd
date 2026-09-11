using MediatR;
using MedicalAppointments.Api.Controllers.Generated.Seguridad;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Seguridad.Commands;
using MedicalAppointments.Application.Features.Seguridad.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[Authorize(Policy = "usuarios.administrar")]
public class SeguridadController : SeguridadControllerBase
{
    private readonly IMediator _mediator;

    public SeguridadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ICollection<PermisoResponse>> ListarPermisos(CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPermisosQuery(), cancellationToken);
    }

    public override async Task<ICollection<RolResponse>> ListarRoles(CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetRolesQuery(), cancellationToken);
    }

    public override async Task<ICollection<PermisoResponse>> ObtenerPermisosDeRol(RolUsuario rol, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetPermisosDeRolQuery(rol.ToString()), cancellationToken);
    }

    public override async Task<ICollection<PermisoResponse>> ActualizarPermisosDeRol(
        [FromBody] ActualizarPermisosRolRequest body,
        RolUsuario rol,
        CancellationToken cancellationToken = default)
    {
        var command = new ActualizarPermisosDeRolCommand(rol.ToString(), body.PermisosIds.ToList());
        return await _mediator.Send(command, cancellationToken);
    }
}
