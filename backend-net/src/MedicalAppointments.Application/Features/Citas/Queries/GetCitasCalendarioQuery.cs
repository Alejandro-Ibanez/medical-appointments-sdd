using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Citas.Queries;

public record GetCitasCalendarioQuery(
    DateTime? Start,
    DateTime? End,
    long? MedicoIdDelUsuarioAutenticado) : IRequest<List<CitaCalendarioResponse>>;
