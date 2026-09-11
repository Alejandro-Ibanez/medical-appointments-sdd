using MediatR;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public record CambiarPasswordCommand(long UsuarioId, string PasswordActual, string PasswordNueva) : IRequest;
