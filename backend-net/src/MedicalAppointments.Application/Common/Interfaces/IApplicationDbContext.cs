using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MedicalAppointments.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<Paciente> Pacientes { get; }
    DbSet<Medico> Medicos { get; }
    DbSet<Cita> Citas { get; }
    DbSet<HistorialClinico> HistorialesClinicos { get; }
    DbSet<Consultorio> Consultorios { get; }
    DbSet<HorarioAtencion> HorariosAtencion { get; }
    DbSet<Rol> Roles { get; }
    DbSet<Permiso> Permisos { get; }
    DbSet<RolPermiso> RolPermisos { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
