using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Usuarios.Commands;

public record LoginCommand(string Usuario, string Contrasena) : IRequest<LoginResponse>;
