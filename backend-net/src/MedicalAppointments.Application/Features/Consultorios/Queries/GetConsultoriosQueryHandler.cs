using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Consultorios.Queries;

public class GetConsultoriosQueryHandler : IRequestHandler<GetConsultoriosQuery, List<ConsultorioResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetConsultoriosQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConsultorioResponse>> Handle(GetConsultoriosQuery request, CancellationToken cancellationToken)
    {
        var consultorios = await _context.Consultorios
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);

        return consultorios.Select(c => c.ToDto()).ToList();
    }
}
