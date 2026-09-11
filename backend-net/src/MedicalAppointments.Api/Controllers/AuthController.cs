using MediatR;
using MedicalAppointments.Api.Controllers.Generated.Auth;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Usuarios.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

public class AuthController : AuthControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    public override async Task<LoginResponse> Login([FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand(body.Usuario, body.Contrasena);
        return await _mediator.Send(command, cancellationToken);
    }
}
