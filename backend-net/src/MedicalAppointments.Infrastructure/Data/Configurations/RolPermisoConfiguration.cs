using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
{
    public void Configure(EntityTypeBuilder<RolPermiso> builder)
    {
        builder.ToTable("RolesPermisos");
        builder.HasKey(rp => new { rp.RolId, rp.PermisoId });

        builder.HasOne(rp => rp.Rol)
            .WithMany(r => r.RolPermisos)
            .HasForeignKey(rp => rp.RolId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permiso)
            .WithMany(p => p.RolPermisos)
            .HasForeignKey(rp => rp.PermisoId)
            .OnDelete(DeleteBehavior.Cascade);

        const long admin = 1;
        const long medico = 2;
        const long recepcionista = 3;

        builder.HasData(
            // Admin: todos los permisos del sistema.
            new RolPermiso { RolId = admin, PermisoId = 1 },
            new RolPermiso { RolId = admin, PermisoId = 2 },
            new RolPermiso { RolId = admin, PermisoId = 3 },
            new RolPermiso { RolId = admin, PermisoId = 4 },
            new RolPermiso { RolId = admin, PermisoId = 5 },
            new RolPermiso { RolId = admin, PermisoId = 6 },
            new RolPermiso { RolId = admin, PermisoId = 7 },
            new RolPermiso { RolId = admin, PermisoId = 8 },
            new RolPermiso { RolId = admin, PermisoId = 9 },
            new RolPermiso { RolId = admin, PermisoId = 10 },
            new RolPermiso { RolId = admin, PermisoId = 11 },

            // Medico: permisos clinicos.
            new RolPermiso { RolId = medico, PermisoId = 4 }, // dashboard.ver
            new RolPermiso { RolId = medico, PermisoId = 7 }, // pacientes.leer
            new RolPermiso { RolId = medico, PermisoId = 8 }, // citas.escribir
            new RolPermiso { RolId = medico, PermisoId = 9 }, // citas.leer
            new RolPermiso { RolId = medico, PermisoId = 10 }, // historial.escribir
            new RolPermiso { RolId = medico, PermisoId = 11 }, // historial.leer

            // Recepcionista: permisos de agenda.
            new RolPermiso { RolId = recepcionista, PermisoId = 4 }, // dashboard.ver
            new RolPermiso { RolId = recepcionista, PermisoId = 5 }, // reportes.descargar
            new RolPermiso { RolId = recepcionista, PermisoId = 6 }, // pacientes.escribir
            new RolPermiso { RolId = recepcionista, PermisoId = 7 }, // pacientes.leer
            new RolPermiso { RolId = recepcionista, PermisoId = 8 }, // citas.escribir
            new RolPermiso { RolId = recepcionista, PermisoId = 9 }); // citas.leer
    }
}
