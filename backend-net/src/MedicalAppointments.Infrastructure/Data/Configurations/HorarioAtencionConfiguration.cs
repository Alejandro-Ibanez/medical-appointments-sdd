using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class HorarioAtencionConfiguration : IEntityTypeConfiguration<HorarioAtencion>
{
    public void Configure(EntityTypeBuilder<HorarioAtencion> builder)
    {
        builder.ToTable("HorariosAtencion");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.DiaSemana).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(h => h.Medico)
            .WithMany(m => m.HorariosAtencion)
            .HasForeignKey(h => h.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Consultorio)
            .WithMany(c => c.HorariosAtencion)
            .HasForeignKey(h => h.ConsultorioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new HorarioAtencion
            {
                Id = 1,
                MedicoId = 1,
                ConsultorioId = 1,
                DiaSemana = DiaSemana.Lunes,
                HoraInicio = new TimeOnly(8, 0),
                HoraFin = new TimeOnly(12, 0)
            },
            new HorarioAtencion
            {
                Id = 2,
                MedicoId = 1,
                ConsultorioId = 1,
                DiaSemana = DiaSemana.Miercoles,
                HoraInicio = new TimeOnly(14, 0),
                HoraFin = new TimeOnly(18, 0)
            },
            new HorarioAtencion
            {
                Id = 3,
                MedicoId = 2,
                ConsultorioId = 2,
                DiaSemana = DiaSemana.Martes,
                HoraInicio = new TimeOnly(9, 0),
                HoraFin = new TimeOnly(13, 0)
            });
    }
}
