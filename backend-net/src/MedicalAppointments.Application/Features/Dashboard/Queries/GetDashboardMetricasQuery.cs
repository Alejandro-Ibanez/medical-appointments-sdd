using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Dashboard.Queries;

public record GetDashboardMetricasQuery : IRequest<DashboardMetricas>;
