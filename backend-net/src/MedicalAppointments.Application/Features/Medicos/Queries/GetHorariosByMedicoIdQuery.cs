using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Medicos.Queries;

public record GetHorariosByMedicoIdQuery(long MedicoId) : IRequest<List<HorarioAtencionResponse>>;
