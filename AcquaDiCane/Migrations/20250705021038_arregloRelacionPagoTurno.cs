using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcquaDiCane.Migrations
{
    /// <inheritdoc />
    public partial class arregloRelacionPagoTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Turnos_TurnoId1",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_TurnoId1",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "TurnoId1",
                table: "Pagos");

            migrationBuilder.AlterColumn<string>(
                name: "CuentaDestino",
                table: "Pagos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CuentaDestino",
                table: "Pagos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TurnoId1",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TurnoId1",
                table: "Pagos",
                column: "TurnoId1",
                unique: true,
                filter: "[TurnoId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Turnos_TurnoId1",
                table: "Pagos",
                column: "TurnoId1",
                principalTable: "Turnos",
                principalColumn: "Id");
        }
    }
}
