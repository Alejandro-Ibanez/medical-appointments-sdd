using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalAppointments.Infrastructure.Data.Configurations;

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Pacientes");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.NombreCompleto).IsRequired().HasMaxLength(200);
        builder.Property(p => p.DocumentoIdentidad).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Telefono).HasMaxLength(30);
        builder.Property(p => p.Email).HasMaxLength(256);
        builder.Property(p => p.Direccion).HasMaxLength(300);
        builder.Property(p => p.Genero).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Alergias).HasMaxLength(1000);

        builder.HasIndex(p => p.DocumentoIdentidad).IsUnique();

        builder.HasData(
            new Paciente
            {
                Id = 1,
                NombreCompleto = "Maria Gonzalez",
                DocumentoIdentidad = "0102030405",
                FechaNacimiento = new DateTime(1990, 5, 15),
                Telefono = "+593987654321",
                Email = "maria.gonzalez@correo.com",
                Direccion = "Av. Siempre Viva 123",
                Activo = true,
                Genero = GeneroPaciente.Femenino,
                Alergias = "Alergia a la Penicilina"
            },
            new Paciente
            {
                Id = 2,
                NombreCompleto = "Jose Martinez",
                DocumentoIdentidad = "0203040506",
                FechaNacimiento = new DateTime(1985, 11, 20),
                Telefono = "+593987654344",
                Email = "jose.martinez@correo.com",
                Direccion = "Calle Los Pinos 456",
                Activo = true,
                Genero = GeneroPaciente.Masculino,
                Alergias = string.Empty
            },
            new Paciente
            {
                Id = 3,
                NombreCompleto = "Ana Sofia Herrera",
                DocumentoIdentidad = "0304050607",
                FechaNacimiento = new DateTime(1998, 2, 8),
                Telefono = "+593987011223",
                Email = "ana.herrera@correo.com",
                Direccion = "Urbanizacion Las Acacias, Mz 4 Villa 12",
                Activo = true,
                Genero = GeneroPaciente.Femenino,
                Alergias = "Alergia al Latex"
            },
            new Paciente
            {
                Id = 4,
                NombreCompleto = "Roberto Paredes",
                DocumentoIdentidad = "0405060708",
                FechaNacimiento = new DateTime(1972, 9, 30),
                Telefono = "+593987223344",
                Email = "roberto.paredes@correo.com",
                Direccion = "Cdla. La Alborada, Etapa 3",
                Activo = false,
                Genero = GeneroPaciente.Masculino,
                Alergias = string.Empty
            },
            new Paciente
            {
                Id = 5,
                NombreCompleto = "Lucia Mendoza",
                DocumentoIdentidad = "0506070809",
                FechaNacimiento = new DateTime(2005, 7, 22),
                Telefono = "+593987445566",
                Email = "lucia.mendoza@correo.com",
                Direccion = "Av. de las Americas y Rio Amazonas",
                Activo = true,
                Genero = GeneroPaciente.Otro,
                Alergias = string.Empty
            });
    }
}
