using DomainCita = MedicalAppointments.Domain.Entities.Cita;

namespace MedicalAppointments.Application.Features.Citas;

internal static class CitaQueryFilters
{
    public static IQueryable<DomainCita> Aplicar(
        IQueryable<DomainCita> query,
        string? palabraClave,
        long? medicoId,
        long? pacienteId,
        DateTime? fechaInicio,
        DateTime? fechaFin)
    {
        if (medicoId.HasValue)
        {
            query = query.Where(c => c.MedicoId == medicoId.Value);
        }

        if (pacienteId.HasValue)
        {
            query = query.Where(c => c.PacienteId == pacienteId.Value);
        }

        if (fechaInicio.HasValue)
        {
            // Desde las 00:00:00 de fechaInicio.
            var inicio = fechaInicio.Value.Date;
            query = query.Where(c => c.FechaHora >= inicio);
        }

        if (fechaFin.HasValue)
        {
            // Hasta las 23:59:59 de fechaFin (inclusive).
            var fin = fechaFin.Value.Date.AddDays(1).AddSeconds(-1);
            query = query.Where(c => c.FechaHora <= fin);
        }

        if (!string.IsNullOrWhiteSpace(palabraClave))
        {
            // ToLower() en ambos lados para que la busqueda sea insensible a mayusculas/minusculas.
            var texto = palabraClave.Trim().ToLower();
            query = query.Where(c =>
                c.MotivoConsulta.ToLower().Contains(texto) ||
                c.Paciente!.NombreCompleto.ToLower().Contains(texto) ||
                c.Paciente!.Alergias.ToLower().Contains(texto) ||
                c.Paciente!.HistorialClinico.Any(h => h.Diagnostico.ToLower().Contains(texto)) ||
                c.Medico!.NombreCompleto.ToLower().Contains(texto));
        }

        return query;
    }
}
