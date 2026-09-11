using MediatR;
using MedicalAppointments.Application.Common.Reports;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.Reportes.Queries;

public record GetAgendaMedicoReporteQuery(
    long MedicoId,
    Formato Formato,
    DateTime? FechaInicio,
    DateTime? FechaFin) : IRequest<ReporteBinario>;
