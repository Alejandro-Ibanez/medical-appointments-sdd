using MediatR;
using MedicalAppointments.Application.Common;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Reports;
using MedicalAppointments.Application.Common.Reports.Excel;
using Microsoft.EntityFrameworkCore;
using EstadoCita = MedicalAppointments.Domain.Enums.EstadoCita;

namespace MedicalAppointments.Application.Features.Reportes.Queries;

public class GetOcupacionConsultoriosQueryHandler : IRequestHandler<GetOcupacionConsultoriosQuery, ReporteBinario>
{
    private readonly IApplicationDbContext _context;

    public GetOcupacionConsultoriosQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReporteBinario> Handle(GetOcupacionConsultoriosQuery request, CancellationToken cancellationToken)
    {
        var consultorios = await _context.Consultorios
            .OrderBy(c => c.Nombre)
            .ToListAsync(cancellationToken);

        var query = _context.Citas
            .Include(c => c.Medico)
                .ThenInclude(m => m!.HorariosAtencion)
            .Where(c => c.Estado != EstadoCita.Cancelada)
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

        var citas = await query.ToListAsync(cancellationToken);

        var citasPorConsultorio = citas
            .Select(c => new { Cita = c, ConsultorioId = HorarioResolver.ResolverConsultorioId(c) })
            .Where(x => x.ConsultorioId.HasValue)
            .GroupBy(x => x.ConsultorioId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Cita).ToList());

        var ocupacion = consultorios
            .Select(consultorio =>
            {
                var citasDelConsultorio = citasPorConsultorio.TryGetValue(consultorio.Id, out var lista)
                    ? lista
                    : new List<Domain.Entities.Cita>();

                var horas = citasDelConsultorio.Sum(c => c.DuracionMinutos) / 60.0;

                return new OcupacionConsultorioItem(consultorio, citasDelConsultorio.Count, horas);
            })
            .ToList();

        var contenido = OcupacionConsultoriosExcelReportBuilder.Construir(ocupacion, request.FechaInicio, request.FechaFin);

        return new ReporteBinario(
            contenido,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ocupacion_consultorios.xlsx");
    }
}
