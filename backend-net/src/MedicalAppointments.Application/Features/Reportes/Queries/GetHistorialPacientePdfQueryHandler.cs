using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Reports;
using MedicalAppointments.Application.Common.Reports.Pdf;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Reportes.Queries;

public class GetHistorialPacientePdfQueryHandler : IRequestHandler<GetHistorialPacientePdfQuery, ReporteBinario>
{
    private readonly IApplicationDbContext _context;

    public GetHistorialPacientePdfQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteBinario> Handle(GetHistorialPacientePdfQuery request, CancellationToken cancellationToken)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id == request.PacienteId, cancellationToken)
            ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Paciente), request.PacienteId);

        var historial = await _context.HistorialesClinicos
            .Where(h => h.PacienteId == request.PacienteId)
            .OrderBy(h => h.FechaHora)
            .ToListAsync(cancellationToken);

        var contenido = HistorialPacientePdfReportBuilder.Construir(paciente, historial);

        var nombreArchivo = $"historial_clinico_{paciente.DocumentoIdentidad}.pdf";

        return new ReporteBinario(contenido, "application/pdf", nombreArchivo);
    }
}
