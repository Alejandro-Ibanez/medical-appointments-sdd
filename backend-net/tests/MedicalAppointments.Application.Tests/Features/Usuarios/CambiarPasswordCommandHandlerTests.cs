using FluentAssertions;
using MedicalAppointments.Application.Features.Usuarios.Commands;
using MedicalAppointments.Application.Tests.Common;
using MedicalAppointments.Domain.Exceptions;
using MedicalAppointments.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Tests.Features.Usuarios;

public class CambiarPasswordCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConPasswordActualQueNoCoincide_LanzaBusinessRuleException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var passwordHasher = new PasswordHasherService();

        var rol = new MedicalAppointments.Domain.Entities.Rol { Nombre = "Recepcionista" };
        context.Roles.Add(rol);
        await context.SaveChangesAsync();

        var usuario = new MedicalAppointments.Domain.Entities.Usuario
        {
            Username = "ana.torres",
            NombreCompleto = "Ana Torres",
            Email = "ana.torres@clinica.com",
            PasswordHash = passwordHasher.HashPassword("ClaveOriginal123!"),
            RolId = rol.Id,
            Activo = true
        };

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var command = new CambiarPasswordCommand(usuario.Id, "ClaveActualIncorrecta999!", "ClaveNueva456!");
        var handler = new CambiarPasswordCommandHandler(context, passwordHasher);

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<BusinessRuleException>();
        excepcion.WithMessage("La contrasena actual no es correcta.");

        // El hash almacenado no debe haber cambiado.
        var usuarioSinModificar = await context.Usuarios.SingleAsync(u => u.Id == usuario.Id);
        passwordHasher.VerifyPassword("ClaveOriginal123!", usuarioSinModificar.PasswordHash).Should().BeTrue();
    }
}
