using MedicalAppointments.Application.Common.Interfaces;

namespace MedicalAppointments.Infrastructure.Security;

/// <summary>
/// Hashing de contrasenas basado en BCrypt, el mismo algoritmo usado para
/// generar los hashes de la siembra de datos.
/// </summary>
public class PasswordHasherService : IPasswordHasher
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string password, string passwordHash) =>
        BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
