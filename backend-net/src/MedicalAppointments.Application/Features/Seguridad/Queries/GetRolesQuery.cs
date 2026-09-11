using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Seguridad.Queries;

public record GetRolesQuery : IRequest<List<RolResponse>>;
