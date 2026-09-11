using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;

namespace MedicalAppointments.Application.Features.Citas.Queries;

public class GetCitasCalendarioQueryHandler : IRequestHandler<GetCitasCalendarioQuery, List<CitaCalendarioResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetCitasCalendarioQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CitaCalendarioResponse>> Handle(GetCitasCalendarioQuery request, CancellationToken cancellationToken)
    {
        IQueryable<DomainCita> query = _context.Citas
            .Include(c => c.Paciente)
            .Include(c => c.Medico);

        if (request.Start.HasValue)
        {
            query = query.Where(c => c.FechaHora >= request.Start.Value);
        }

        if (request.End.HasValue)
        {
            query = query.Where(c => c.FechaHora <= request.End.Value);
        }

        // Si el usuario autenticado es un Medico, se ignora cualquier otro
        // filtro y se retornan estrictamente las citas asociadas a su
        // propio MedicoId (control de acceso aplicado sobre el IQueryable).
        if (request.MedicoIdDelUsuarioAutenticado.HasValue)
        {
            query = query.Where(c => c.MedicoId == request.MedicoIdDelUsuarioAutenticado.Value);
        }

        var citas = await query
            .OrderBy(c => c.FechaHora)
            .ToListAsync(cancellationToken);

        return citas.Select(cita =>
        {
            var pacienteNombre = cita.Paciente?.NombreCompleto ?? string.Empty;
            var motivo = string.IsNullOrWhiteSpace(cita.MotivoConsulta) ? "Consulta" : cita.MotivoConsulta;

            return new CitaCalendarioResponse
            {
                Id = cita.Id,
                Title = $"{pacienteNombre} - {motivo}",
                Start = cita.FechaHora,
                End = cita.FechaHora.AddMinutes(cita.DuracionMinutos),
                Estado = Enum.Parse<EstadoCita>(cita.Estado.ToString()),
                PacienteId = cita.PacienteId,
                PacienteNombre = pacienteNombre,
                MedicoNombre = cita.Medico?.NombreCompleto ?? string.Empty
            };
        }).ToList();
    }
}
