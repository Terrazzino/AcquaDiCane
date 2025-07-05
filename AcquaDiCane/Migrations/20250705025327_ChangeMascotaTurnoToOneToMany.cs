using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcquaDiCane.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMascotaTurnoToOneToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos",
                column: "MascotaAsignadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos",
                column: "MascotaAsignadaId",
                unique: true);
        }
    }
}
