using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pedidos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.CheckConstraint("CK_Usuarios_Rol", "[Rol] IN ('Admin','User')");
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroPedido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Cliente = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                    table.CheckConstraint("CK_Pedidos_Estado", "[Estado] IN ('Registrado','EnProceso','Completado','Cancelado')");
                    table.CheckConstraint("CK_Pedidos_Total", "[Total] > 0");
                    table.ForeignKey(
                        name: "FK_Pedidos_UsuarioCreacion",
                        column: x => x.UsuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedidos_UsuarioModificacion",
                        column: x => x.UsuarioModificacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Email", "FechaCreacion", "Nombre", "PasswordHash", "Rol" },
                values: new object[,]
                {
                    { 1, true, "admin@pedidos.com", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador", "$2a$11$6/hvar.9e3218SzBYSruKe135GDRXVLzVtO08r8t0RnYUNvd1kpjm", "Admin" },
                    { 2, true, "user@pedidos.com", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Usuario Demo", "$2a$11$weSJCsNw0/hk1O0jb3YTI.FHbLjUUoDPM3j6ZDZr6d3De3j2.GHXC", "User" }
                });

            migrationBuilder.InsertData(
                table: "Pedidos",
                columns: new[] { "Id", "Cliente", "Estado", "Fecha", "FechaActualizacion", "FechaCreacion", "NumeroPedido", "Total", "UsuarioCreacionId", "UsuarioModificacionId" },
                values: new object[,]
                {
                    { 1, "Juan Perez", "Registrado", new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "PE-001", 250.75m, 1, null },
                    { 2, "Maria Gonzalez", "EnProceso", new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "PE-002", 1200.00m, 1, null },
                    { 3, "Carlos Ramirez", "Completado", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "PE-003", 89.90m, 2, null },
                    { 4, "Ana Torres", "Registrado", new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "PE-004", 430.50m, 2, null },
                    { 5, "Luis Fernandez", "Cancelado", new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "PE-005", 75.00m, 1, null }
                });

            // Índices de FK removidos a mano (EF los agrega por convención) para igualar el script.
            migrationBuilder.CreateIndex(
                name: "UQ_Pedidos_NumeroPedido",
                table: "Pedidos",
                column: "NumeroPedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
