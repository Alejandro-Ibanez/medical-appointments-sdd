using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Pacientes.Commands;

public record CrearPacienteCommand(
    string NombreCompleto,
    string DocumentoIdentidad,
    DateTime FechaNacimiento,
    string? Telefono,
    string? Email,
    string? Direccion,
    bool Activo,
    GeneroPaciente Genero,
    string? Alergias) : IRequest<Paciente>;
