using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using EstadoCita = MedicalAppointments.Domain.Enums.EstadoCita;

namespace MedicalAppointments.Application.Features.Dashboard.Queries;

public class GetDashboardMetricasQueryHandler : IRequestHandler<GetDashboardMetricasQuery, DashboardMetricas>
{
    private readonly IApplicationDbContext _context;

    public GetDashboardMetricasQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardMetricas> Handle(GetDashboardMetricasQuery request, CancellationToken cancellationToken)
    {
        var hoyInicio = DateTime.Today;
        var hoyFin = hoyInicio.AddDays(1);

        var citasHoy = await _context.Citas
            .CountAsync(c => c.FechaHora >= hoyInicio && c.FechaHora < hoyFin, cancellationToken);

        var totalPacientes = await _context.Pacientes.CountAsync(cancellationToken);

        var conteosPorEstado = await _context.Citas
            .GroupBy(c => c.Estado)
            .Select(g => new { Estado = g.Key, Total = g.Count() })
            .ToListAsync(cancellationToken);

        int ContarEstado(EstadoCita estado) => conteosPorEstado.FirstOrDefault(c => c.Estado == estado)?.Total ?? 0;

        var demandasPorEspecialidad = await _context.Citas
            .GroupBy(c => c.Medico!.Especialidad)
            .Select(g => new DemandaEspecialidad
            {
                Especialidad = g.Key,
                TotalCitas = g.Count()
            })
            .OrderByDescending(d => d.TotalCitas)
            .ToListAsync(cancellationToken);

        return new DashboardMetricas
        {
            CitasHoy = citasHoy,
            TotalPacientes = totalPacientes,
            ResumenEstados = new ResumenEstadosCitas
            {
                Pendientes = ContarEstado(EstadoCita.Programada),
                Confirmadas = ContarEstado(EstadoCita.Confirmada),
                Completadas = ContarEstado(EstadoCita.Completada),
                Canceladas = ContarEstado(EstadoCita.Cancelada)
            },
            DemandasPorEspecialidad = demandasPorEspecialidad
        };
    }
}
