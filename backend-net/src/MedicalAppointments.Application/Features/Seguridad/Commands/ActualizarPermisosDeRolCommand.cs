using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Seguridad.Commands;

public record ActualizarPermisosDeRolCommand(string Rol, List<long> PermisosIds) : IRequest<List<PermisoResponse>>;
