using MediatR;
using MedicalAppointments.Application.Common.Reports;

namespace MedicalAppointments.Application.Features.Reportes.Queries;

public record GetHistorialPacientePdfQuery(long PacienteId) : IRequest<ReporteBinario>;
