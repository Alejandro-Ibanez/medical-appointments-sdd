using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public class CambiarPasswordCommandHandler : IRequestHandler<CambiarPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CambiarPasswordCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(CambiarPasswordCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Usuario), request.UsuarioId);

        if (!_passwordHasher.VerifyPassword(request.PasswordActual, usuario.PasswordHash))
        {
            throw new BusinessRuleException("La contrasena actual no es correcta.");
        }

        usuario.PasswordHash = _passwordHasher.HashPassword(request.PasswordNueva);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
