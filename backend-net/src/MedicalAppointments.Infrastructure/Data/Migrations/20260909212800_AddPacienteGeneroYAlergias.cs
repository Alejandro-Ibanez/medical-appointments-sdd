using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPacienteGeneroYAlergias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alergias",
                table: "Pacientes",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Pacientes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Alergias", "Genero" },
                values: new object[] { "Alergia a la Penicilina", "Femenino" });

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "Alergias", "Genero" },
                values: new object[] { "", "Masculino" });

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "Alergias", "Genero" },
                values: new object[] { "Alergia al Latex", "Femenino" });

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "Alergias", "Genero" },
                values: new object[] { "", "Masculino" });

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "Alergias", "Genero" },
                values: new object[] { "", "Otro" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alergias",
                table: "Pacientes");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Pacientes");
        }
    }
}
