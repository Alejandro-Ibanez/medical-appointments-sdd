using FluentAssertions;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Medicos.Commands;
using MedicalAppointments.Application.Tests.Common;
using MedicalAppointments.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using DomainEnums = MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Application.Tests.Features.Medicos;

public class AsignarHorarioCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConMedicoYConsultorioDisponibles_AsignaElHorarioCorrectamente()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dra. Lucia Fernandez",
            DocumentoIdentidad = "3030303030",
            Especialidad = "Pediatria",
            NumeroColegiado = "MED-00111",
            Telefono = "+593999555666",
            Email = "lucia.fernandez@clinica.com"
        };

        var consultorio = new MedicalAppointments.Domain.Entities.Consultorio
        {
            Nombre = "Consultorio 202",
            Ubicacion = "Piso 2"
        };

        context.Medicos.Add(medico);
        context.Consultorios.Add(consultorio);
        await context.SaveChangesAsync();

        var command = new AsignarHorarioCommand(
            medico.Id,
            DiaSemana.Lunes,
            new TimeOnly(8, 0),
            new TimeOnly(12, 0),
            consultorio.Id);

        var handler = new AsignarHorarioCommandHandler(context);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.MedicoId.Should().Be(medico.Id);
        resultado.DiaSemana.Should().Be(DiaSemana.Lunes);
        resultado.HoraInicio.Should().Be("08:00");
        resultado.HoraFin.Should().Be("12:00");
        resultado.Consultorio.Id.Should().Be(consultorio.Id);

        var horarioGuardado = await context.HorariosAtencion.SingleAsync();
        horarioGuardado.MedicoId.Should().Be(medico.Id);
        horarioGuardado.ConsultorioId.Should().Be(consultorio.Id);
        horarioGuardado.DiaSemana.Should().Be(DomainEnums.DiaSemana.Lunes);
        horarioGuardado.HoraInicio.Should().Be(new TimeOnly(8, 0));
        horarioGuardado.HoraFin.Should().Be(new TimeOnly(12, 0));
    }

    [Fact]
    public async Task Handle_CuandoElMedicoYaTieneUnHorarioSolapadoEseDia_LanzaBusinessRuleException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var medico = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Ricardo Leon",
            DocumentoIdentidad = "1212121212",
            Especialidad = "Neurologia",
            NumeroColegiado = "MED-00666",
            Telefono = "+593999111000",
            Email = "ricardo.leon@clinica.com"
        };

        var consultorio1 = new MedicalAppointments.Domain.Entities.Consultorio { Nombre = "Consultorio 601", Ubicacion = "Piso 6" };
        var consultorio2 = new MedicalAppointments.Domain.Entities.Consultorio { Nombre = "Consultorio 602", Ubicacion = "Piso 6" };

        context.Medicos.Add(medico);
        context.Consultorios.AddRange(consultorio1, consultorio2);
        await context.SaveChangesAsync();

        // El medico ya atiende los Lunes de 08:00 a 12:00 en el consultorio 1.
        context.HorariosAtencion.Add(new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            MedicoId = medico.Id,
            ConsultorioId = consultorio1.Id,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        });
        await context.SaveChangesAsync();

        // Se intenta asignarle, el mismo Lunes, un bloque que se solapa (10:00-14:00),
        // aunque sea en un consultorio fisico distinto.
        var command = new AsignarHorarioCommand(
            medico.Id,
            DiaSemana.Lunes,
            new TimeOnly(10, 0),
            new TimeOnly(14, 0),
            consultorio2.Id);

        var handler = new AsignarHorarioCommandHandler(context);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("El medico ya tiene un horario asignado que se solapa con el rango indicado para ese dia.");

        (await context.HorariosAtencion.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_CuandoElConsultorioYaEstaReservadoPorOtroMedico_LanzaBusinessRuleException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var medicoA = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Fernando Aguilar",
            DocumentoIdentidad = "1313131313",
            Especialidad = "Cardiologia",
            NumeroColegiado = "MED-00777",
            Telefono = "+593999222111",
            Email = "fernando.aguilar@clinica.com"
        };

        var medicoB = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dra. Gabriela Mora",
            DocumentoIdentidad = "1414141414",
            Especialidad = "Ginecologia",
            NumeroColegiado = "MED-00888",
            Telefono = "+593999333222",
            Email = "gabriela.mora@clinica.com"
        };

        var consultorioCompartido = new MedicalAppointments.Domain.Entities.Consultorio
        {
            Nombre = "Consultorio 701",
            Ubicacion = "Piso 7"
        };

        context.Medicos.AddRange(medicoA, medicoB);
        context.Consultorios.Add(consultorioCompartido);
        await context.SaveChangesAsync();

        // El medicoA ya tiene reservado el consultorio compartido los Lunes de 08:00 a 12:00.
        context.HorariosAtencion.Add(new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            MedicoId = medicoA.Id,
            ConsultorioId = consultorioCompartido.Id,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        });
        await context.SaveChangesAsync();

        // Se intenta asignarle al medicoB el mismo consultorio, mismo dia, en un
        // rango que se solapa (10:00-11:00) con el del medicoA.
        var command = new AsignarHorarioCommand(
            medicoB.Id,
            DiaSemana.Lunes,
            new TimeOnly(10, 0),
            new TimeOnly(11, 0),
            consultorioCompartido.Id);

        var handler = new AsignarHorarioCommandHandler(context);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("El consultorio ya esta reservado por otro medico en ese mismo dia y rango horario.");

        (await context.HorariosAtencion.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_ConBloquesContinuosEnElMismoConsultorio_AsignaElSegundoHorarioSinMarcarCruce()
    {
        // Arrange: al Dr. Alan se le asigna el Consultorio 101 de 08:00 a
        // 12:00. Luego se intenta asignar a la Dra. Elena el mismo
        // consultorio de 12:00 a 16:00 el mismo dia: la entrega del
        // espacio fisico es inmediata, asi que no debe marcarse como cruce.
        await using var context = TestDbContextFactory.Create();

        var drAlan = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dr. Alan Mendez",
            DocumentoIdentidad = "2323232323",
            Especialidad = "Medicina General",
            NumeroColegiado = "MED-01001",
            Telefono = "+593999777001",
            Email = "alan.mendez@clinica.com"
        };

        var draElena = new MedicalAppointments.Domain.Entities.Medico
        {
            NombreCompleto = "Dra. Elena Campos",
            DocumentoIdentidad = "2424242424",
            Especialidad = "Pediatria",
            NumeroColegiado = "MED-01002",
            Telefono = "+593999777002",
            Email = "elena.campos@clinica.com"
        };

        var consultorio101 = new MedicalAppointments.Domain.Entities.Consultorio { Nombre = "Consultorio 101", Ubicacion = "Piso 1" };

        context.Medicos.AddRange(drAlan, draElena);
        context.Consultorios.Add(consultorio101);
        await context.SaveChangesAsync();

        context.HorariosAtencion.Add(new MedicalAppointments.Domain.Entities.HorarioAtencion
        {
            MedicoId = drAlan.Id,
            ConsultorioId = consultorio101.Id,
            DiaSemana = DomainEnums.DiaSemana.Lunes,
            HoraInicio = new TimeOnly(8, 0),
            HoraFin = new TimeOnly(12, 0)
        });
        await context.SaveChangesAsync();

        var command = new AsignarHorarioCommand(
            draElena.Id,
            DiaSemana.Lunes,
            new TimeOnly(12, 0),
            new TimeOnly(16, 0),
            consultorio101.Id);

        var handler = new AsignarHorarioCommandHandler(context);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.MedicoId.Should().Be(draElena.Id);
        resultado.HoraInicio.Should().Be("12:00");
        resultado.HoraFin.Should().Be("16:00");
        resultado.Consultorio.Id.Should().Be(consultorio101.Id);

        (await context.HorariosAtencion.CountAsync()).Should().Be(2);
    }
}
