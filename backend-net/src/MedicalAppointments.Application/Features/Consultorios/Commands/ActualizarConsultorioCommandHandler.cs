using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Consultorios.Commands;

public class ActualizarConsultorioCommandHandler : IRequestHandler<ActualizarConsultorioCommand, ConsultorioResponse>
{
    private readonly IApplicationDbContext _context;

    public ActualizarConsultorioCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ConsultorioResponse> Handle(ActualizarConsultorioCommand request, CancellationToken cancellationToken)
    {
        var consultorio = await _context.Consultorios
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Consultorio), request.Id);

        consultorio.Nombre = request.Nombre;
        consultorio.Ubicacion = request.Ubicacion;

        await _context.SaveChangesAsync(cancellationToken);

        return consultorio.ToDto();
    }
}
