using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(200);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Medico)
            .WithMany()
            .HasForeignKey(u => u.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);

        var fechaSemilla = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new Usuario
            {
                Id = 1,
                Username = "admin",
                NombreCompleto = "Administrador General",
                Email = "admin@clinica.com",
                // Hash BCrypt precomputado para "admin123"
                PasswordHash = "$2a$11$hrJzyu02eWYoeV56qp.UHuXAK9qoN2XkIoWD8zpI8/sqi7NCGQB6.",
                RolId = 1, // Admin
                Activo = true,
                FechaCreacion = fechaSemilla
            },
            new Usuario
            {
                Id = 2,
                Username = "medico1",
                NombreCompleto = "Dr. Carlos Ramirez",
                Email = "carlos.ramirez@clinica.com",
                // Hash BCrypt precomputado para "medico123"
                PasswordHash = "$2a$11$sycKZcTlxbtS7uFL3MJjI.AKz4/9PrC/tDa0mxxrrH45aN5t8Djiq",
                RolId = 2, // Medico
                MedicoId = 1, // Vincula con Dr. Carlos Ramirez (Medico Id=1)
                Activo = true,
                FechaCreacion = fechaSemilla
            },
            new Usuario
            {
                Id = 3,
                Username = "recepcionista1",
                NombreCompleto = "Ana Torres",
                Email = "ana.torres@clinica.com",
                // Hash BCrypt precomputado para "recepcionista123"
                PasswordHash = "$2a$11$kq1G3Pc/8aDOaGHWA1lqtuWvYn.hWbV9uTn.G9o.eOXIMVQI93/VO",
                RolId = 3, // Recepcionista
                Activo = true,
                FechaCreacion = fechaSemilla
            });
    }
}
