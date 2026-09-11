using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public record CrearUsuarioCommand(
    string NombreCompleto,
    string Username,
    string Password,
    long RolId,
    long? MedicoId) : IRequest<UsuarioResponse>;
