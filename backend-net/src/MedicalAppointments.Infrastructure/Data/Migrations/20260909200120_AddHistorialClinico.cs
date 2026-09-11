using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHistorialClinico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UsuarioId",
                table: "Medicos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HistorialesClinicos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PacienteId = table.Column<long>(type: "INTEGER", nullable: false),
                    MedicoId = table.Column<long>(type: "INTEGER", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Diagnostico = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Tratamiento = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Notas = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesClinicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesClinicos_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialesClinicos_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "HistorialesClinicos",
                columns: new[] { "Id", "Diagnostico", "FechaHora", "MedicoId", "Notas", "PacienteId", "Tratamiento" },
                values: new object[,]
                {
                    { 1L, "Chequeo general - sin hallazgos relevantes", new DateTime(2026, 8, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), 1L, "Se recomienda control anual.", 1L, "Ninguno" },
                    { 2L, "Rinitis alergica estacional", new DateTime(2026, 8, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), 2L, "Paciente indica mejoria con tratamientos similares previos.", 2L, "Loratadina 10mg cada 24 horas por 7 dias" }
                });

            migrationBuilder.UpdateData(
                table: "Medicos",
                keyColumn: "Id",
                keyValue: 1L,
                column: "UsuarioId",
                value: 2L);

            migrationBuilder.UpdateData(
                table: "Medicos",
                keyColumn: "Id",
                keyValue: 2L,
                column: "UsuarioId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesClinicos_MedicoId",
                table: "HistorialesClinicos",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesClinicos_PacienteId",
                table: "HistorialesClinicos",
                column: "PacienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialesClinicos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Medicos");
        }
    }
}
