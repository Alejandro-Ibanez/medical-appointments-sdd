using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class HistorialClinicoConfiguration : IEntityTypeConfiguration<HistorialClinico>
{
    public void Configure(EntityTypeBuilder<HistorialClinico> builder)
    {
        builder.ToTable("HistorialesClinicos");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.MedicoNombre).IsRequired().HasMaxLength(200);
        builder.Property(h => h.Diagnostico).IsRequired().HasMaxLength(500);
        builder.Property(h => h.Tratamiento).HasMaxLength(1000);
        builder.Property(h => h.Notas).HasMaxLength(2000);

        builder.HasOne(h => h.Paciente)
            .WithMany(p => p.HistorialClinico)
            .HasForeignKey(h => h.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            // Paciente principal de prueba (Id 1, Maria Gonzalez): 3 entradas cronologicas.
            new HistorialClinico
            {
                Id = 1,
                PacienteId = 1,
                FechaHora = new DateTime(2026, 7, 10, 9, 0, 0),
                MedicoNombre = "Dr. Carlos Ramirez",
                Diagnostico = "Chequeo general - sin hallazgos relevantes",
                Tratamiento = "Ninguno",
                Notas = "Se recomienda control anual."
            },
            new HistorialClinico
            {
                Id = 2,
                PacienteId = 1,
                FechaHora = new DateTime(2026, 8, 15, 10, 30, 0),
                MedicoNombre = "Dr. Carlos Ramirez",
                Diagnostico = "Hipertension arterial leve",
                Tratamiento = "Dieta baja en sodio y control en 3 meses",
                Notas = "Presion arterial registrada: 135/85 mmHg."
            },
            new HistorialClinico
            {
                Id = 3,
                PacienteId = 1,
                FechaHora = new DateTime(2026, 9, 5, 16, 30, 0),
                MedicoNombre = "Dra. Lucia Fernandez",
                Diagnostico = "Control post consulta cardiologica",
                Tratamiento = "Continuar tratamiento habitual",
                Notas = "Paciente estable, sin nuevos sintomas reportados."
            },
            // Otro paciente de prueba (Id 2, Jose Martinez) para variedad.
            new HistorialClinico
            {
                Id = 4,
                PacienteId = 2,
                FechaHora = new DateTime(2026, 8, 20, 11, 0, 0),
                MedicoNombre = "Dra. Lucia Fernandez",
                Diagnostico = "Rinitis alergica estacional",
                Tratamiento = "Loratadina 10mg cada 24 horas por 7 dias",
                Notas = "Paciente indica mejoria con tratamientos similares previos."
            });
    }
}
