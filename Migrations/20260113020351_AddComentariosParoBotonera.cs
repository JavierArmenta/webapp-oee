using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddComentariosParoBotonera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComentariosParoBotonera",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistroParoBotoneraId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Comentario = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosParoBotonera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosParoBotonera_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "authentication",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComentariosParoBotonera_RegistrosParoBotonera_RegistroParoB~",
                        column: x => x.RegistroParoBotoneraId,
                        principalSchema: "linealytics",
                        principalTable: "RegistrosParoBotonera",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosParoBotonera_RegistroParoBotoneraId",
                schema: "linealytics",
                table: "ComentariosParoBotonera",
                column: "RegistroParoBotoneraId");

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosParoBotonera_UserId",
                schema: "linealytics",
                table: "ComentariosParoBotonera",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComentariosParoBotonera",
                schema: "linealytics");
        }
    }
}
