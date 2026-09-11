using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public record CambiarEstadoUsuarioCommand(long Id, bool Activo) : IRequest<UsuarioResponse>;
