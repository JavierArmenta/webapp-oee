using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class EliminarCategoriasYCausasParo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFallas_CausasParo_FallaId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosParos_CausasParo_CausaParoId",
                schema: "linealytics",
                table: "RegistrosParos");

            migrationBuilder.DropTable(
                name: "CausasParo",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "CategoriasParo",
                schema: "linealytics");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosParos_CausaParoId",
                schema: "linealytics",
                table: "RegistrosParos");

            migrationBuilder.AlterColumn<int>(
                name: "CausaParoId",
                schema: "linealytics",
                table: "RegistrosParos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CausaParoId",
                schema: "linealytics",
                table: "RegistrosParos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CategoriasParo",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EsPlaneado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasParo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CausasParo",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoriaParoId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CodigoInterno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RequiereMantenimiento = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CausasParo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CausasParo_CategoriasParo_CategoriaParoId",
                        column: x => x.CategoriaParoId,
                        principalSchema: "linealytics",
                        principalTable: "CategoriasParo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParos_CausaParoId",
                schema: "linealytics",
                table: "RegistrosParos",
                column: "CausaParoId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasParo_Nombre",
                schema: "linealytics",
                table: "CategoriasParo",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CausasParo_CategoriaParoId_Nombre",
                schema: "linealytics",
                table: "CausasParo",
                columns: new[] { "CategoriaParoId", "Nombre" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFallas_CausasParo_FallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "FallaId",
                principalSchema: "linealytics",
                principalTable: "CausasParo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosParos_CausasParo_CausaParoId",
                schema: "linealytics",
                table: "RegistrosParos",
                column: "CausaParoId",
                principalSchema: "linealytics",
                principalTable: "CausasParo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
