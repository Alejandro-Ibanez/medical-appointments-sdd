using MedicalAppointments.Application.DTOs;
using DomainEntities = MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class CitaMappingExtensions
{
    public static Paciente ToDto(this DomainEntities.Paciente paciente) => new()
    {
        Id = paciente.Id,
        NombreCompleto = paciente.NombreCompleto,
        DocumentoIdentidad = paciente.DocumentoIdentidad,
        FechaNacimiento = paciente.FechaNacimiento,
        Telefono = paciente.Telefono,
        Email = paciente.Email,
        Direccion = paciente.Direccion,
        Activo = paciente.Activo,
        Genero = Enum.Parse<GeneroPaciente>(paciente.Genero.ToString()),
        Alergias = string.IsNullOrWhiteSpace(paciente.Alergias) ? null : paciente.Alergias
    };

    public static Medico ToDto(this DomainEntities.Medico medico) => new()
    {
        Id = medico.Id,
        NombreCompleto = medico.NombreCompleto,
        DocumentoIdentidad = medico.DocumentoIdentidad,
        Especialidad = medico.Especialidad,
        NumeroColegiado = medico.NumeroColegiado,
        Telefono = medico.Telefono,
        Email = medico.Email,
        Activo = medico.Activo
    };

    public static Cita ToDto(this DomainEntities.Cita cita)
    {
        if (cita.Paciente is null || cita.Medico is null)
        {
            throw new InvalidOperationException(
                "La cita debe incluir el Paciente y el Medico cargados para poder mapearse a su DTO.");
        }

        return new Cita
        {
            Id = cita.Id,
            Paciente = cita.Paciente.ToDto(),
            Medico = cita.Medico.ToDto(),
            FechaHora = cita.FechaHora,
            Estado = Enum.Parse<EstadoCita>(cita.Estado.ToString()),
            Motivo = cita.MotivoConsulta,
            Observaciones = string.Empty,
            FechaCreacion = cita.FechaCreacion
        };
    }

    public static CitaDetalleResponse ToDetalleDto(this DomainEntities.Cita cita)
    {
        if (cita.Paciente is null || cita.Medico is null)
        {
            throw new InvalidOperationException(
                "La cita debe incluir el Paciente y el Medico cargados para poder mapearse a su DTO.");
        }

        return new CitaDetalleResponse
        {
            Id = cita.Id,
            FechaHora = cita.FechaHora,
            MotivoConsulta = cita.MotivoConsulta,
            Estado = Enum.Parse<EstadoCita>(cita.Estado.ToString()),
            Paciente = new PacienteResumenCita
            {
                Id = cita.Paciente.Id,
                NombreCompleto = cita.Paciente.NombreCompleto,
                Alergias = string.IsNullOrWhiteSpace(cita.Paciente.Alergias) ? null : cita.Paciente.Alergias
            },
            Medico = new MedicoResumenCita
            {
                Id = cita.Medico.Id,
                NombreCompleto = cita.Medico.NombreCompleto,
                Especialidad = cita.Medico.Especialidad
            }
        };
    }
}
