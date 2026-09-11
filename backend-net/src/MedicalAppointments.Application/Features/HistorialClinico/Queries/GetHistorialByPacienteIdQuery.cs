using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.HistorialClinico.Queries;

public record GetHistorialByPacienteIdQuery(long PacienteId) : IRequest<List<HistorialClinicoResponse>>;
