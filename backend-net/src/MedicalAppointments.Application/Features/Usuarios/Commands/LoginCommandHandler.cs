using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Username == request.Usuario, cancellationToken);

        if (usuario is null
            || usuario.Rol is null
            || !usuario.Activo
            || !_passwordHasher.VerifyPassword(request.Contrasena, usuario.PasswordHash))
        {
            throw new UnauthorizedException("Usuario o contrasena incorrectos.");
        }

        var permisos = await _context.RolPermisos
            .Where(rp => rp.RolId == usuario.RolId)
            .Select(rp => rp.Permiso!.Codigo)
            .ToListAsync(cancellationToken);

        var token = _jwtTokenGenerator.GenerarToken(usuario, permisos);

        return new LoginResponse
        {
            Token = token,
            Username = usuario.Username,
            NombreCompleto = usuario.NombreCompleto,
            Rol = Enum.Parse<RolUsuario>(usuario.Rol.Nombre),
            MedicoId = usuario.MedicoId
        };
    }
}
