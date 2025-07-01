using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcquaDiCane.Migrations
{
    /// <inheritdoc />
    public partial class NuevoTipoDeDatoParaPreciosV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MotivoCancelacion",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesFinalizacion",
                table: "Turnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "MotivoCancelacion",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "ObservacionesFinalizacion",
                table: "Turnos");
        }
    }
}
