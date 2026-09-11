using FluentAssertions;
using MedicalAppointments.Application.Features.HistorialClinico.Commands;
using MedicalAppointments.Application.Tests.Common;
using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEnums = MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Application.Tests.Features.HistorialClinico;

public class CrearEntradaHistorialCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConCitaAsociada_GuardaLaEvolucionYCompletaLaCitaAutomaticamente()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var rol = new Rol { Nombre = "Medico" };
        context.Roles.Add(rol);
        await context.SaveChangesAsync();

        var medico = new Medico
        {
            NombreCompleto = "Dr. Carlos Ramirez",
            DocumentoIdentidad = "5050505050",
            Especialidad = "Cardiologia",
            NumeroColegiado = "MED-00777",
            Telefono = "+593999777888",
            Email = "carlos.ramirez@clinica.com"
        };

        var paciente = new Paciente
        {
            NombreCompleto = "Pedro Salazar",
            DocumentoIdentidad = "6060606060",
            FechaNacimiento = new DateTime(1988, 3, 15),
            Telefono = "+593999222333",
            Email = "pedro.salazar@correo.com"
        };

        context.Medicos.Add(medico);
        context.Pacientes.Add(paciente);
        await context.SaveChangesAsync();

        var usuario = new Usuario
        {
            Username = "carlos.ramirez",
            NombreCompleto = medico.NombreCompleto,
            Email = medico.Email,
            PasswordHash = "hash-irrelevante-para-este-test",
            RolId = rol.Id,
            MedicoId = medico.Id
        };

        var cita = new Cita
        {
            PacienteId = paciente.Id,
            MedicoId = medico.Id,
            FechaHora = DateTime.Now.AddDays(-1),
            DuracionMinutos = 30,
            Estado = DomainEnums.EstadoCita.Programada,
            MotivoConsulta = "Control de rutina"
        };

        context.Usuarios.Add(usuario);
        context.Citas.Add(cita);
        await context.SaveChangesAsync();

        var command = new CrearEntradaHistorialCommand(
            paciente.Id,
            usuario.Id,
            "Paciente estable, sin hallazgos relevantes.",
            "Continuar tratamiento actual.",
            "Proxima revision en 6 meses.",
            cita.Id);

        var handler = new CrearEntradaHistorialCommandHandler(context);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Diagnostico.Should().Be("Paciente estable, sin hallazgos relevantes.");
        resultado.MedicoNombre.Should().Be(medico.NombreCompleto);

        var entradaGuardada = await context.HistorialesClinicos.SingleAsync();
        entradaGuardada.PacienteId.Should().Be(paciente.Id);
        entradaGuardada.Diagnostico.Should().Be("Paciente estable, sin hallazgos relevantes.");

        var citaActualizada = await context.Citas.SingleAsync(c => c.Id == cita.Id);
        citaActualizada.Estado.Should().Be(DomainEnums.EstadoCita.Completada);
    }
}
