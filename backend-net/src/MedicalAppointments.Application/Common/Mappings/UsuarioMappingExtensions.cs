using MedicalAppointments.Application.DTOs;
using DomainEntities = MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class UsuarioMappingExtensions
{
    public static UsuarioResponse ToDto(this DomainEntities.Usuario usuario)
    {
        if (usuario.Rol is null)
        {
            throw new InvalidOperationException(
                "El usuario debe tener su Rol cargado para poder mapearse a su DTO.");
        }

        return new UsuarioResponse
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Username = usuario.Username,
            RolNombre = usuario.Rol.Nombre,
            Activo = usuario.Activo,
            MedicoId = usuario.MedicoId
        };
    }
}
