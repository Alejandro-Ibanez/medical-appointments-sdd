using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainUsuario = MedicalAppointments.Domain.Entities.Usuario;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public class CrearUsuarioCommandHandler : IRequestHandler<CrearUsuarioCommand, UsuarioResponse>
{
    private const string NombreRolMedico = "Medico";

    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CrearUsuarioCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioResponse> Handle(CrearUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usernameEnUso = await _context.Usuarios
            .AnyAsync(u => u.Username == request.Username, cancellationToken);
        if (usernameEnUso)
        {
            throw new BusinessRuleException($"El nombre de usuario '{request.Username}' ya esta en uso.");
        }

        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RolId, cancellationToken)
            ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Rol), request.RolId);

        long? medicoId = null;

        if (rol.Nombre == NombreRolMedico)
        {
            if (request.MedicoId is null)
            {
                throw new BusinessRuleException(
                    "Debes indicar el medico a vincular cuando el rol asignado es Medico.");
            }

            var medicoExiste = await _context.Medicos.AnyAsync(m => m.Id == request.MedicoId, cancellationToken);
            if (!medicoExiste)
            {
                throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Medico), request.MedicoId);
            }

            medicoId = request.MedicoId;
        }

        var usuario = new DomainUsuario
        {
            NombreCompleto = request.NombreCompleto,
            Username = request.Username,
            // El contrato de creacion manual de usuarios no solicita un email
            // explicito; se deriva del username (ya validado como unico) para
            // satisfacer la restriccion de unicidad de Email en la tabla.
            Email = $"{request.Username}@clinica.com",
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            RolId = rol.Id,
            MedicoId = medicoId,
            Activo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        usuario.Rol = rol;

        return usuario.ToDto();
    }
}
