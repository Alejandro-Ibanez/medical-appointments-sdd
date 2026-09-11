using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHistorialClinicoCompleto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialesClinicos_Medicos_MedicoId",
                table: "HistorialesClinicos");

            migrationBuilder.DropIndex(
                name: "IX_HistorialesClinicos_MedicoId",
                table: "HistorialesClinicos");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "HistorialesClinicos");

            migrationBuilder.AddColumn<string>(
                name: "MedicoNombre",
                table: "HistorialesClinicos",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicoNombre",
                table: "HistorialesClinicos");

            migrationBuilder.AddColumn<long>(
                name: "MedicoId",
                table: "HistorialesClinicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesClinicos_MedicoId",
                table: "HistorialesClinicos",
                column: "MedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialesClinicos_Medicos_MedicoId",
                table: "HistorialesClinicos",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
