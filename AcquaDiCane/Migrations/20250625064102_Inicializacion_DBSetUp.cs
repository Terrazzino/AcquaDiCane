using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcquaDiCane.Migrations
{
    /// <inheritdoc />
    public partial class Inicializacion_DBSetUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleDelTurno_Servicios_ServicioAsignadoId",
                table: "DetalleDelTurno");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleDelTurno_Turnos_TurnoAsignadoId",
                table: "DetalleDelTurno");

            migrationBuilder.DropForeignKey(
                name: "FK_Jornadas_Peluqueros_PeluqueroCorrespondienteId",
                table: "Jornadas");

            migrationBuilder.DropForeignKey(
                name: "FK_Pago_MetodoDePago_MetodoDePagoId",
                table: "Pago");

            migrationBuilder.DropForeignKey(
                name: "FK_Pago_Turnos_TurnoId",
                table: "Pago");

            migrationBuilder.DropForeignKey(
                name: "FK_ReciboDePagos_Pago_PagoId",
                table: "ReciboDePagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Peluqueros_PeluqueroAsignadoId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReciboDePagos",
                table: "ReciboDePagos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pago",
                table: "Pago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetodoDePago",
                table: "MetodoDePago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jornadas",
                table: "Jornadas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetalleDelTurno",
                table: "DetalleDelTurno");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "Contraseña",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "CorreoElectronico",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Contraseña",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CorreoElectronico",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Clientes");

            migrationBuilder.RenameTable(
                name: "ReciboDePagos",
                newName: "RecibosDePago");

            migrationBuilder.RenameTable(
                name: "Pago",
                newName: "Pagos");

            migrationBuilder.RenameTable(
                name: "MetodoDePago",
                newName: "MetodosDePago");

            migrationBuilder.RenameTable(
                name: "Jornadas",
                newName: "JornadasSemanales");

            migrationBuilder.RenameTable(
                name: "DetalleDelTurno",
                newName: "DetallesDeTurnos");

            migrationBuilder.RenameIndex(
                name: "IX_ReciboDePagos_PagoId",
                table: "RecibosDePago",
                newName: "IX_RecibosDePago_PagoId");

            migrationBuilder.RenameIndex(
                name: "IX_Pago_TurnoId",
                table: "Pagos",
                newName: "IX_Pagos_TurnoId");

            migrationBuilder.RenameIndex(
                name: "IX_Pago_MetodoDePagoId",
                table: "Pagos",
                newName: "IX_Pagos_MetodoDePagoId");

            migrationBuilder.RenameIndex(
                name: "IX_Jornadas_PeluqueroCorrespondienteId",
                table: "JornadasSemanales",
                newName: "IX_JornadasSemanales_PeluqueroCorrespondienteId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleDelTurno_TurnoAsignadoId",
                table: "DetallesDeTurnos",
                newName: "IX_DetallesDeTurnos_TurnoAsignadoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleDelTurno_ServicioAsignadoId",
                table: "DetallesDeTurnos",
                newName: "IX_DetallesDeTurnos_ServicioAsignadoId");

            migrationBuilder.AddColumn<int>(
                name: "AplicationUserId",
                table: "Peluqueros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AplicationUserId",
                table: "Clientes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TurnoId1",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RecibosDePago",
                table: "RecibosDePago",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pagos",
                table: "Pagos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetodosDePago",
                table: "MetodosDePago",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JornadasSemanales",
                table: "JornadasSemanales",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetallesDeTurnos",
                table: "DetallesDeTurnos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DNI = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos",
                column: "MascotaAsignadaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Peluqueros_AplicationUserId",
                table: "Peluqueros",
                column: "AplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_AplicationUserId",
                table: "Clientes",
                column: "AplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TurnoId1",
                table: "Pagos",
                column: "TurnoId1",
                unique: true,
                filter: "[TurnoId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_AspNetUsers_AplicationUserId",
                table: "Clientes",
                column: "AplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesDeTurnos_Servicios_ServicioAsignadoId",
                table: "DetallesDeTurnos",
                column: "ServicioAsignadoId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesDeTurnos_Turnos_TurnoAsignadoId",
                table: "DetallesDeTurnos",
                column: "TurnoAsignadoId",
                principalTable: "Turnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JornadasSemanales_Peluqueros_PeluqueroCorrespondienteId",
                table: "JornadasSemanales",
                column: "PeluqueroCorrespondienteId",
                principalTable: "Peluqueros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_MetodosDePago_MetodoDePagoId",
                table: "Pagos",
                column: "MetodoDePagoId",
                principalTable: "MetodosDePago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Turnos_TurnoId",
                table: "Pagos",
                column: "TurnoId",
                principalTable: "Turnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pagos_Turnos_TurnoId1",
                table: "Pagos",
                column: "TurnoId1",
                principalTable: "Turnos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Peluqueros_AspNetUsers_AplicationUserId",
                table: "Peluqueros",
                column: "AplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecibosDePago_Pagos_PagoId",
                table: "RecibosDePago",
                column: "PagoId",
                principalTable: "Pagos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Peluqueros_PeluqueroAsignadoId",
                table: "Turnos",
                column: "PeluqueroAsignadoId",
                principalTable: "Peluqueros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_AspNetUsers_AplicationUserId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesDeTurnos_Servicios_ServicioAsignadoId",
                table: "DetallesDeTurnos");

            migrationBuilder.DropForeignKey(
                name: "FK_DetallesDeTurnos_Turnos_TurnoAsignadoId",
                table: "DetallesDeTurnos");

            migrationBuilder.DropForeignKey(
                name: "FK_JornadasSemanales_Peluqueros_PeluqueroCorrespondienteId",
                table: "JornadasSemanales");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_MetodosDePago_MetodoDePagoId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Turnos_TurnoId",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pagos_Turnos_TurnoId1",
                table: "Pagos");

            migrationBuilder.DropForeignKey(
                name: "FK_Peluqueros_AspNetUsers_AplicationUserId",
                table: "Peluqueros");

            migrationBuilder.DropForeignKey(
                name: "FK_RecibosDePago_Pagos_PagoId",
                table: "RecibosDePago");

            migrationBuilder.DropForeignKey(
                name: "FK_Turnos_Peluqueros_PeluqueroAsignadoId",
                table: "Turnos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos");

            migrationBuilder.DropIndex(
                name: "IX_Peluqueros_AplicationUserId",
                table: "Peluqueros");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_AplicationUserId",
                table: "Clientes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RecibosDePago",
                table: "RecibosDePago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pagos",
                table: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Pagos_TurnoId1",
                table: "Pagos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetodosDePago",
                table: "MetodosDePago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JornadasSemanales",
                table: "JornadasSemanales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetallesDeTurnos",
                table: "DetallesDeTurnos");

            migrationBuilder.DropColumn(
                name: "AplicationUserId",
                table: "Peluqueros");

            migrationBuilder.DropColumn(
                name: "AplicationUserId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TurnoId1",
                table: "Pagos");

            migrationBuilder.RenameTable(
                name: "RecibosDePago",
                newName: "ReciboDePagos");

            migrationBuilder.RenameTable(
                name: "Pagos",
                newName: "Pago");

            migrationBuilder.RenameTable(
                name: "MetodosDePago",
                newName: "MetodoDePago");

            migrationBuilder.RenameTable(
                name: "JornadasSemanales",
                newName: "Jornadas");

            migrationBuilder.RenameTable(
                name: "DetallesDeTurnos",
                newName: "DetalleDelTurno");

            migrationBuilder.RenameIndex(
                name: "IX_RecibosDePago_PagoId",
                table: "ReciboDePagos",
                newName: "IX_ReciboDePagos_PagoId");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_TurnoId",
                table: "Pago",
                newName: "IX_Pago_TurnoId");

            migrationBuilder.RenameIndex(
                name: "IX_Pagos_MetodoDePagoId",
                table: "Pago",
                newName: "IX_Pago_MetodoDePagoId");

            migrationBuilder.RenameIndex(
                name: "IX_JornadasSemanales_PeluqueroCorrespondienteId",
                table: "Jornadas",
                newName: "IX_Jornadas_PeluqueroCorrespondienteId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallesDeTurnos_TurnoAsignadoId",
                table: "DetalleDelTurno",
                newName: "IX_DetalleDelTurno_TurnoAsignadoId");

            migrationBuilder.RenameIndex(
                name: "IX_DetallesDeTurnos_ServicioAsignadoId",
                table: "DetalleDelTurno",
                newName: "IX_DetalleDelTurno_ServicioAsignadoId");

            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Peluqueros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Peluqueros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contraseña",
                table: "Peluqueros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorreoElectronico",
                table: "Peluqueros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Peluqueros",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Peluqueros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Peluqueros",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contraseña",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CorreoElectronico",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Clientes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReciboDePagos",
                table: "ReciboDePagos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pago",
                table: "Pago",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetodoDePago",
                table: "MetodoDePago",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jornadas",
                table: "Jornadas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetalleDelTurno",
                table: "DetalleDelTurno",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_MascotaAsignadaId",
                table: "Turnos",
                column: "MascotaAsignadaId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleDelTurno_Servicios_ServicioAsignadoId",
                table: "DetalleDelTurno",
                column: "ServicioAsignadoId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleDelTurno_Turnos_TurnoAsignadoId",
                table: "DetalleDelTurno",
                column: "TurnoAsignadoId",
                principalTable: "Turnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jornadas_Peluqueros_PeluqueroCorrespondienteId",
                table: "Jornadas",
                column: "PeluqueroCorrespondienteId",
                principalTable: "Peluqueros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pago_MetodoDePago_MetodoDePagoId",
                table: "Pago",
                column: "MetodoDePagoId",
                principalTable: "MetodoDePago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pago_Turnos_TurnoId",
                table: "Pago",
                column: "TurnoId",
                principalTable: "Turnos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReciboDePagos_Pago_PagoId",
                table: "ReciboDePagos",
                column: "PagoId",
                principalTable: "Pago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Turnos_Peluqueros_PeluqueroAsignadoId",
                table: "Turnos",
                column: "PeluqueroAsignadoId",
                principalTable: "Peluqueros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
