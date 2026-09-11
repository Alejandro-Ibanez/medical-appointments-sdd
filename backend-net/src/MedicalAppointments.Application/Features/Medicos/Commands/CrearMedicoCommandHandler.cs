using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainMedico = MedicalAppointments.Domain.Entities.Medico;
using DomainUsuario = MedicalAppointments.Domain.Entities.Usuario;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public class CrearMedicoCommandHandler : IRequestHandler<CrearMedicoCommand, Medico>
{
    private const string NombreRolMedico = "Medico";

    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CrearMedicoCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Medico> Handle(CrearMedicoCommand request, CancellationToken cancellationToken)
    {
        var usernameEnUso = await _context.Usuarios
            .AnyAsync(u => u.Username == request.Username, cancellationToken);
        if (usernameEnUso)
        {
            throw new BusinessRuleException($"El nombre de usuario '{request.Username}' ya esta en uso.");
        }

        var emailEnUso = await _context.Usuarios
            .AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (emailEnUso)
        {
            throw new BusinessRuleException($"El correo '{request.Email}' ya esta en uso por otra cuenta de usuario.");
        }

        var rolMedico = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == NombreRolMedico, cancellationToken)
            ?? throw new BusinessRuleException("No se encontro el rol 'Medico' configurado en el sistema.");

        await using var transaccion = await _context.Database.BeginTransactionAsync(cancellationToken);

        var medico = new DomainMedico
        {
            NombreCompleto = request.NombreCompleto,
            DocumentoIdentidad = request.DocumentoIdentidad,
            Especialidad = request.Especialidad,
            NumeroColegiado = request.NumeroColegiado,
            Telefono = request.Telefono,
            Email = request.Email,
            Activo = true
        };

        _context.Medicos.Add(medico);
        await _context.SaveChangesAsync(cancellationToken);

        var usuario = new DomainUsuario
        {
            Username = request.Username,
            NombreCompleto = request.NombreCompleto,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            RolId = rolMedico.Id,
            MedicoId = medico.Id,
            Activo = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        await transaccion.CommitAsync(cancellationToken);

        return medico.ToDto();
    }
}
