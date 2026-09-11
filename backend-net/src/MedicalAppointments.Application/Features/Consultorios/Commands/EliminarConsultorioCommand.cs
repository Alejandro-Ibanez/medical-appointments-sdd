using MediatR;

namespace MedicalAppointments.Application.Features.Consultorios.Commands;

public record EliminarConsultorioCommand(long Id) : IRequest;
