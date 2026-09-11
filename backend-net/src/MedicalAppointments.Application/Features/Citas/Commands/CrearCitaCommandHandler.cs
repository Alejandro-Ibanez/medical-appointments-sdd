using MediatR;
using MedicalAppointments.Application.Common;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;
using DomainMedico = MedicalAppointments.Domain.Entities.Medico;
using DomainPaciente = MedicalAppointments.Domain.Entities.Paciente;
using EstadoCita = MedicalAppointments.Domain.Enums.EstadoCita;

namespace MedicalAppointments.Application.Features.Citas.Commands;

public class CrearCitaCommandHandler : IRequestHandler<CrearCitaCommand, CitaDetalleResponse>
{
    private const int DuracionCitaMinutosPorDefecto = 30;

    private readonly IApplicationDbContext _context;

    public CrearCitaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CitaDetalleResponse> Handle(CrearCitaCommand request, CancellationToken cancellationToken)
    {
        // Regla de negocio critica: esta estrictamente prohibido agendar
        // citas en una fecha/hora que ya paso.
        if (request.FechaHora < DateTime.Now)
        {
            throw new BusinessRuleException("No es posible agendar una cita en una fecha o deudas pasadas.");
        }

        var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.Id == request.PacienteId, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainPaciente), request.PacienteId);

        var medico = await _context.Medicos
            .Include(m => m.HorariosAtencion)
            .FirstOrDefaultAsync(m => m.Id == request.MedicoId, cancellationToken)
            ?? throw new NotFoundException(nameof(DomainMedico), request.MedicoId);

        var duracion = DuracionCitaMinutosPorDefecto;
        var inicioCita = request.FechaHora;
        var finCita = inicioCita.AddMinutes(duracion);

        var cita = new DomainCita
        {
            PacienteId = paciente.Id,
            MedicoId = medico.Id,
            Medico = medico,
            FechaHora = inicioCita,
            DuracionMinutos = duracion,
            MotivoConsulta = request.MotivoConsulta,
            Estado = EstadoCita.Programada
        };

        // a) El medico debe tener un HorarioAtencion que cubra ese dia y rango horario.
        var horarioQueCubre = HorarioResolver.ResolverHorario(cita);

        if (horarioQueCubre is null)
        {
            throw new BusinessRuleException(
                "La fecha y hora solicitada esta fuera del horario de atencion del medico.");
        }

        // Trae, en una sola consulta, todas las citas activas que se solapan con el
        // rango solicitado (de cualquier medico), junto con los horarios del medico
        // de cada una para poder resolver a que consultorio estaba asignada.
        var citasSolapadas = await _context.Citas
            .Include(c => c.Medico)
                .ThenInclude(m => m!.HorariosAtencion)
            .Where(c => c.Estado != EstadoCita.Cancelada
                && c.FechaHora < finCita
                && inicioCita < c.FechaHora.AddMinutes(c.DuracionMinutos))
            .ToListAsync(cancellationToken);

        // b) El medico no puede tener otra cita activa que se solape con este rango.
        var medicoOcupado = citasSolapadas.Any(c => c.MedicoId == request.MedicoId);
        if (medicoOcupado)
        {
            throw new BusinessRuleException(
                "El medico ya tiene otra cita agendada que se solapa con ese mismo horario.");
        }

        // c) El consultorio fisico resuelto para este horario no puede estar ocupado
        // por otra cita (de otro medico) en ese mismo rango.
        var consultorioId = horarioQueCubre.ConsultorioId;
        var consultorioOcupado = citasSolapadas.Any(c => HorarioResolver.ResolverConsultorioId(c) == consultorioId);
        if (consultorioOcupado)
        {
            throw new BusinessRuleException(
                "El consultorio asignado a ese horario ya esta ocupado por otra cita en ese mismo rango.");
        }

        _context.Citas.Add(cita);
        await _context.SaveChangesAsync(cancellationToken);

        cita.Paciente = paciente;

        return cita.ToDetalleDto();
    }
}
