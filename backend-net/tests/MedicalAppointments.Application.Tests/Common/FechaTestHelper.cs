namespace MedicalAppointments.Application.Tests.Common;

public static class FechaTestHelper
{
    /// <summary>
    /// Calcula la proxima fecha (estrictamente futura, nunca hoy) que caiga
    /// en el dia de la semana indicado, para poder construir citas de
    /// prueba que siempre superen la validacion de "fecha no pasada" sin
    /// importar en que dia se ejecuten los tests.
    /// </summary>
    public static DateTime ProximaFecha(DayOfWeek diaSemana)
    {
        var hoy = DateTime.Now.Date;
        var diasHastaSiguiente = ((int)diaSemana - (int)hoy.DayOfWeek + 7) % 7;
        diasHastaSiguiente = diasHastaSiguiente == 0 ? 7 : diasHastaSiguiente;

        return hoy.AddDays(diasHastaSiguiente);
    }
}
