using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Seguridad.Queries;

public record GetPermisosDeRolQuery(string Rol) : IRequest<List<PermisoResponse>>;
