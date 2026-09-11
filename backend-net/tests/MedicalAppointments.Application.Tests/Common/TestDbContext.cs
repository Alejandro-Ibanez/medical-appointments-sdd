using MedicalAppointments.Application.Common.Interfaces;
using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Tests.Common;

/// <summary>
/// DbContext minimo para pruebas unitarias, respaldado por el proveedor
/// InMemory de EF Core. A diferencia del ApplicationDbContext de
/// Infrastructure, no aplica las configuraciones de siembra (HasData) de
/// cada entidad, para que cada test controle exactamente que datos existen
/// en la base de datos antes de ejercitar el handler bajo prueba.
/// </summary>
public class TestDbContext : DbContext, IApplicationDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<HistorialClinico> HistorialesClinicos => Set<HistorialClinico>();
    public DbSet<Consultorio> Consultorios => Set<Consultorio>();
    public DbSet<HorarioAtencion> HorariosAtencion => Set<HorarioAtencion>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RolPermiso>().HasKey(rp => new { rp.RolId, rp.PermisoId });

        base.OnModelCreating(modelBuilder);
    }
}
