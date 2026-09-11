using MediatR;
using MedicalAppointments.Application.DTOs;

namespace MedicalAppointments.Application.Features.HistorialClinico.Commands;

/// <param name="UsuarioAutenticadoId">
/// Id del Usuario autenticado (extraido del claim 'sub' del JWT), utilizado
/// para resolver a que Medico corresponde la entrada que se esta creando.
/// </param>
/// <param name="CitaId">
/// Identificador opcional de la cita medica que origino esta entrada del
/// historial clinico. Cuando se recibe, la cita correspondiente se marca
/// automaticamente como 'Completada'.
/// </param>
public record CrearEntradaHistorialCommand(
    long PacienteId,
    long UsuarioAutenticadoId,
    string Diagnostico,
    string? Tratamiento,
    string? Notas,
    long? CitaId) : IRequest<HistorialClinicoResponse>;
