using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Dashboard.Queries;

public record GetDashboardStatsQuery : IRequest<DashboardStats>;
