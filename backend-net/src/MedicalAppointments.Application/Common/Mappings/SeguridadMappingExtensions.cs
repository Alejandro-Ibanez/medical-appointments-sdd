using MedicalAppointments.Application.DTOs;
using DomainEntities = MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class SeguridadMappingExtensions
{
    public static PermisoResponse ToDto(this DomainEntities.Permiso permiso) => new()
    {
        Id = permiso.Id,
        Nombre = permiso.Nombre,
        Codigo = permiso.Codigo
    };

    public static RolResponse ToDto(this DomainEntities.Rol rol) => new()
    {
        Id = rol.Id,
        Nombre = rol.Nombre
    };
}
