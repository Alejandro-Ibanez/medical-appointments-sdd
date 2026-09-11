using MediatR;
using MedicalAppointments.Application.Common.Reports;

namespace MedicalAppointments.Application.Features.Reportes.Queries;

public record GetOcupacionConsultoriosQuery(DateTime? FechaInicio, DateTime? FechaFin) : IRequest<ReporteBinario>;
