using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MedicalAppointments.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerarToken(Usuario usuario, IEnumerable<string> permisos)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("No se ha configurado 'Jwt:Secret'.");
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var minutosExpiracion = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var minutos) ? minutos : 60;

        if (usuario.Rol is null)
        {
            throw new InvalidOperationException("El usuario debe tener su Rol cargado para generar el token JWT.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new("unique_name", usuario.Username),
            new("role", usuario.Rol.Nombre),
            new("NombreCompleto", usuario.NombreCompleto),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(permisos.Select(codigo => new Claim("permission", codigo)));

        if (usuario.MedicoId.HasValue)
        {
            claims.Add(new Claim("MedicoId", usuario.MedicoId.Value.ToString()));
        }

        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutosExpiracion),
            signingCredentials: credenciales);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
