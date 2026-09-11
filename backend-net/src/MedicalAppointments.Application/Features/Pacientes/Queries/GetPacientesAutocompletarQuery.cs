using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Pacientes.Queries;

public record GetPacientesAutocompletarQuery(string Term) : IRequest<List<PacienteAutocompletado>>;
