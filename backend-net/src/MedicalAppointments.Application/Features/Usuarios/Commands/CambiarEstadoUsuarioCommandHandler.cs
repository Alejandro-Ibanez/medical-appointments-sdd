using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainUsuario = MedicalAppointments.Domain.Entities.Usuario;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public class CambiarEstadoUsuarioCommandHandler : IRequestHandler<CambiarEstadoUsuarioCommand, UsuarioResponse>
{
    private readonly IApplicationDbContext _context;

    public CambiarEstadoUsuarioCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioResponse> Handle(CambiarEstadoUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Usuario), request.Id);

        usuario.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return usuario.ToDto();
    }
}
