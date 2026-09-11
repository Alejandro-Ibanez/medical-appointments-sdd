namespace MedicalAppointments.Domain.Entities;

public class Usuario
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public long RolId { get; set; }
    public Rol? Rol { get; set; }

    /// <summary>
    /// Vincula esta cuenta de Usuario con su registro de Medico (solo
    /// aplica cuando el Rol asociado es "Medico"), para poder resolver a
    /// partir del token JWT a que medico corresponde el usuario autenticado.
    /// </summary>
    public long? MedicoId { get; set; }
    public Medico? Medico { get; set; }
}
