using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("Citas");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Estado).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.MotivoConsulta).HasMaxLength(500);
        builder.Property(c => c.MotivoCancelacion).HasMaxLength(500);

        builder.HasOne(c => c.Paciente)
            .WithMany(p => p.Citas)
            .HasForeignKey(c => c.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Medico)
            .WithMany(m => m.Citas)
            .HasForeignKey(c => c.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);

        var fechaCreacionSemilla = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc);

        // Citas de prueba distribuidas en dias de Septiembre 2026 que caen dentro
        // de los HorarioAtencion sembrados: Medico 1 (Lunes 8-12 y Miercoles 14-18,
        // Consultorio 1) y Medico 2 (Martes 9-13, Consultorio 2).
        builder.HasData(
            new Cita
            {
                Id = 1,
                PacienteId = 1,
                MedicoId = 1,
                FechaHora = new DateTime(2026, 9, 7, 9, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Programada,
                MotivoConsulta = "Control anual",
                FechaCreacion = fechaCreacionSemilla
            },
            new Cita
            {
                Id = 2,
                PacienteId = 2,
                MedicoId = 1,
                FechaHora = new DateTime(2026, 9, 14, 10, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Confirmada,
                MotivoConsulta = "Dolor de pecho",
                FechaCreacion = fechaCreacionSemilla
            },
            new Cita
            {
                Id = 3,
                PacienteId = 1,
                MedicoId = 2,
                FechaHora = new DateTime(2026, 9, 8, 9, 30, 0),
                DuracionMinutos = 45,
                Estado = EstadoCita.Programada,
                MotivoConsulta = "Consulta pediatrica",
                FechaCreacion = fechaCreacionSemilla
            },
            new Cita
            {
                Id = 4,
                PacienteId = 2,
                MedicoId = 2,
                FechaHora = new DateTime(2026, 9, 15, 10, 30, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Cancelada,
                MotivoConsulta = "Vacunacion",
                MotivoCancelacion = "Cancelada por el paciente",
                FechaCreacion = fechaCreacionSemilla
            },
            new Cita
            {
                Id = 5,
                PacienteId = 1,
                MedicoId = 1,
                FechaHora = new DateTime(2026, 9, 9, 15, 0, 0),
                DuracionMinutos = 30,
                Estado = EstadoCita.Completada,
                MotivoConsulta = "Seguimiento",
                FechaCreacion = fechaCreacionSemilla
            });
    }
}
