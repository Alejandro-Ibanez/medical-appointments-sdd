using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Pacientes.Queries;

public record GetPacientesQuery(int PageNumber, int PageSize, string? SearchQuery) : IRequest<PagedResponse>;
