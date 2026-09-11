namespace MedicalAppointments.Domain.Entities;

public class Medico
{
    public long Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string NumeroColegiado { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    public ICollection<HorarioAtencion> HorariosAtencion { get; set; } = new List<HorarioAtencion>();
}
