using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Domain.Entities;

public class HorarioAtencion
{
    public long Id { get; set; }
    public long MedicoId { get; set; }
    public Medico? Medico { get; set; }
    public long ConsultorioId { get; set; }
    public Consultorio? Consultorio { get; set; }
    public DiaSemana DiaSemana { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFin { get; set; }
}
