using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using MedicalAppointments.Application.DTOs;
using MedicalAppointments.Application.Features.Usuarios.Commands;
using MedicalAppointments.Application.Tests.Common;
using MedicalAppointments.Domain.Exceptions;
using MedicalAppointments.Infrastructure.Security;
using Microsoft.Extensions.Configuration;

namespace MedicalAppointments.Application.Tests.Features.Usuarios;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ConCredencialesCorrectasYUsuarioActivo_RetornaTokenJwtConPermisosDinamicos()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var passwordHasher = new PasswordHasherService();

        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "ClaveSecretaDePruebasConAlMenos32Caracteres!",
                ["Jwt:Issuer"] = "MedicalAppointments.Tests",
                ["Jwt:Audience"] = "MedicalAppointments.Tests.Client",
                ["Jwt:ExpirationMinutes"] = "60"
            })
            .Build();

        var jwtTokenGenerator = new JwtTokenGenerator(configuracion);

        var rol = new MedicalAppointments.Domain.Entities.Rol { Nombre = "Admin" };

        var permisoUsuarios = new MedicalAppointments.Domain.Entities.Permiso
        {
            Nombre = "Administrar Usuarios",
            Codigo = "usuarios.administrar"
        };

        var permisoCitas = new MedicalAppointments.Domain.Entities.Permiso
        {
            Nombre = "Consultar Citas",
            Codigo = "citas.leer"
        };

        context.Roles.Add(rol);
        context.Permisos.AddRange(permisoUsuarios, permisoCitas);
        await context.SaveChangesAsync();

        context.RolPermisos.AddRange(
            new MedicalAppointments.Domain.Entities.RolPermiso { RolId = rol.Id, PermisoId = permisoUsuarios.Id },
            new MedicalAppointments.Domain.Entities.RolPermiso { RolId = rol.Id, PermisoId = permisoCitas.Id });

        var usuario = new MedicalAppointments.Domain.Entities.Usuario
        {
            Username = "admin.test",
            NombreCompleto = "Administrador de Pruebas",
            Email = "admin.test@clinica.com",
            PasswordHash = passwordHasher.HashPassword("Admin123!"),
            RolId = rol.Id,
            Activo = true
        };

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var command = new LoginCommand("admin.test", "Admin123!");
        var handler = new LoginCommandHandler(context, passwordHasher, jwtTokenGenerator);

        // Act
        var resultado = await handler.Handle(command, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Token.Should().NotBeNullOrWhiteSpace();
        resultado.Username.Should().Be("admin.test");
        resultado.NombreCompleto.Should().Be("Administrador de Pruebas");
        resultado.Rol.Should().Be(RolUsuario.Admin);
        resultado.MedicoId.Should().BeNull();

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(resultado.Token);

        var permisosEnToken = jwt.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        permisosEnToken.Should().HaveCount(2);
        permisosEnToken.Should().Contain("usuarios.administrar");
        permisosEnToken.Should().Contain("citas.leer");
    }

    [Fact]
    public async Task Handle_ConUsuarioQueNoExiste_LanzaUnauthorizedException()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var handler = new LoginCommandHandler(context, new PasswordHasherService(), CrearJwtTokenGenerator());
        var command = new LoginCommand("usuario.inexistente", "CualquierClave123!");

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<UnauthorizedException>();
        excepcion.WithMessage("Usuario o contrasena incorrectos.");
    }

    [Fact]
    public async Task Handle_ConContrasenaIncorrecta_LanzaUnauthorizedException()
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
            PasswordHash = passwordHasher.HashPassword("ClaveCorrecta123!"),
            RolId = rol.Id,
            Activo = true
        };

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, passwordHasher, CrearJwtTokenGenerator());
        var command = new LoginCommand("ana.torres", "ClaveIncorrecta999!");

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<UnauthorizedException>();
        excepcion.WithMessage("Usuario o contrasena incorrectos.");
    }

    [Fact]
    public async Task Handle_ConUsuarioInactivo_RechazaElAccesoAunqueLaClaveSeaCorrecta()
    {
        // Arrange
        await using var context = TestDbContextFactory.Create();

        var passwordHasher = new PasswordHasherService();

        var rol = new MedicalAppointments.Domain.Entities.Rol { Nombre = "Medico" };
        context.Roles.Add(rol);
        await context.SaveChangesAsync();

        var usuario = new MedicalAppointments.Domain.Entities.Usuario
        {
            Username = "medico.desactivado",
            NombreCompleto = "Dr. Desactivado",
            Email = "medico.desactivado@clinica.com",
            PasswordHash = passwordHasher.HashPassword("ClaveCorrecta123!"),
            RolId = rol.Id,
            Activo = false
        };

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var handler = new LoginCommandHandler(context, passwordHasher, CrearJwtTokenGenerator());
        var command = new LoginCommand("medico.desactivado", "ClaveCorrecta123!");

        // Act
        var accion = () => handler.Handle(command, CancellationToken.None);

        // Assert
        var excepcion = await accion.Should().ThrowAsync<UnauthorizedException>();
        excepcion.WithMessage("Usuario o contrasena incorrectos.");
    }

    private static JwtTokenGenerator CrearJwtTokenGenerator()
    {
        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "ClaveSecretaDePruebasConAlMenos32Caracteres!",
                ["Jwt:Issuer"] = "MedicalAppointments.Tests",
                ["Jwt:Audience"] = "MedicalAppointments.Tests.Client",
                ["Jwt:ExpirationMinutes"] = "60"
            })
            .Build();

        return new JwtTokenGenerator(configuracion);
    }
}
