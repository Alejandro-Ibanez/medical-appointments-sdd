using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Medicos.Queries;

public class GetMedicosAutocompletarQueryHandler
    : IRequestHandler<GetMedicosAutocompletarQuery, List<MedicoAutocompletado>>
{
    private const int MaximoResultados = 10;

    private readonly IApplicationDbContext _context;

    public GetMedicosAutocompletarQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MedicoAutocompletado>> Handle(
        GetMedicosAutocompletarQuery request,
        CancellationToken cancellationToken)
    {
        var texto = request.Term.Trim().ToLower();

        return await _context.Medicos
            .Where(m => m.NombreCompleto.ToLower().Contains(texto))
            .OrderBy(m => m.NombreCompleto)
            .Take(MaximoResultados)
            .Select(m => new MedicoAutocompletado
            {
                Id = m.Id,
                NombreCompleto = m.NombreCompleto,
                Especialidad = m.Especialidad
            })
            .ToListAsync(cancellationToken);
    }
}
