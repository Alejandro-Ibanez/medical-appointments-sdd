using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Medicos.Queries;

public record GetMedicosQuery(int PageNumber, int PageSize, string? SearchQuery) : IRequest<MedicosPagedResponse>;
