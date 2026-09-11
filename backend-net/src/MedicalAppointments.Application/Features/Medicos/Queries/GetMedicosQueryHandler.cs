using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using DomainMedico = MedicalAppointments.Domain.Entities.Medico;

namespace MedicalAppointments.Application.Features.Medicos.Queries;

public class GetMedicosQueryHandler : IRequestHandler<GetMedicosQuery, MedicosPagedResponse>
{
    private readonly IApplicationDbContext _context;

    public GetMedicosQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MedicosPagedResponse> Handle(GetMedicosQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        IQueryable<DomainMedico> query = _context.Medicos;

        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            // El "documento" de un Medico es su Numero de Colegiado (no tiene DocumentoIdentidad propio).
            // ToLower() en ambos lados para que la busqueda sea insensible a mayusculas/minusculas.
            var texto = request.SearchQuery.Trim().ToLower();
            query = query.Where(m =>
                m.NombreCompleto.ToLower().Contains(texto) ||
                m.NumeroColegiado.ToLower().Contains(texto) ||
                m.Telefono.ToLower().Contains(texto));
        }

        query = query.OrderBy(m => m.NombreCompleto);

        var totalCount = await query.CountAsync(cancellationToken);

        var medicos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new MedicosPagedResponse
        {
            Data = medicos.Select(m => m.ToDto()).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
