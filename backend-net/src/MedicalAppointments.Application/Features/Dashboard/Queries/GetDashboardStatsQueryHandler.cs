using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Dashboard.Queries;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStats>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStats> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var hoyInicio = DateTime.Today;
        var hoyFin = hoyInicio.AddDays(1);

        var citasHoy = await _context.Citas
            .CountAsync(c => c.FechaHora >= hoyInicio && c.FechaHora < hoyFin, cancellationToken);

        var totalPacientes = await _context.Pacientes.CountAsync(cancellationToken);

        var citasPendientes = await _context.Citas
            .CountAsync(c => c.Estado == MedicalAppointments.Domain.Enums.EstadoCita.Programada, cancellationToken);

        var citasConfirmadas = await _context.Citas
            .CountAsync(c => c.Estado == MedicalAppointments.Domain.Enums.EstadoCita.Confirmada, cancellationToken);

        var citasCanceladas = await _context.Citas
            .CountAsync(c => c.Estado == MedicalAppointments.Domain.Enums.EstadoCita.Cancelada, cancellationToken);

        return new DashboardStats
        {
            CitasHoy = citasHoy,
            TotalPacientes = totalPacientes,
            CitasPendientes = citasPendientes,
            CitasConfirmadas = citasConfirmadas,
            CitasCanceladas = citasCanceladas
        };
    }
}
