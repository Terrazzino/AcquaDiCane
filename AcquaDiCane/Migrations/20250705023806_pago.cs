using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcquaDiCane.Migrations
{
    /// <inheritdoc />
    public partial class pago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pagos_TurnoId",
                table: "Pagos");

            migrationBuilder.AddColumn<int>(
                name: "PagoId",
                table: "Turnos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TurnoId2",
                table: "Pagos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "MetodosDePago",
                columns: new[] { "Id", "NombreDelMetodo" },
                values: new object[] { 3, "Pendiente" });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_PagoId",
                table: "Turnos",
                column: "PagoId",
                unique: true,
                filter: "[PagoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TurnoId",
                table: "Pagos",
                column: "TurnoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Pagos_PagoId",
                table: "Turnos",
                column: "PagoId",
                principalTable: "Pagos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Pagos_PagoId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_PagoId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_TurnoId",
                table: "Pagos");

            migrationBuilder.DeleteData(
                table: "MetodosDePago",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "PagoId",
                table: "Turnos");

            migrationBuilder.DropColumn(
                name: "TurnoId2",
                table: "Pagos");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TurnoId",
                table: "Pagos",
                column: "TurnoId",
                unique: true);
        }
    }
}
