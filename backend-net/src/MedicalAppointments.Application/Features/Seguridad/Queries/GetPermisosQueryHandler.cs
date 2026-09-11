using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Seguridad.Queries;

public class GetPermisosQueryHandler : IRequestHandler<GetPermisosQuery, List<PermisoResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetPermisosQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermisoResponse>> Handle(GetPermisosQuery request, CancellationToken cancellationToken)
    {
        var permisos = await _context.Permisos
            .OrderBy(p => p.Id)
            .ToListAsync(cancellationToken);

        return permisos.Select(p => p.ToDto()).ToList();
    }
}
