using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainHistorial = MedicalAppointments.Domain.Entities.HistorialClinico;
using DomainEstadoCita = MedicalAppointments.Domain.Enums.EstadoCita;

namespace MedicalAppointments.Application.Features.HistorialClinico.Commands;

public class CrearEntradaHistorialCommandHandler : IRequestHandler<CrearEntradaHistorialCommand, HistorialClinicoResponse>
{
    private readonly IApplicationDbContext _context;

    public CrearEntradaHistorialCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HistorialClinicoResponse> Handle(CrearEntradaHistorialCommand request, CancellationToken cancellationToken)
    {
        var pacienteExiste = await _context.Pacientes.AnyAsync(p => p.Id == request.PacienteId, cancellationToken);
        if (!pacienteExiste)
        {
            throw new NotFoundException(nameof(Paciente), request.PacienteId);
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Medico)
            .FirstOrDefaultAsync(u => u.Id == request.UsuarioAutenticadoId, cancellationToken);

        var medico = usuario?.Medico
            ?? throw new BusinessRuleException("El usuario autenticado no tiene un perfil de Medico asociado.");

        var entrada = new DomainHistorial
        {
            PacienteId = request.PacienteId,
            MedicoNombre = medico.NombreCompleto,
            FechaHora = DateTime.UtcNow,
            Diagnostico = request.Diagnostico,
            Tratamiento = request.Tratamiento ?? string.Empty,
            Notas = request.Notas ?? string.Empty
        };

        _context.HistorialesClinicos.Add(entrada);

        if (request.CitaId.HasValue)
        {
            var cita = await _context.Citas
                .FirstOrDefaultAsync(c => c.Id == request.CitaId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Cita), request.CitaId.Value);

            cita.Estado = DomainEstadoCita.Completada;
        }

        // Una sola llamada a SaveChangesAsync: la nueva entrada del historial
        // y la actualizacion del estado de la cita (si aplica) se persisten
        // juntas, en la misma transaccion implicita de EF Core.
        await _context.SaveChangesAsync(cancellationToken);

        return entrada.ToDto();
    }
}
