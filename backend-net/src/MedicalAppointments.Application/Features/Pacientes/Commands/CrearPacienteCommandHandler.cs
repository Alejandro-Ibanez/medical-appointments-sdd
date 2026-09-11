using MediatR;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.DTOs;
using DomainPaciente = MedicalAppointments.Domain.Entities.Paciente;

namespace MedicalAppointments.Application.Features.Pacientes.Commands;

public class CrearPacienteCommandHandler : IRequestHandler<CrearPacienteCommand, Paciente>
{
    private readonly IApplicationDbContext _context;

    public CrearPacienteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Paciente> Handle(CrearPacienteCommand request, CancellationToken cancellationToken)
    {
        var paciente = new DomainPaciente
        {
            NombreCompleto = request.NombreCompleto,
            DocumentoIdentidad = request.DocumentoIdentidad,
            FechaNacimiento = request.FechaNacimiento,
            Telefono = request.Telefono ?? string.Empty,
            Email = request.Email ?? string.Empty,
            Direccion = request.Direccion ?? string.Empty,
            Activo = request.Activo,
            Genero = Enum.Parse<MedicalAppointments.Domain.Enums.GeneroPaciente>(request.Genero.ToString()),
            Alergias = request.Alergias ?? string.Empty
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync(cancellationToken);

        return paciente.ToDto();
    }
}
