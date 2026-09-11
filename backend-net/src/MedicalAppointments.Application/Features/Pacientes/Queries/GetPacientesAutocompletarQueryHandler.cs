using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Pacientes.Queries;

public class GetPacientesAutocompletarQueryHandler
    : IRequestHandler<GetPacientesAutocompletarQuery, List<PacienteAutocompletado>>
{
    private const int MaximoResultados = 10;

    private readonly IApplicationDbContext _context;

    public GetPacientesAutocompletarQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PacienteAutocompletado>> Handle(
        GetPacientesAutocompletarQuery request,
        CancellationToken cancellationToken)
    {
        var texto = request.Term.Trim().ToLower();

        return await _context.Pacientes
            .Where(p => p.NombreCompleto.ToLower().Contains(texto))
            .OrderBy(p => p.NombreCompleto)
            .Take(MaximoResultados)
            .Select(p => new PacienteAutocompletado
            {
                Id = p.Id,
                NombreCompleto = p.NombreCompleto
            })
            .ToListAsync(cancellationToken);
    }
}
