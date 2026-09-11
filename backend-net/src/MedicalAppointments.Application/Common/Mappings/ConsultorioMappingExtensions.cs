using MedicalAppointments.Application.DTOs;
using DomainEntities = MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class ConsultorioMappingExtensions
{
    public static ConsultorioResponse ToDto(this DomainEntities.Consultorio consultorio) => new()
    {
        Id = consultorio.Id,
        Nombre = consultorio.Nombre,
        Ubicacion = consultorio.Ubicacion,
        Activo = consultorio.Activo
    };

    public static HorarioAtencionResponse ToDto(this DomainEntities.HorarioAtencion horario)
    {
        if (horario.Consultorio is null)
        {
            throw new InvalidOperationException(
                "El horario de atencion debe incluir el Consultorio cargado para poder mapearse a su DTO.");
        }

        return new HorarioAtencionResponse
        {
            Id = horario.Id,
            MedicoId = horario.MedicoId,
            DiaSemana = Enum.Parse<DiaSemana>(horario.DiaSemana.ToString()),
            HoraInicio = horario.HoraInicio.ToString("HH:mm"),
            HoraFin = horario.HoraFin.ToString("HH:mm"),
            Consultorio = horario.Consultorio.ToDto()
        };
    }
}
