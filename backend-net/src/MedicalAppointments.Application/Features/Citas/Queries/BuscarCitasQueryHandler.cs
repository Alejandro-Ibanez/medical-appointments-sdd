using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;

namespace MedicalAppointments.Application.Features.Citas.Queries;

public class BuscarCitasQueryHandler : IRequestHandler<BuscarCitasQuery, CitasPagedResponse>
{
    private readonly IApplicationDbContext _context;

    public BuscarCitasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CitasPagedResponse> Handle(BuscarCitasQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        IQueryable<DomainCita> query = _context.Citas
            .Include(c => c.Paciente)
            .Include(c => c.Medico);

        query = CitaQueryFilters.Aplicar(
            query,
            request.PalabraClave,
            request.MedicoId,
            request.PacienteId,
            request.FechaInicio,
            request.FechaFin);

        query = query.OrderByDescending(c => c.FechaHora);

        var totalCount = await query.CountAsync(cancellationToken);

        var citas = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new CitasPagedResponse
        {
            Data = citas.Select(c => c.ToDto()).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
