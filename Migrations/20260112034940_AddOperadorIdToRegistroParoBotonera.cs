using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOperadorIdToRegistroParoBotonera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperadorId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_OperadorId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "OperadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosParoBotonera_Operadores_OperadorId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "OperadorId",
                principalSchema: "operadores",
                principalTable: "Operadores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosParoBotonera_Operadores_OperadorId",
                schema: "linealytics",
                table: "RegistrosParoBotonera");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosParoBotonera_OperadorId",
                schema: "linealytics",
                table: "RegistrosParoBotonera");

            migrationBuilder.DropColumn(
                name: "OperadorId",
                schema: "linealytics",
                table: "RegistrosParoBotonera");
        }
    }
}
