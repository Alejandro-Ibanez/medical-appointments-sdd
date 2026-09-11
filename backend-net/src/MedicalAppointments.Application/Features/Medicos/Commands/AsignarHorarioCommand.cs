using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public record AsignarHorarioCommand(
    long MedicoId,
    DiaSemana DiaSemana,
    TimeOnly HoraInicio,
    TimeOnly HoraFin,
    long ConsultorioId) : IRequest<HorarioAtencionResponse>;
