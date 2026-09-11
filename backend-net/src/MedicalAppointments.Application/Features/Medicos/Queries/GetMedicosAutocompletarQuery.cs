using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Medicos.Queries;

public record GetMedicosAutocompletarQuery(string Term) : IRequest<List<MedicoAutocompletado>>;
