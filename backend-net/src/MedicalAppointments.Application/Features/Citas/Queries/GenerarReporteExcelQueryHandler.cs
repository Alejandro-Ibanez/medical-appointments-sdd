using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Reports.Excel;
using Microsoft.EntityFrameworkCore;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;

namespace MedicalAppointments.Application.Features.Citas.Queries;

public class GenerarReporteExcelQueryHandler : IRequestHandler<GenerarReporteExcelQuery, byte[]>
{
    private readonly IApplicationDbContext _context;

    public GenerarReporteExcelQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> Handle(GenerarReporteExcelQuery request, CancellationToken cancellationToken)
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

        return CitasExcelReportBuilder.Construir(citas);
    }
}
