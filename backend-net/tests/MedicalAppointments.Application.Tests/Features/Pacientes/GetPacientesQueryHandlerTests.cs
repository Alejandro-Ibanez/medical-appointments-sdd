using FluentAssertions;
using MedicalAppointments.Application.Features.Pacientes.Queries;
using MedicalAppointments.Application.Tests.Common;
using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Tests.Features.Pacientes;

public class GetPacientesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ConVariosPacientes_RetornaElPagedResponseCorrectoParaLaPaginaSolicitada()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var nombres = new[] { "Ana Aguirre", "Carlos Castro", "Elena Espinoza", "Gabriel Guzman", "Ines Ibarra" };

        foreach (var nombre in nombres)
        {
            context.Pacientes.Add(new Paciente
            {
                NombreCompleto = nombre,
                DocumentoIdentidad = Guid.NewGuid().ToString("N")[..10],
                FechaNacimiento = new DateTime(1995, 1, 1),
                Telefono = "+593999000000",
                Email = $"{nombre.Split(' ')[0].ToLower()}@correo.com"
            });
        }

        await context.SaveChangesAsync();

        var query = new GetPacientesQuery(PageNumber: 2, PageSize: 2, SearchQuery: null);
        var handler = new GetPacientesQueryHandler(context);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PageNumber.Should().Be(2);
        resultado.PageSize.Should().Be(2);
        resultado.TotalCount.Should().Be(5);
        resultado.TotalPages.Should().Be(3);
        resultado.Data.Should().HaveCount(2);
        resultado.Data.Select(p => p.NombreCompleto).Should().ContainInOrder("Elena Espinoza", "Gabriel Guzman");
    }

    [Fact]
    public async Task Handle_ConPaginaSolicitadaMasAllaDelTotalDeRegistros_RetornaDataVaciaConMetadataCorrecta()
    {
        // Arrange: 15 pacientes en total, se solicita la pagina 5 con
        // tamano de pagina 10 (solo existen 2 paginas). No debe romper, y
        // la metadata debe seguir reflejando el total real.
        await using var context = TestDbContextFactory.Create();

        for (var i = 1; i <= 15; i++)
        {
            context.Pacientes.Add(new Paciente
            {
                NombreCompleto = $"Paciente Numero {i:D2}",
                DocumentoIdentidad = Guid.NewGuid().ToString("N")[..10],
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "+593999000000",
                Email = $"paciente{i}@correo.com"
            });
        }

        await context.SaveChangesAsync();

        var query = new GetPacientesQuery(PageNumber: 5, PageSize: 10, SearchQuery: null);
        var handler = new GetPacientesQueryHandler(context);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Data.Should().BeEmpty();
        resultado.PageNumber.Should().Be(5);
        resultado.PageSize.Should().Be(10);
        resultado.TotalCount.Should().Be(15);
        resultado.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ConSearchQueryDeSoloEspacios_IgnoraElFiltroYRetornaTodosLosPacientes()
    {
        // Arrange: un SearchQuery compuesto unicamente por espacios en
        // blanco debe tratarse igual que "sin busqueda" (IsNullOrWhiteSpace),
        // sin filtrar ni romper la consulta.
        await using var context = TestDbContextFactory.Create();

        context.Pacientes.AddRange(
            new Paciente
            {
                NombreCompleto = "Fabian Ochoa",
                DocumentoIdentidad = "3131313131",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "+593999000001",
                Email = "fabian.ochoa@correo.com"
            },
            new Paciente
            {
                NombreCompleto = "Gloria Paredes",
                DocumentoIdentidad = "3232323232",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "+593999000002",
                Email = "gloria.paredes@correo.com"
            });

        await context.SaveChangesAsync();

        var query = new GetPacientesQuery(PageNumber: 1, PageSize: 10, SearchQuery: "     ");
        var handler = new GetPacientesQueryHandler(context);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.TotalCount.Should().Be(2);
        resultado.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ConCaracteresDeEscapeDeBaseDeDatosEnElSearchQuery_NoRompeLaConsultaNiRetornaCoincidenciasFalsas()
    {
        // Arrange: un SearchQuery con comodines SQL y un intento de
        // inyeccion debe tratarse como texto literal (Contains simple),
        // sin lanzar excepciones y sin alterar los datos existentes.
        await using var context = TestDbContextFactory.Create();

        context.Pacientes.AddRange(
            new Paciente
            {
                NombreCompleto = "Hector Salinas",
                DocumentoIdentidad = "3333333334",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "+593999000003",
                Email = "hector.salinas@correo.com"
            },
            new Paciente
            {
                NombreCompleto = "Irene Delgado",
                DocumentoIdentidad = "3434343434",
                FechaNacimiento = new DateTime(1990, 1, 1),
                Telefono = "+593999000004",
                Email = "irene.delgado@correo.com"
            });

        await context.SaveChangesAsync();

        var searchQueryMalicioso = "  %'; DROP TABLE Pacientes; --  ";
        var query = new GetPacientesQuery(PageNumber: 1, PageSize: 10, SearchQuery: searchQueryMalicioso);
        var handler = new GetPacientesQueryHandler(context);

        // Act
        var accion = () => handler.Handle(query, CancellationToken.None);

        // Assert
        var resultado = await accion.Should().NotThrowAsync();
        resultado.Subject.TotalCount.Should().Be(0);
        resultado.Subject.Data.Should().BeEmpty();

        // La "inyeccion" nunca se ejecuto como SQL: los pacientes originales siguen intactos.
        (await context.Pacientes.CountAsync()).Should().Be(2);
    }
}
