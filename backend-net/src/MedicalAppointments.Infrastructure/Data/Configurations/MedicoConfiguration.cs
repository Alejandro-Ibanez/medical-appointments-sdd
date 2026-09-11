using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class MedicoConfiguration : IEntityTypeConfiguration<Medico>
{
    public void Configure(EntityTypeBuilder<Medico> builder)
    {
        builder.ToTable("Medicos");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.NombreCompleto).IsRequired().HasMaxLength(200);
        builder.Property(m => m.DocumentoIdentidad).IsRequired().HasMaxLength(20);
        builder.Property(m => m.Especialidad).IsRequired().HasMaxLength(150);
        builder.Property(m => m.NumeroColegiado).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Telefono).HasMaxLength(30);
        builder.Property(m => m.Email).HasMaxLength(256);

        builder.HasIndex(m => m.NumeroColegiado).IsUnique();
        builder.HasIndex(m => m.DocumentoIdentidad).IsUnique();

        builder.HasData(
            new Medico
            {
                Id = 1,
                NombreCompleto = "Dr. Carlos Ramirez",
                DocumentoIdentidad = "0101010101",
                Especialidad = "Cardiologia",
                NumeroColegiado = "MED-00123",
                Telefono = "+593987654322",
                Email = "carlos.ramirez@clinica.com",
                Activo = true
            },
            new Medico
            {
                Id = 2,
                NombreCompleto = "Dra. Lucia Fernandez",
                DocumentoIdentidad = "0202020202",
                Especialidad = "Pediatria",
                NumeroColegiado = "MED-00456",
                Telefono = "+593987654333",
                Email = "lucia.fernandez@clinica.com",
                Activo = true
            });
    }
}
