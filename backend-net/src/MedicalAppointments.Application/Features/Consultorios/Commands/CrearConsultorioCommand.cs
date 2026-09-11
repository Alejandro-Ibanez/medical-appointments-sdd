using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Consultorios.Commands;

public record CrearConsultorioCommand(string Nombre, string Ubicacion) : IRequest<ConsultorioResponse>;
