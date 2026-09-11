using DomainCita = MedicalAppointments.Domain.Entities.Cita;
using DomainHorario = MedicalAppointments.Domain.Entities.HorarioAtencion;
using DiaSemana = MedicalAppointments.Domain.Enums.DiaSemana;

namespace MedicalAppointments.Application.Common;

/// <summary>
/// Resuelve, a partir de la fecha/hora y duracion de una cita, a que dia de
/// la semana (enum DiaSemana) y a que HorarioAtencion (y por lo tanto a que
/// Consultorio) corresponde esa cita segun la agenda del medico.
/// </summary>
public static class HorarioResolver
{
    public static DiaSemana? MapearDiaSemana(DayOfWeek diaSemana) => diaSemana switch
    {
        DayOfWeek.Monday => DiaSemana.Lunes,
        DayOfWeek.Tuesday => DiaSemana.Martes,
        DayOfWeek.Wednesday => DiaSemana.Miercoles,
        DayOfWeek.Thursday => DiaSemana.Jueves,
        DayOfWeek.Friday => DiaSemana.Viernes,
        DayOfWeek.Saturday => DiaSemana.Sabado,
        _ => null
    };

    /// <summary>
    /// Encuentra el HorarioAtencion del medico de la cita que cubre el dia y
    /// rango horario exacto de esa cita. Requiere que la navegacion
    /// Cita.Medico.HorariosAtencion este cargada.
    /// </summary>
    public static DomainHorario? ResolverHorario(DomainCita cita)
    {
        if (cita.Medico is null)
        {
            return null;
        }

        var dia = MapearDiaSemana(cita.FechaHora.DayOfWeek);
        if (dia is null)
        {
            return null;
        }

        var horaInicio = TimeOnly.FromDateTime(cita.FechaHora);
        var horaFin = horaInicio.AddMinutes(cita.DuracionMinutos);

        return cita.Medico.HorariosAtencion.FirstOrDefault(h =>
            h.DiaSemana == dia.Value &&
            h.HoraInicio <= horaInicio &&
            horaFin <= h.HoraFin);
    }

    public static long? ResolverConsultorioId(DomainCita cita) => ResolverHorario(cita)?.ConsultorioId;
}
