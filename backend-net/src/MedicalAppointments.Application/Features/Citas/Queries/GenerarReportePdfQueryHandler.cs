using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Reports.Pdf;
using Microsoft.EntityFrameworkCore;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;

namespace MedicalAppointments.Application.Features.Citas.Queries;

public class GenerarReportePdfQueryHandler : IRequestHandler<GenerarReportePdfQuery, byte[]>
{
    private readonly IApplicationDbContext _context;

    public GenerarReportePdfQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> Handle(GenerarReportePdfQuery request, CancellationToken cancellationToken)
    {
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

        var citas = await query
            .OrderByDescending(c => c.FechaHora)
            .ToListAsync(cancellationToken);

        return CitasPdfReportBuilder.Construir(citas);
    }
}
