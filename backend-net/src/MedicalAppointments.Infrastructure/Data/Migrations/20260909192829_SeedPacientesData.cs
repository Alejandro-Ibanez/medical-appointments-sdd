using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedPacientesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Pacientes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Activo",
                value: true);

            migrationBuilder.UpdateData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Activo",
                value: true);

            migrationBuilder.InsertData(
                table: "Pacientes",
                columns: new[] { "Id", "Activo", "Direccion", "DocumentoIdentidad", "Email", "FechaNacimiento", "NombreCompleto", "Telefono" },
                values: new object[,]
                {
                    { 3L, true, "Urbanizacion Las Acacias, Mz 4 Villa 12", "0304050607", "ana.herrera@correo.com", new DateTime(1998, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ana Sofia Herrera", "+593987011223" },
                    { 4L, false, "Cdla. La Alborada, Etapa 3", "0405060708", "roberto.paredes@correo.com", new DateTime(1972, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Roberto Paredes", "+593987223344" },
                    { 5L, true, "Av. de las Americas y Rio Amazonas", "0506070809", "lucia.mendoza@correo.com", new DateTime(2005, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lucia Mendoza", "+593987445566" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Pacientes",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Pacientes");
        }
    }
}
