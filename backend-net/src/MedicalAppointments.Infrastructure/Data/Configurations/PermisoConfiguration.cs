using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
{
    public void Configure(EntityTypeBuilder<Permiso> builder)
    {
        builder.ToTable("Permisos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Codigo).IsRequired().HasMaxLength(100);
        builder.HasIndex(p => p.Codigo).IsUnique();

        builder.HasData(
            new Permiso { Id = 1, Nombre = "Administrar Usuarios", Codigo = "usuarios.administrar" },
            new Permiso { Id = 2, Nombre = "Administrar Consultorios", Codigo = "consultorios.administrar" },
            new Permiso { Id = 3, Nombre = "Administrar Medicos", Codigo = "medicos.administrar" },
            new Permiso { Id = 4, Nombre = "Ver Dashboard", Codigo = "dashboard.ver" },
            new Permiso { Id = 5, Nombre = "Descargar Reportes", Codigo = "reportes.descargar" },
            new Permiso { Id = 6, Nombre = "Registrar y Editar Pacientes", Codigo = "pacientes.escribir" },
            new Permiso { Id = 7, Nombre = "Consultar Pacientes", Codigo = "pacientes.leer" },
            new Permiso { Id = 8, Nombre = "Agendar y Cancelar Citas", Codigo = "citas.escribir" },
            new Permiso { Id = 9, Nombre = "Consultar Citas", Codigo = "citas.leer" },
            new Permiso { Id = 10, Nombre = "Registrar Historial Clinico", Codigo = "historial.escribir" },
            new Permiso { Id = 11, Nombre = "Consultar Historial Clinico", Codigo = "historial.leer" });
    }
}
