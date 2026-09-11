using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Features.Pacientes.Commands;

public class ActualizarPacienteCommandHandler : IRequestHandler<ActualizarPacienteCommand, Paciente>
{
    private readonly IApplicationDbContext _context;

    public ActualizarPacienteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Paciente> Handle(ActualizarPacienteCommand request, CancellationToken cancellationToken)
    {
        var paciente = await _context.Pacientes
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Paciente), request.Id);

        paciente.NombreCompleto = request.NombreCompleto;
        paciente.DocumentoIdentidad = request.DocumentoIdentidad;
        paciente.FechaNacimiento = request.FechaNacimiento;
        paciente.Telefono = request.Telefono ?? string.Empty;
        paciente.Email = request.Email ?? string.Empty;
        paciente.Direccion = request.Direccion ?? string.Empty;
        paciente.Activo = request.Activo;
        paciente.Genero = Enum.Parse<MedicalAppointments.Domain.Enums.GeneroPaciente>(request.Genero.ToString());
        paciente.Alergias = request.Alergias ?? string.Empty;

        await _context.SaveChangesAsync(cancellationToken);

        return paciente.ToDto();
    }
}
