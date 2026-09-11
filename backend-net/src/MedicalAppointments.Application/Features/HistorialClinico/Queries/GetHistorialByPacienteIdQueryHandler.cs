using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.HistorialClinico.Queries;

public class GetHistorialByPacienteIdQueryHandler : IRequestHandler<GetHistorialByPacienteIdQuery, List<HistorialClinicoResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetHistorialByPacienteIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HistorialClinicoResponse>> Handle(GetHistorialByPacienteIdQuery request, CancellationToken cancellationToken)
    {
        var pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == request.PacienteId, cancellationToken);
        if (!pacienteExiste)
        {
            throw new NotFoundException(nameof(Paciente), request.PacienteId);
        }

        var entradas = await _context.HistorialesClinicos
            .Where(h => h.PacienteId == request.PacienteId)
            .OrderByDescending(h => h.FechaHora)
            .ToListAsync(cancellationToken);

        return entradas.Select(e => e.ToDto()).ToList();
    }
}
