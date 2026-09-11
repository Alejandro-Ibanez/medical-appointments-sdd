using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedHistorialClinicoCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "HistorialesClinicos",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "FechaHora", "MedicoNombre" },
                values: new object[] { new DateTime(2026, 7, 10, 9, 0, 0, 0, DateTimeKind.Unspecified), "Dr. Carlos Ramirez" });

            migrationBuilder.UpdateData(
                table: "HistorialesClinicos",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Diagnostico", "FechaHora", "MedicoNombre", "Notas", "PacienteId", "Tratamiento" },
                values: new object[] { "Hipertension arterial leve", new DateTime(2026, 8, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), "Dr. Carlos Ramirez", "Presion arterial registrada: 135/85 mmHg.", 1L, "Dieta baja en sodio y control en 3 meses" });

            migrationBuilder.InsertData(
                table: "HistorialesClinicos",
                columns: new[] { "Id", "Diagnostico", "FechaHora", "MedicoNombre", "Notas", "PacienteId", "Tratamiento" },
                values: new object[,]
                {
                    { 3L, "Control post consulta cardiologica", new DateTime(2026, 9, 5, 16, 30, 0, 0, DateTimeKind.Unspecified), "Dra. Lucia Fernandez", "Paciente estable, sin nuevos sintomas reportados.", 1L, "Continuar tratamiento habitual" },
                    { 4L, "Rinitis alergica estacional", new DateTime(2026, 8, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), "Dra. Lucia Fernandez", "Paciente indica mejoria con tratamientos similares previos.", 2L, "Loratadina 10mg cada 24 horas por 7 dias" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HistorialesClinicos",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "HistorialesClinicos",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.UpdateData(
                table: "HistorialesClinicos",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "FechaHora", "MedicoNombre" },
                values: new object[] { new DateTime(2026, 8, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), "" });

            migrationBuilder.UpdateData(
                table: "HistorialesClinicos",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Diagnostico", "FechaHora", "MedicoNombre", "Notas", "PacienteId", "Tratamiento" },
                values: new object[] { "Rinitis alergica estacional", new DateTime(2026, 8, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), "", "Paciente indica mejoria con tratamientos similares previos.", 2L, "Loratadina 10mg cada 24 horas por 7 dias" });
        }
    }
}
