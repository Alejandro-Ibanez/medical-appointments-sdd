using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public class ActualizarMedicoCommandHandler : IRequestHandler<ActualizarMedicoCommand, Medico>
{
    private readonly IApplicationDbContext _context;

    public ActualizarMedicoCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Medico> Handle(ActualizarMedicoCommand request, CancellationToken cancellationToken)
    {
        var medico = await _context.Medicos
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Medico), request.Id);

        medico.NombreCompleto = request.NombreCompleto;
        medico.DocumentoIdentidad = request.DocumentoIdentidad ?? medico.DocumentoIdentidad;
        medico.Especialidad = request.Especialidad;
        medico.NumeroColegiado = request.NumeroColegiado;
        medico.Telefono = request.Telefono ?? string.Empty;
        medico.Email = request.Email ?? string.Empty;
        medico.Activo = request.Activo;

        await _context.SaveChangesAsync(cancellationToken);

        return medico.ToDto();
    }
}
