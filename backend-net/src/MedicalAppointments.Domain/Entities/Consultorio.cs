namespace MedicalAppointments.Domain.Entities;

public class Consultorio
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public ICollection<HorarioAtencion> HorariosAtencion { get; set; } = new List<HorarioAtencion>();
}
