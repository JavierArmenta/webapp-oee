using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class SistemaFallas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFallas_Modelos_ModeloId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosFallas_FallaId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosFallas_ModeloId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "FallaId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.RenameColumn(
                name: "ModeloId",
                schema: "linealytics",
                table: "RegistrosFallas",
                newName: "DuracionMinutos");

            migrationBuilder.RenameColumn(
                name: "FechaHoraLectura",
                schema: "linealytics",
                table: "RegistrosFallas",
                newName: "FechaHoraDeteccion");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosFallas_MaquinaId_FechaHoraLectura",
                schema: "linealytics",
                table: "RegistrosFallas",
                newName: "IX_RegistrosFallas_MaquinaId_FechaHoraDeteccion");

            migrationBuilder.AddColumn<string>(
                name: "AccionesTomadas",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CatalogoFallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pendiente");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraAtencion",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraResolucion",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TecnicoAsignadoId",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CatalogoFallas",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Severidad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TiempoEstimadoSolucionMinutos = table.Column<int>(type: "integer", nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogoFallas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_CatalogoFallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "CatalogoFallaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_Estado",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_TecnicoAsignadoId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "TecnicoAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoFallas_Codigo",
                schema: "linealytics",
                table: "CatalogoFallas",
                column: "Codigo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFallas_AspNetUsers_TecnicoAsignadoId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "TecnicoAsignadoId",
                principalSchema: "authentication",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFallas_CatalogoFallas_CatalogoFallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "CatalogoFallaId",
                principalSchema: "linealytics",
                principalTable: "CatalogoFallas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFallas_AspNetUsers_TecnicoAsignadoId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosFallas_CatalogoFallas_CatalogoFallaId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropTable(
                name: "CatalogoFallas",
                schema: "linealytics");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosFallas_CatalogoFallaId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosFallas_Estado",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosFallas_TecnicoAsignadoId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "AccionesTomadas",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "CatalogoFallaId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "Estado",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "FechaHoraAtencion",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "FechaHoraResolucion",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.DropColumn(
                name: "TecnicoAsignadoId",
                schema: "linealytics",
                table: "RegistrosFallas");

            migrationBuilder.RenameColumn(
                name: "FechaHoraDeteccion",
                schema: "linealytics",
                table: "RegistrosFallas",
                newName: "FechaHoraLectura");

            migrationBuilder.RenameColumn(
                name: "DuracionMinutos",
                schema: "linealytics",
                table: "RegistrosFallas",
                newName: "ModeloId");

            migrationBuilder.RenameIndex(
                name: "IX_RegistrosFallas_MaquinaId_FechaHoraDeteccion",
                schema: "linealytics",
                table: "RegistrosFallas",
                newName: "IX_RegistrosFallas_MaquinaId_FechaHoraLectura");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_FallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "FallaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_ModeloId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "ModeloId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosFallas_Modelos_ModeloId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "ModeloId",
                principalSchema: "planta",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
