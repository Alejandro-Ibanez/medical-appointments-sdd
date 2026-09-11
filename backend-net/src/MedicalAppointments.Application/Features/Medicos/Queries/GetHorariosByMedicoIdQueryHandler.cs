using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Medicos.Queries;

public class GetHorariosByMedicoIdQueryHandler : IRequestHandler<GetHorariosByMedicoIdQuery, List<HorarioAtencionResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetHorariosByMedicoIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HorarioAtencionResponse>> Handle(GetHorariosByMedicoIdQuery request, CancellationToken cancellationToken)
    {
        var medicoExiste = await _context.Medicos.AnyAsync(m => m.Id == request.MedicoId, cancellationToken);
        if (!medicoExiste)
        {
            throw new NotFoundException(nameof(Medico), request.MedicoId);
        }

        var horarios = await _context.HorariosAtencion
            .Include(h => h.Consultorio)
            .Where(h => h.MedicoId == request.MedicoId)
            .OrderBy(h => h.DiaSemana)
            .ThenBy(h => h.HoraInicio)
            .ToListAsync(cancellationToken);

        return horarios.Select(h => h.ToDto()).ToList();
    }
}
