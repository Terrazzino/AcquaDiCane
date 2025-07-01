using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcquaDiCane.Migrations
{
    /// <inheritdoc />
    public partial class NuevoTipoDeDatoParaPrecios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duracion",
                table: "Servicios");

            migrationBuilder.RenameColumn(
                name: "nombreServicio",
                table: "Servicios",
                newName: "Nombre");

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioTotal",
                table: "Turnos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Servicios",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Servicios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DuracionEnMinutos",
                table: "Servicios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "PrecioServicio",
                table: "DetallesDeTurnos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Servicios");

            migrationBuilder.DropColumn(
                name: "DuracionEnMinutos",
                table: "Servicios");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Servicios",
                newName: "nombreServicio");

            migrationBuilder.AlterColumn<double>(
                name: "PrecioTotal",
                table: "Turnos",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Precio",
                table: "Servicios",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<double>(
                name: "Duracion",
                table: "Servicios",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "PrecioServicio",
                table: "DetallesDeTurnos",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
