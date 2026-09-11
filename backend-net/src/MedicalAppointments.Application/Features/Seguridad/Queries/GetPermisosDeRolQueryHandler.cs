using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainRol = MedicalAppointments.Domain.Entities.Rol;

namespace MedicalAppointments.Application.Features.Seguridad.Queries;

public class GetPermisosDeRolQueryHandler : IRequestHandler<GetPermisosDeRolQuery, List<PermisoResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetPermisosDeRolQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermisoResponse>> Handle(GetPermisosDeRolQuery request, CancellationToken cancellationToken)
    {
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == request.Rol, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainRol), request.Rol);

        var permisos = await _context.RolPermisos
            .Where(rp => rp.RolId == rol.Id)
            .Select(rp => rp.Permiso!)
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        return permisos.Select(p => p.ToDto()).ToList();
    }
}
