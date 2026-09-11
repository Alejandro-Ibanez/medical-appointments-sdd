using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using DomainPaciente = MedicalAppointments.Domain.Entities.Paciente;

namespace MedicalAppointments.Application.Features.Pacientes.Queries;

public class GetPacientesQueryHandler : IRequestHandler<GetPacientesQuery, PagedResponse>
{
    private readonly IApplicationDbContext _context;

    public GetPacientesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse> Handle(GetPacientesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        IQueryable<DomainPaciente> query = _context.Pacientes;

        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            // ToLower() en ambos lados para que la busqueda sea insensible a
            // mayusculas/minusculas (SQLite traduce Contains() a instr(),
            // que es sensible a mayusculas por defecto).
            var texto = request.SearchQuery.Trim().ToLower();
            query = query.Where(p =>
                p.NombreCompleto.ToLower().Contains(texto) ||
                p.DocumentoIdentidad.ToLower().Contains(texto) ||
                p.Telefono.ToLower().Contains(texto));
        }

        query = query.OrderBy(p => p.NombreCompleto);

        var totalCount = await query.CountAsync(cancellationToken);

        var pacientes = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResponse
        {
            Data = pacientes.Select(p => p.ToDto()).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
