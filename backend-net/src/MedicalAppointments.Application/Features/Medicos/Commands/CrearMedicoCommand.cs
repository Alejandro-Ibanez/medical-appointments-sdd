using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public record CrearMedicoCommand(
    string NombreCompleto,
    string DocumentoIdentidad,
    string Especialidad,
    string NumeroColegiado,
    string Telefono,
    string Email,
    string Username,
    string Password) : IRequest<Medico>;
