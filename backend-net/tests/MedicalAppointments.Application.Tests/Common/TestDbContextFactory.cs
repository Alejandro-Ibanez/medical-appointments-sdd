using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Application.Tests.Common;

public static class TestDbContextFactory
{
    /// <summary>
    /// Crea un TestDbContext respaldado por una base de datos InMemory
    /// completamente nueva y aislada (nombre unico por llamada), para que
    /// los tests no compartan estado entre si.
    /// </summary>
    public static TestDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }
}
