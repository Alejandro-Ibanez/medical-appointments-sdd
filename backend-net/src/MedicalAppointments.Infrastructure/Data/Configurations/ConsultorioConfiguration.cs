using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class ConsultorioConfiguration : IEntityTypeConfiguration<Consultorio>
{
    public void Configure(EntityTypeBuilder<Consultorio> builder)
    {
        builder.ToTable("Consultorios");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Ubicacion).IsRequired().HasMaxLength(200);

        builder.HasIndex(c => c.Nombre).IsUnique();

        builder.HasData(
            new Consultorio
            {
                Id = 1,
                Nombre = "Consultorio 101",
                Ubicacion = "Piso 1 - Ala Norte",
                Activo = true
            },
            new Consultorio
            {
                Id = 2,
                Nombre = "Consultorio 102",
                Ubicacion = "Piso 1 - Ala Sur",
                Activo = true
            },
            new Consultorio
            {
                Id = 3,
                Nombre = "Consultorio 201",
                Ubicacion = "Piso 2 - Ala Norte",
                Activo = true
            });
    }
}
