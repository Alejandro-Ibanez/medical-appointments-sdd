using MediatR;

namespace MedicalAppointments.Application.Features.Citas.Queries;

public record GenerarReporteExcelQuery(
    string? PalabraClave,
    long? MedicoId,
    long? PacienteId,
    DateTime? FechaInicio,
    DateTime? FechaFin) : IRequest<byte[]>;
