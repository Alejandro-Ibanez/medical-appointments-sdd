using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Reports;
using MedicalAppointments.Application.Common.Reports.Excel;
using MedicalAppointments.Application.Common.Reports.Pdf;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Reportes.Queries;

public class GetAgendaMedicoReporteQueryHandler : IRequestHandler<GetAgendaMedicoReporteQuery, ReporteBinario>
{
    private readonly IApplicationDbContext _context;

    public GetAgendaMedicoReporteQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteBinario> Handle(GetAgendaMedicoReporteQuery request, CancellationToken cancellationToken)
    {
        var medico = await _context.Medicos
            .FirstOrDefaultAsync(m => m.Id == request.MedicoId, cancellationToken)
            ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Medico), request.MedicoId);

        var query = _context.Citas
            .Include(c => c.Paciente)
            .Where(c => c.MedicoId == request.MedicoId)
            .AsQueryable();

        if (request.FechaInicio.HasValue)
        {
            query = query.Where(c => c.FechaHora >= request.FechaInicio.Value.Date);
        }

        if (request.FechaFin.HasValue)
        {
            var finDelDia = request.FechaFin.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(c => c.FechaHora <= finDelDia);
        }

        var citas = await query
            .OrderBy(c => c.FechaHora)
            .ToListAsync(cancellationToken);

        var documentoBase = medico.NombreCompleto.Replace(' ', '_');

        if (request.Formato == Formato.Excel)
        {
            var contenidoExcel = AgendaMedicoExcelReportBuilder.Construir(medico, citas);
            return new ReporteBinario(
                contenidoExcel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"agenda_{documentoBase}.xlsx");
        }

        var contenidoPdf = AgendaMedicoPdfReportBuilder.Construir(medico, citas, request.FechaInicio, request.FechaFin);
        return new ReporteBinario(contenidoPdf, "application/pdf", $"agenda_{documentoBase}.pdf");
    }
}
