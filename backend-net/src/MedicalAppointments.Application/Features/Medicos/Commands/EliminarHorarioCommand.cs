using MediatR;

namespace MedicalAppointments.Application.Features.Medicos.Commands;

public record EliminarHorarioCommand(long Id) : IRequest;
