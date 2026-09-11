using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Especialidad = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    NumeroColegiado = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DocumentoIdentidad = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PacienteId = table.Column<long>(type: "INTEGER", nullable: false),
                    MedicoId = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Citas_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Medicos",
                columns: new[] { "Id", "Activo", "Email", "Especialidad", "NombreCompleto", "NumeroColegiado", "Telefono" },
                values: new object[,]
                {
                    { 1L, true, "carlos.ramirez@clinica.com", "Cardiologia", "Dr. Carlos Ramirez", "MED-00123", "+593987654322" },
                    { 2L, true, "lucia.fernandez@clinica.com", "Pediatria", "Dra. Lucia Fernandez", "MED-00456", "+593987654333" }
                });

            migrationBuilder.InsertData(
                table: "Pacientes",
                columns: new[] { "Id", "Direccion", "DocumentoIdentidad", "Email", "FechaNacimiento", "NombreCompleto", "Telefono" },
                values: new object[,]
                {
                    { 1L, "Av. Siempre Viva 123", "0102030405", "maria.gonzalez@correo.com", new DateTime(1990, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maria Gonzalez", "+593987654321" },
                    { 2L, "Calle Los Pinos 456", "0203040506", "jose.martinez@correo.com", new DateTime(1985, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jose Martinez", "+593987654344" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Email", "FechaCreacion", "NombreCompleto", "PasswordHash", "Rol", "Username" },
                values: new object[,]
                {
                    { 1L, true, "admin@clinica.com", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Administrador General", "AQAAAAIAAYagAAAAEAKQgcKVaH2xZAOI5wU/f1ZC++k3OKtbSA3XuD0uWS0kM0IiuML5KoEOwLHB2EZhNQ==", "Admin", "admin" },
                    { 2L, true, "carlos.ramirez@clinica.com", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dr. Carlos Ramirez", "AQAAAAIAAYagAAAAEDGhFYvI3j55bxawo11jop0lzoCeIo+LL2J76DigRLHXJq0lYDyxLnhNdiJRuepIGQ==", "Medico", "medico1" },
                    { 3L, true, "ana.torres@clinica.com", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ana Torres", "AQAAAAIAAYagAAAAEPzORbcQhDNnOybrBME+81NbJJJKJQEgLVkG91H1VXRGbxv0eo9UQB8nfjvIDBPMaQ==", "Recepcionista", "recepcionista1" }
                });

            migrationBuilder.InsertData(
                table: "Citas",
                columns: new[] { "Id", "Estado", "FechaCreacion", "FechaHora", "MedicoId", "Motivo", "Observaciones", "PacienteId" },
                values: new object[,]
                {
                    { 1L, "Programada", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), 1L, "Control anual", "", 1L },
                    { 2L, "Confirmada", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 10, 10, 30, 0, 0, DateTimeKind.Unspecified), 1L, "Dolor de pecho", "Paciente en ayunas", 2L },
                    { 3L, "Programada", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), 2L, "Consulta pediatrica", "", 1L },
                    { 4L, "Cancelada", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), 2L, "Vacunacion", "Cancelada por el paciente", 2L },
                    { 5L, "Completada", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 25, 16, 0, 0, 0, DateTimeKind.Unspecified), 1L, "Seguimiento", "Evolucion favorable", 1L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoId",
                table: "Citas",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_PacienteId",
                table: "Citas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_NumeroColegiado",
                table: "Medicos",
                column: "NumeroColegiado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_DocumentoIdentidad",
                table: "Pacientes",
                column: "DocumentoIdentidad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                table: "Usuarios",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "Pacientes");
        }
    }
}
