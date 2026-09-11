using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Seguridad.Queries;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RolResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetRolesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RolResponse>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _context.Roles
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);

        return roles.Select(r => r.ToDto()).ToList();
    }
}
