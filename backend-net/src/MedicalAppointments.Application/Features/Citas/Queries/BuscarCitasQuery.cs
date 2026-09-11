using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Citas.Queries;

public record BuscarCitasQuery(
    string? PalabraClave,
    long? MedicoId,
    long? PacienteId,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    int PageNumber,
    int PageSize) : IRequest<CitasPagedResponse>;
