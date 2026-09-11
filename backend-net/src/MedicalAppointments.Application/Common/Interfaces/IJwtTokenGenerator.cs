using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerarToken(Usuario usuario, IEnumerable<string> permisos);
}
