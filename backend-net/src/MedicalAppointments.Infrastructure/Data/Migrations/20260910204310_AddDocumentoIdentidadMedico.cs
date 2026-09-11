using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalAppointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentoIdentidadMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentoIdentidad",
                table: "Medicos",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Medicos",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DocumentoIdentidad",
                value: "0101010101");

            migrationBuilder.UpdateData(
                table: "Medicos",
                keyColumn: "Id",
                keyValue: 2L,
                column: "DocumentoIdentidad",
                value: "0202020202");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_DocumentoIdentidad",
                table: "Medicos",
                column: "DocumentoIdentidad",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Medicos_DocumentoIdentidad",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "DocumentoIdentidad",
                table: "Medicos");
        }
    }
}
