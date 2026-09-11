using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedSeguridadDinamica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1L, "usuarios.administrar", "Administrar Usuarios" },
                    { 2L, "consultorios.administrar", "Administrar Consultorios" },
                    { 3L, "medicos.administrar", "Administrar Medicos" },
                    { 4L, "dashboard.ver", "Ver Dashboard" },
                    { 5L, "reportes.descargar", "Descargar Reportes" },
                    { 6L, "pacientes.escribir", "Registrar y Editar Pacientes" },
                    { 7L, "pacientes.leer", "Consultar Pacientes" },
                    { 8L, "citas.escribir", "Agendar y Cancelar Citas" },
                    { 9L, "citas.leer", "Consultar Citas" },
                    { 10L, "historial.escribir", "Registrar Historial Clinico" },
                    { 11L, "historial.leer", "Consultar Historial Clinico" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1L, "Admin" },
                    { 2L, "Medico" },
                    { 3L, "Recepcionista" }
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "MedicoId", "PasswordHash", "RolId" },
                values: new object[] { null, "$2a$11$hrJzyu02eWYoeV56qp.UHuXAK9qoN2XkIoWD8zpI8/sqi7NCGQB6.", 1L });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "MedicoId", "PasswordHash", "RolId" },
                values: new object[] { 1L, "$2a$11$sycKZcTlxbtS7uFL3MJjI.AKz4/9PrC/tDa0mxxrrH45aN5t8Djiq", 2L });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "MedicoId", "PasswordHash", "RolId" },
                values: new object[] { null, "$2a$11$kq1G3Pc/8aDOaGHWA1lqtuWvYn.hWbV9uTn.G9o.eOXIMVQI93/VO", 3L });

            migrationBuilder.InsertData(
                table: "RolesPermisos",
                columns: new[] { "PermisoId", "RolId" },
                values: new object[,]
                {
                    { 1L, 1L },
                    { 2L, 1L },
                    { 3L, 1L },
                    { 4L, 1L },
                    { 5L, 1L },
                    { 6L, 1L },
                    { 7L, 1L },
                    { 8L, 1L },
                    { 9L, 1L },
                    { 10L, 1L },
                    { 11L, 1L },
                    { 4L, 2L },
                    { 7L, 2L },
                    { 8L, 2L },
                    { 9L, 2L },
                    { 10L, 2L },
                    { 11L, 2L },
                    { 4L, 3L },
                    { 5L, 3L },
                    { 6L, 3L },
                    { 7L, 3L },
                    { 8L, 3L },
                    { 9L, 3L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolesPermisos",
                keyColumns: new[] { "RolId", "PermisoId" },
                keyValues: new object[,]
                {
                    { 1L, 1L },
                    { 1L, 2L },
                    { 1L, 3L },
                    { 1L, 4L },
                    { 1L, 5L },
                    { 1L, 6L },
                    { 1L, 7L },
                    { 1L, 8L },
                    { 1L, 9L },
                    { 1L, 10L },
                    { 1L, 11L },
                    { 2L, 4L },
                    { 2L, 7L },
                    { 2L, 8L },
                    { 2L, 9L },
                    { 2L, 10L },
                    { 2L, 11L },
                    { 3L, 4L },
                    { 3L, 5L },
                    { 3L, 6L },
                    { 3L, 7L },
                    { 3L, 8L },
                    { 3L, 9L }
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "MedicoId", "PasswordHash", "RolId" },
                values: new object[] { null, "AQAAAAIAAYagAAAAEAKQgcKVaH2xZAOI5wU/f1ZC++k3OKtbSA3XuD0uWS0kM0IiuML5KoEOwLHB2EZhNQ==", 0L });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "MedicoId", "PasswordHash", "RolId" },
                values: new object[] { null, "AQAAAAIAAYagAAAAEDGhFYvI3j55bxawo11jop0lzoCeIo+LL2J76DigRLHXJq0lYDyxLnhNdiJRuepIGQ==", 0L });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "MedicoId", "PasswordHash", "RolId" },
                values: new object[] { null, "AQAAAAIAAYagAAAAEPzORbcQhDNnOybrBME+81NbJJJKJQEgLVkG91H1VXRGbxv0eo9UQB8nfjvIDBPMaQ==", 0L });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValues: new object[] { 1L, 2L, 3L });

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValues: new object[] { 1L, 2L, 3L, 4L, 5L, 6L, 7L, 8L, 9L, 10L, 11L });
        }
    }
}
