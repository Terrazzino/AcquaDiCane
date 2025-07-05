using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AcquaDiCane.Migrations
{
    /// <inheritdoc />
    public partial class Seedmetodosdepago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MetodosDePago",
                columns: new[] { "Id", "NombreDelMetodo" },
                values: new object[,]
                {
                    { 1, "Efectivo" },
                    { 2, "MercadoPago" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MetodosDePago",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MetodosDePago",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
