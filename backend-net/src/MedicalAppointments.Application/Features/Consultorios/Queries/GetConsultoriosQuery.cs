using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Consultorios.Queries;

public record GetConsultoriosQuery : IRequest<List<ConsultorioResponse>>;
