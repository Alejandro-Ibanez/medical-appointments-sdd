using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public class EliminarHorarioCommandHandler : IRequestHandler<EliminarHorarioCommand>
{
    private readonly IApplicationDbContext _context;

    public EliminarHorarioCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EliminarHorarioCommand request, CancellationToken cancellationToken)
    {
        var horario = await _context.HorariosAtencion
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(HorarioAtencion), request.Id);

        _context.HorariosAtencion.Remove(horario);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
