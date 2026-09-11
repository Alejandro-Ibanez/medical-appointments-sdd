using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Domain.Entities;

public class Paciente
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public GeneroPaciente Genero { get; set; } = GeneroPaciente.Otro;
    public string Alergias { get; set; } = string.Empty;

    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<HistorialClinico> HistorialClinico { get; set; } = new List<HistorialClinico>();
}
