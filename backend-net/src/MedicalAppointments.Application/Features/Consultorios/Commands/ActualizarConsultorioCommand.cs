using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Consultorios.Commands;

public record ActualizarConsultorioCommand(long Id, string Nombre, string Ubicacion) : IRequest<ConsultorioResponse>;
