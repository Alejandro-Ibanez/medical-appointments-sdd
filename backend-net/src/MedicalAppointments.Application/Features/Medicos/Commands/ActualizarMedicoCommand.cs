using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public record ActualizarMedicoCommand(
    long Id,
    string NombreCompleto,
    string? DocumentoIdentidad,
    string Especialidad,
    string NumeroColegiado,
    string? Telefono,
    string? Email,
    bool Activo) : IRequest<Medico>;
