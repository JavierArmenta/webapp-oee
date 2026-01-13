using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddBotonIdToRegistroParoBotonera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BotonId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_BotonId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "BotonId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosParoBotonera_Botones_BotonId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "BotonId",
                principalSchema: "planta",
                principalTable: "Botones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosParoBotonera_Botones_BotonId",
                schema: "linealytics",
                table: "RegistrosParoBotonera");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosParoBotonera_BotonId",
                schema: "linealytics",
                table: "RegistrosParoBotonera");

            migrationBuilder.DropColumn(
                name: "BotonId",
                schema: "linealytics",
                table: "RegistrosParoBotonera");
        }
    }
}
