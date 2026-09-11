using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedCitasCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 30, new DateTime(2026, 9, 7, 9, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 30, new DateTime(2026, 9, 14, 10, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 45, new DateTime(2026, 9, 8, 9, 30, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 30, new DateTime(2026, 9, 15, 10, 30, 0, 0, DateTimeKind.Unspecified), "Cancelada por el paciente" });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 30, new DateTime(2026, 9, 9, 15, 0, 0, 0, DateTimeKind.Unspecified), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 0, new DateTime(2026, 9, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 0, new DateTime(2026, 9, 10, 10, 30, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 0, new DateTime(2026, 9, 15, 14, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 0, new DateTime(2026, 9, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Citas",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "DuracionMinutos", "FechaHora", "MotivoCancelacion" },
                values: new object[] { 0, new DateTime(2026, 9, 25, 16, 0, 0, 0, DateTimeKind.Unspecified), null });
        }
    }
}
