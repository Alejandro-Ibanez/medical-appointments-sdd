using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Domain.Entities;

public class Cita
{
    public long Id { get; set; }
    public long PacienteId { get; set; }
    public Paciente? Paciente { get; set; }
    public long MedicoId { get; set; }
    public Medico? Medico { get; set; }
    public DateTime FechaHora { get; set; }
    public int DuracionMinutos { get; set; } = 30;
    public EstadoCita Estado { get; set; } = EstadoCita.Programada;
    public string MotivoConsulta { get; set; } = string.Empty;
    public string? MotivoCancelacion { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
