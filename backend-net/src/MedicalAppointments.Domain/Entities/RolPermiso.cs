namespace MedicalAppointments.Domain.Entities;

public class RolPermiso
{
    public long RolId { get; set; }
    public Rol? Rol { get; set; }

    public long PermisoId { get; set; }
    public Permiso? Permiso { get; set; }
}
