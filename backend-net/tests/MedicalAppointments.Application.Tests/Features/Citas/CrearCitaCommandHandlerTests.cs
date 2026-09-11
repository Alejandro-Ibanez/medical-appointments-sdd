using FluentAssertions;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Citas.Commands;
using MedicalAppointments.Application.Tests.Common;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainEnums = MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Application.Tests.Features.Citas;

public class CrearCitaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConDatosValidos_RegistraLaCitaEnEstadoProgramada()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var paciente = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Maria Lopez",
            DocumentoIdentidad = "1010101010",
            FechaNacimiento = new DateTime(1990, 5, 20),
            Telefono = "+593999111222",
            Email = "maria.lopez@correo.com"
        };

        var consultorio = new MedicalAppointments.Domain.Entities.Consultorio
        {
            Nombre = "Consultorio 101",
            Ubicacion = "Piso 1"
        };

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Juan Perez",
            DocumentoIdentidad = "2020202020",
            Especialidad = "Medicina General",
            NumeroColegiado = "MED-00999",
            Telefono = "+593999333444",
            Email = "juan.perez@clinica.com"
        };

        var horario = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medico,
            Consultorio = consultorio,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        context.Pacientes.Add(paciente);
        context.Consultorios.Add(consultorio);
        context.Medicos.Add(medico);
        context.HorariosAtencion.Add(horario);
        await context.SaveChangesAsync();

        var fechaHoraCita = FechaTestHelper.ProximaFecha(DayOfWeek.Monday).AddHours(10);

        var command = new CrearCitaCommand(
            paciente.Id,
            medico.Id,
            fechaHoraCita,
            "Consulta general de rutina");

        var handler = new CrearCitaCommandHandler(context);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be(EstadoCita.Programada);
        resultado.MotivoConsulta.Should().Be("Consulta general de rutina");
        resultado.Paciente.Id.Should().Be(paciente.Id);
        resultado.Medico.Id.Should().Be(medico.Id);

        var citaGuardada = await context.Citas.SingleAsync();
        citaGuardada.PacienteId.Should().Be(paciente.Id);
        citaGuardada.MedicoId.Should().Be(medico.Id);
        citaGuardada.Estado.Should().Be(DomainEnums.EstadoCita.Programada);
        citaGuardada.FechaHora.Should().Be(fechaHoraCita);
    }

    [Fact]
    public async Task Handle_ConFechaPasada_LanzaBusinessRuleExceptionDeFormaInmediata()
    {
        // Arrange: el contexto queda vacio a proposito. La validacion de
        // fecha pasada es la primera instruccion del handler, antes de
        // cualquier consulta a la base de datos, asi que debe fallar sin
        // necesidad de que existan paciente ni medico.
        await using var context = TestDbContextFactory.Create();

        var fechaPasada = DateTime.Now.AddDays(-7);
        var command = new CrearCitaCommand(1, 1, fechaPasada, "Consulta de rutina");
        var handler = new CrearCitaCommandHandler(context);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("No es posible agendar una cita en una fecha o deudas pasadas.");

        (await context.Citas.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Handle_CuandoElMedicoNoTieneHorarioEseDia_LanzaBusinessRuleException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var paciente = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Sofia Torres",
            DocumentoIdentidad = "1111111111",
            FechaNacimiento = new DateTime(1992, 4, 10),
            Telefono = "+593999444555",
            Email = "sofia.torres@correo.com"
        };

        var consultorio = new MedicalAppointments.Domain.Entities.Consultorio
        {
            Nombre = "Consultorio 303",
            Ubicacion = "Piso 3"
        };

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Pablo Nunez",
            DocumentoIdentidad = "2222222222",
            Especialidad = "Traumatologia",
            NumeroColegiado = "MED-00222",
            Telefono = "+593999666777",
            Email = "pablo.nunez@clinica.com"
        };

        // El medico solo atiende los Martes, no los Lunes.
        var horario = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medico,
            Consultorio = consultorio,
            DiaSemana = DomainEnums.DiaSemana.Martes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        context.Pacientes.Add(paciente);
        context.Consultorios.Add(consultorio);
        context.Medicos.Add(medico);
        context.HorariosAtencion.Add(horario);
        await context.SaveChangesAsync();

        var fechaHoraCita = FechaTestHelper.ProximaFecha(DayOfWeek.Monday).AddHours(10);
        var command = new CrearCitaCommand(paciente.Id, medico.Id, fechaHoraCita, "Consulta de rutina");
        var handler = new CrearCitaCommandHandler(context);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("La fecha y hora solicitada esta fuera del horario de atencion del medico.");

        (await context.Citas.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Handle_CuandoElMedicoYaTieneOtraCitaSolapada_LanzaBusinessRuleException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var pacienteExistente = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Luis Herrera",
            DocumentoIdentidad = "3333333333",
            FechaNacimiento = new DateTime(1985, 6, 1),
            Telefono = "+593999888999",
            Email = "luis.herrera@correo.com"
        };

        var pacienteNuevo = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Carla Vargas",
            DocumentoIdentidad = "4444444444",
            FechaNacimiento = new DateTime(1993, 8, 12),
            Telefono = "+593999000111",
            Email = "carla.vargas@correo.com"
        };

        var consultorio = new MedicalAppointments.Domain.Entities.Consultorio
        {
            Nombre = "Consultorio 404",
            Ubicacion = "Piso 4"
        };

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dra. Monica Salas",
            DocumentoIdentidad = "5555555555",
            Especialidad = "Dermatologia",
            NumeroColegiado = "MED-00333",
            Telefono = "+593999222888",
            Email = "monica.salas@clinica.com"
        };

        var horario = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medico,
            Consultorio = consultorio,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        context.Pacientes.AddRange(pacienteExistente, pacienteNuevo);
        context.Consultorios.Add(consultorio);
        context.Medicos.Add(medico);
        context.HorariosAtencion.Add(horario);
        await context.SaveChangesAsync();

        var fechaHoraCita = FechaTestHelper.ProximaFecha(DayOfWeek.Monday).AddHours(10);

        // El medico ya tiene una cita activa a las 10:00 con otro paciente.
        context.Citas.Add(new MedicalAppointments.Domain.Entities.Cita
        {
            PacienteId = pacienteExistente.Id,
            MedicoId = medico.Id,
            FechaHora = fechaHoraCita,
            DuracionMinutos = 30,
            Estado = DomainEnums.EstadoCita.Programada,
            MotivoConsulta = "Cita ya agendada"
        });
        await context.SaveChangesAsync();

        // Se intenta agendar otra cita con el mismo medico, mismo bloque horario.
        var command = new CrearCitaCommand(pacienteNuevo.Id, medico.Id, fechaHoraCita, "Nueva consulta");
        var handler = new CrearCitaCommandHandler(context);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("El medico ya tiene otra cita agendada que se solapa con ese mismo horario.");

        (await context.Citas.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_CuandoElConsultorioYaEstaOcupadoPorOtroMedico_LanzaBusinessRuleException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var pacienteExistente = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Diego Ponce",
            DocumentoIdentidad = "6666666666",
            FechaNacimiento = new DateTime(1980, 2, 2),
            Telefono = "+593999333222",
            Email = "diego.ponce@correo.com"
        };

        var pacienteNuevo = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Valeria Ortiz",
            DocumentoIdentidad = "7777777777",
            FechaNacimiento = new DateTime(1991, 11, 23),
            Telefono = "+593999444333",
            Email = "valeria.ortiz@correo.com"
        };

        var consultorioCompartido = new MedicalAppointments.Domain.Entities.Consultorio
        {
            Nombre = "Consultorio 505",
            Ubicacion = "Piso 5"
        };

        var medicoA = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Ivan Castro",
            DocumentoIdentidad = "8888888888",
            Especialidad = "Medicina Interna",
            NumeroColegiado = "MED-00444",
            Telefono = "+593999555444",
            Email = "ivan.castro@clinica.com"
        };

        var medicoB = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dra. Paola Reyes",
            DocumentoIdentidad = "9999999999",
            Especialidad = "Endocrinologia",
            NumeroColegiado = "MED-00555",
            Telefono = "+593999666555",
            Email = "paola.reyes@clinica.com"
        };

        // Ambos medicos, por un error previo de asignacion, quedaron con
        // horarios que apuntan al mismo consultorio fisico el mismo dia.
        var horarioMedicoA = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medicoA,
            Consultorio = consultorioCompartido,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        var horarioMedicoB = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medicoB,
            Consultorio = consultorioCompartido,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        context.Pacientes.AddRange(pacienteExistente, pacienteNuevo);
        context.Consultorios.Add(consultorioCompartido);
        context.Medicos.AddRange(medicoA, medicoB);
        context.HorariosAtencion.AddRange(horarioMedicoA, horarioMedicoB);
        await context.SaveChangesAsync();

        var fechaHoraCita = FechaTestHelper.ProximaFecha(DayOfWeek.Monday).AddHours(10);

        // El medicoB ya tiene una cita activa en el consultorio compartido a esa hora.
        context.Citas.Add(new MedicalAppointments.Domain.Entities.Cita
        {
            PacienteId = pacienteExistente.Id,
            MedicoId = medicoB.Id,
            FechaHora = fechaHoraCita,
            DuracionMinutos = 30,
            Estado = DomainEnums.EstadoCita.Programada,
            MotivoConsulta = "Cita ya agendada con medicoB"
        });
        await context.SaveChangesAsync();

        // Se intenta agendar con el medicoA, mismo consultorio fisico, mismo rango horario.
        var command = new CrearCitaCommand(pacienteNuevo.Id, medicoA.Id, fechaHoraCita, "Nueva consulta");
        var handler = new CrearCitaCommandHandler(context);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("El consultorio asignado a ese horario ya esta ocupado por otra cita en ese mismo rango.");

        (await context.Citas.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_CuandoLaCitaTerminaExactamenteAlCierreDelHorario_RegistraLaCitaSinDesbordar()
    {
        // Arrange: el medico atiende hasta las 12:00. Se agenda a las 11:30
        // con 30 minutos de duracion, terminando justo a las 12:00 (limite
        // inclusive), lo cual debe ser aceptado.
        await using var context = TestDbContextFactory.Create();

        var paciente = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Rosa Delgado",
            DocumentoIdentidad = "1515151515",
            FechaNacimiento = new DateTime(1994, 7, 7),
            Telefono = "+593999123123",
            Email = "rosa.delgado@correo.com"
        };

        var consultorio = new MedicalAppointments.Domain.Entities.Consultorio { Nombre = "Consultorio 801", Ubicacion = "Piso 8" };

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Hugo Vera",
            DocumentoIdentidad = "1616161616",
            Especialidad = "Oftalmologia",
            NumeroColegiado = "MED-00999X",
            Telefono = "+593999456456",
            Email = "hugo.vera@clinica.com"
        };

        var horario = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medico,
            Consultorio = consultorio,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        context.Pacientes.Add(paciente);
        context.Consultorios.Add(consultorio);
        context.Medicos.Add(medico);
        context.HorariosAtencion.Add(horario);
        await context.SaveChangesAsync();

        var fechaHoraCita = FechaTestHelper.ProximaFecha(DayOfWeek.Monday).AddHours(11).AddMinutes(30);
        var command = new CrearCitaCommand(paciente.Id, medico.Id, fechaHoraCita, "Control de cierre exacto");
        var handler = new CrearCitaCommandHandler(context);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be(EstadoCita.Programada);

        (await context.Citas.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_CuandoLaCitaSeDesbordaPorUnMinutoDelCierre_LanzaBusinessRuleException()
    {
        // Arrange: mismo escenario que el caso anterior, pero se agenda un
        // minuto mas tarde (11:31), por lo que la cita terminaria a las
        // 12:01, un minuto despues del cierre del horario (12:00).
        await using var context = TestDbContextFactory.Create();

        var paciente = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Ruben Castillo",
            DocumentoIdentidad = "1717171717",
            FechaNacimiento = new DateTime(1994, 7, 7),
            Telefono = "+593999123124",
            Email = "ruben.castillo@correo.com"
        };

        var consultorio = new MedicalAppointments.Domain.Entities.Consultorio { Nombre = "Consultorio 802", Ubicacion = "Piso 8" };

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dra. Silvia Nunez",
            DocumentoIdentidad = "1818181818",
            Especialidad = "Oftalmologia",
            NumeroColegiado = "MED-00999Y",
            Telefono = "+593999456457",
            Email = "silvia.nunez@clinica.com"
        };

        var horario = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medico,
            Consultorio = consultorio,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        context.Pacientes.Add(paciente);
        context.Consultorios.Add(consultorio);
        context.Medicos.Add(medico);
        context.HorariosAtencion.Add(horario);
        await context.SaveChangesAsync();

        var fechaHoraCita = FechaTestHelper.ProximaFecha(DayOfWeek.Monday).AddHours(11).AddMinutes(31);
        var command = new CrearCitaCommand(paciente.Id, medico.Id, fechaHoraCita, "Control fuera de horario por 1 minuto");
        var handler = new CrearCitaCommandHandler(context);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("La fecha y hora solicitada esta fuera del horario de atencion del medico.");

        (await context.Citas.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Handle_ConCitaInmediatamenteDespuesDeOtraQueTerminaALaMismaHora_RegistraLaCitaSinConflicto()
    {
        // Arrange: el medico ya tiene una cita de 09:00 a 09:30. Se intenta
        // agendar otra de 09:30 a 10:00: los limites se tocan mas no se
        // solapan, por lo que debe permitirse.
        await using var context = TestDbContextFactory.Create();

        var pacienteExistente = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Mateo Salgado",
            DocumentoIdentidad = "1919191919",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Telefono = "+593999111333",
            Email = "mateo.salgado@correo.com"
        };

        var pacienteNuevo = new MedicalAppointments.Domain.Entities.Paciente
        {
            NombreCompleto = "Nadia Rocha",
            DocumentoIdentidad = "2121212121",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Telefono = "+593999111444",
            Email = "nadia.rocha@correo.com"
        };

        var consultorio = new MedicalAppointments.Domain.Entities.Consultorio { Nombre = "Consultorio 803", Ubicacion = "Piso 8" };

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Oscar Peralta",
            DocumentoIdentidad = "2222222223",
            Especialidad = "Medicina General",
            NumeroColegiado = "MED-00999Z",
            Telefono = "+593999456458",
            Email = "oscar.peralta@clinica.com"
        };

        var horario = new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            Medico = medico,
            Consultorio = consultorio,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        };

        context.Pacientes.AddRange(pacienteExistente, pacienteNuevo);
        context.Consultorios.Add(consultorio);
        context.Medicos.Add(medico);
        context.HorariosAtencion.Add(horario);
        await context.SaveChangesAsync();

        var inicioDelDia = FechaTestHelper.ProximaFecha(DayOfWeek.Monday);

        context.Citas.Add(new MedicalAppointments.Domain.Entities.Cita
        {
            PacienteId = pacienteExistente.Id,
            MedicoId = medico.Id,
            FechaHora = inicioDelDia.AddHours(9),
            DuracionMinutos = 30,
            Estado = DomainEnums.EstadoCita.Programada,
            MotivoConsulta = "Cita previa de 09:00 a 09:30"
        });
        await context.SaveChangesAsync();

        var command = new CrearCitaCommand(
            pacienteNuevo.Id,
            medico.Id,
            inicioDelDia.AddHours(9).AddMinutes(30),
            "Cita inmediatamente sucesiva de 09:30 a 10:00");

        var handler = new CrearCitaCommandHandler(context);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Estado.Should().Be(EstadoCita.Programada);

        (await context.Citas.CountAsync()).Should().Be(2);
    }
}
