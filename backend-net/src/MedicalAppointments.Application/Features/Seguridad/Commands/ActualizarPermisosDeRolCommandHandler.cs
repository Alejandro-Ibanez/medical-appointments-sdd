using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainRol = MedicalAppointments.Domain.Entities.Rol;

namespace MedicalAppointments.Application.Features.Seguridad.Commands;

public class ActualizarPermisosDeRolCommandHandler : IRequestHandler<ActualizarPermisosDeRolCommand, List<PermisoResponse>>
{
    private readonly IApplicationDbContext _context;

    public ActualizarPermisosDeRolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermisoResponse>> Handle(ActualizarPermisosDeRolCommand request, CancellationToken cancellationToken)
    {
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == request.Rol, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainRol), request.Rol);

        var idsUnicos = request.PermisosIds.Distinct().ToList();

        var permisosValidos = await _context.Permisos
            .Where(p => idsUnicos.Contains(p.Id))
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        if (permisosValidos.Count != idsUnicos.Count)
        {
            throw new BusinessRuleException("Uno o mas de los permisos indicados no existe.");
        }

        var asignacionesActuales = await _context.RolPermisos
            .Where(rp => rp.RolId == rol.Id)
            .ToListAsync(cancellationToken);

        _context.RolPermisos.RemoveRange(asignacionesActuales);

        foreach (var permisoId in idsUnicos)
        {
            _context.RolPermisos.Add(new RolPermiso { RolId = rol.Id, PermisoId = permisoId });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return permisosValidos.Select(p => p.ToDto()).ToList();
    }
}
