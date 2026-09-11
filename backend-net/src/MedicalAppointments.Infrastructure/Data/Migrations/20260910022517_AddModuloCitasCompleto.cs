using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloCitasCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Citas");

            migrationBuilder.RenameColumn(
                name: "Motivo",
                table: "Citas",
                newName: "MotivoConsulta");

            migrationBuilder.AddColumn<int>(
                name: "DuracionMinutos",
                table: "Citas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MotivoCancelacion",
                table: "Citas",
                type: "TEXT",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DuracionMinutos",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "MotivoCancelacion",
                table: "Citas");

            migrationBuilder.RenameColumn(
                name: "MotivoConsulta",
                table: "Citas",
                newName: "Motivo");

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Citas",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }
    }
}
