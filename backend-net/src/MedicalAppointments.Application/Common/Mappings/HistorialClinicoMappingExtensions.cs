using MedicalAppointments.Application.DTOs;
using DomainEntities = MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class HistorialClinicoMappingExtensions
{
    public static HistorialClinicoResponse ToDto(this DomainEntities.HistorialClinico entrada) => new()
    {
        Id = entrada.Id,
        FechaHora = entrada.FechaHora,
        MedicoNombre = entrada.MedicoNombre,
        Diagnostico = entrada.Diagnostico,
        Tratamiento = entrada.Tratamiento,
        Notas = entrada.Notas
    };
}
