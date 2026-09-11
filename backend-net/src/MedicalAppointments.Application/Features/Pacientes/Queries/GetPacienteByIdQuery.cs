using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Pacientes.Queries;

public record GetPacienteByIdQuery(long Id) : IRequest<Paciente>;
