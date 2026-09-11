using FluentAssertions;
using MedicalAppointments.Application.Features.Medicos.Queries;
using MedicalAppointments.Application.Tests.Common;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Tests.Features.Medicos;

public class GetMedicosQueryHandlerTests
{
    [Fact]
    public async Task Handle_ConVariosMedicos_RetornaElMedicosPagedResponseCorrectoParaLaPaginaSolicitada()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        // Se usa el mismo prefijo "Dr." en los tres nombres para evitar
        // ambiguedad en el orden alfabetico: "Dr." vs "Dra." no ordenan
        // como cabria esperar bajo comparacion ordinal ('.' < 'a').
        var nombres = new[] { "Dr. Andres Vega", "Dr. Beatriz Rios", "Dr. Cesar Moran" };

        var contador = 1;
        foreach (var nombre in nombres)
        {
            context.Medicos.Add(new Medico
            {
                NombreCompleto = nombre,
                DocumentoIdentidad = $"400000000{contador}",
                Especialidad = "Medicina General",
                NumeroColegiado = $"MED-{1000 + contador}",
                Telefono = "+593999000000",
                Email = $"medico{contador}@clinica.com"
            });
            contador++;
        }

        await context.SaveChangesAsync();

        var query = new GetMedicosQuery(PageNumber: 1, PageSize: 2, SearchQuery: null);
        var handler = new GetMedicosQueryHandler(context);

        // Act
        var resultado = await handler.Handle(query, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.PageNumber.Should().Be(1);
        resultado.PageSize.Should().Be(2);
        resultado.TotalCount.Should().Be(3);
        resultado.TotalPages.Should().Be(2);
        resultado.Data.Should().HaveCount(2);
        resultado.Data.Select(m => m.NombreCompleto).Should().ContainInOrder("Dr. Andres Vega", "Dr. Beatriz Rios");
    }
}
