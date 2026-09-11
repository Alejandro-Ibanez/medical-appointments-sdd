using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Citas.Commands;

public record CancelarCitaCommand(long Id, string MotivoCancelacion) : IRequest<CitaDetalleResponse>;
