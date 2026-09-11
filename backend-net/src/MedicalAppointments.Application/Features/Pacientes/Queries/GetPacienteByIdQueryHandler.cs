using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Pacientes.Queries;

public class GetPacienteByIdQueryHandler : IRequestHandler<GetPacienteByIdQuery, Paciente>
{
    private readonly IApplicationDbContext _context;

    public GetPacienteByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Paciente> Handle(GetPacienteByIdQuery request, CancellationToken cancellationToken)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Paciente), request.Id);

        return paciente.ToDto();
    }
}
