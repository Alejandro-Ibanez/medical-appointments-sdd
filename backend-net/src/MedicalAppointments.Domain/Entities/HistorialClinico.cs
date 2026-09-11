namespace MedicalAppointments.Domain.Entities;

public class HistorialClinico
{
    public long Id { get; set; }
    public long PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
    public string MedicoNombre { get; set; } = string.Empty;
    public string Diagnostico { get; set; } = string.Empty;
    public string Tratamiento { get; set; } = string.Empty;
    public string Notas { get; set; } = string.Empty;
}
