using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainCita = MedicalAppointments.Domain.Entities.Cita;
using EstadoCita = MedicalAppointments.Domain.Enums.EstadoCita;

namespace MedicalAppointments.Application.Features.Citas.Commands;

public class CancelarCitaCommandHandler : IRequestHandler<CancelarCitaCommand, CitaDetalleResponse>
{
    private readonly IApplicationDbContext _context;

    public CancelarCitaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CitaDetalleResponse> Handle(CancelarCitaCommand request, CancellationToken cancellationToken)
    {
        var cita = await _context.Citas
            .Include(c => c.Paciente)
            .Include(c => c.Medico)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(MedicalAppointments.Domain.Entities.Cita), request.Id);

        cita.Estado = EstadoCita.Cancelada;
        cita.MotivoCancelacion = request.MotivoCancelacion;

        await _context.SaveChangesAsync(cancellationToken);

        return cita.ToDetalleDto();
    }
}
