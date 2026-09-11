namespace MedicalAppointments.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"La entidad \"{entityName}\" con el identificador ({key}) no fue encontrada.")
    {
    }
}
