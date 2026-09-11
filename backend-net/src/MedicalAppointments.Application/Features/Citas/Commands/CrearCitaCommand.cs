using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Citas.Commands;

public record CrearCitaCommand(
    long PacienteId,
    long MedicoId,
    DateTime FechaHora,
    string MotivoConsulta) : IRequest<CitaDetalleResponse>;
