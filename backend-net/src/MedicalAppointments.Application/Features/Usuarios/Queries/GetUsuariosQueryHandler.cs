using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using DomainUsuario = MedicalAppointments.Domain.Entities.Usuario;

namespace MedicalAppointments.Application.Features.Usuarios.Queries;

public class GetUsuariosQueryHandler : IRequestHandler<GetUsuariosQuery, UsuariosPagedResponse>
{
    private readonly IApplicationDbContext _context;

    public GetUsuariosQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UsuariosPagedResponse> Handle(GetUsuariosQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;

        IQueryable<DomainUsuario> query = _context.Usuarios.Include(u => u.Rol);

        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            // ToLower() en ambos lados para que la busqueda sea insensible a
            // mayusculas/minusculas (SQLite traduce Contains() a instr(),
            // que es sensible a mayusculas por defecto).
            var texto = request.SearchQuery.Trim().ToLower();
            query = query.Where(u =>
                u.NombreCompleto.ToLower().Contains(texto) ||
                u.Username.ToLower().Contains(texto));
        }

        query = query.OrderBy(u => u.NombreCompleto);

        var totalCount = await query.CountAsync(cancellationToken);

        var usuarios = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        return new UsuariosPagedResponse
        {
            Data = usuarios.Select(u => u.ToDto()).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages
        };
    }
}
