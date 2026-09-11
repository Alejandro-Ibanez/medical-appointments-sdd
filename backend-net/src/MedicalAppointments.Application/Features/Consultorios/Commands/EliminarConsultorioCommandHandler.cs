using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Consultorios.Commands;

public class EliminarConsultorioCommandHandler : IRequestHandler<EliminarConsultorioCommand>
{
    private readonly IApplicationDbContext _context;

    public EliminarConsultorioCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(EliminarConsultorioCommand request, CancellationToken cancellationToken)
    {
        var consultorio = await _context.Consultorios
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Consultorio), request.Id);

        // Baja logica: se marca como inactivo en lugar de eliminarlo fisicamente.
        consultorio.Activo = false;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
