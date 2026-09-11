using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicosYConsultoriosCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Consultorios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ubicacion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultorios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HorariosAtencion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MedicoId = table.Column<long>(type: "INTEGER", nullable: false),
                    ConsultorioId = table.Column<long>(type: "INTEGER", nullable: false),
                    DiaSemana = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    HoraFin = table.Column<TimeOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosAtencion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosAtencion_Consultorios_ConsultorioId",
                        column: x => x.ConsultorioId,
                        principalTable: "Consultorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HorariosAtencion_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Consultorios",
                columns: new[] { "Id", "Activo", "Nombre", "Ubicacion" },
                values: new object[,]
                {
                    { 1L, true, "Consultorio 101", "Piso 1 - Ala Norte" },
                    { 2L, true, "Consultorio 102", "Piso 1 - Ala Sur" },
                    { 3L, true, "Consultorio 201", "Piso 2 - Ala Norte" }
                });

            migrationBuilder.InsertData(
                table: "HorariosAtencion",
                columns: new[] { "Id", "ConsultorioId", "DiaSemana", "HoraFin", "HoraInicio", "MedicoId" },
                values: new object[,]
                {
                    { 1L, 1L, "Lunes", new TimeOnly(12, 0, 0), new TimeOnly(8, 0, 0), 1L },
                    { 2L, 1L, "Miercoles", new TimeOnly(18, 0, 0), new TimeOnly(14, 0, 0), 1L },
                    { 3L, 2L, "Martes", new TimeOnly(13, 0, 0), new TimeOnly(9, 0, 0), 2L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultorios_Nombre",
                table: "Consultorios",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAtencion_ConsultorioId",
                table: "HorariosAtencion",
                column: "ConsultorioId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAtencion_MedicoId",
                table: "HorariosAtencion",
                column: "MedicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorariosAtencion");

            migrationBuilder.DropTable(
                name: "Consultorios");
        }
    }
}
