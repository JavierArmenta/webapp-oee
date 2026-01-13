using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateLinealyticsNewSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modelos",
                schema: "planta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modelos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosParoBotonera",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    DepartamentoId = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    FechaHoraFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "integer", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Abierto")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosParoBotonera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosParoBotonera_DepartamentosOperador_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalSchema: "operadores",
                        principalTable: "DepartamentosOperador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosParoBotonera_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosContadores",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    ContadorOK = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ContadorNOK = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    FechaHoraLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModeloId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosContadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosContadores_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosContadores_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalSchema: "planta",
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosFallas",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FallaId = table.Column<int>(type: "integer", nullable: false),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ModeloId = table.Column<int>(type: "integer", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosFallas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosFallas_CausasParo_FallaId",
                        column: x => x.FallaId,
                        principalSchema: "linealytics",
                        principalTable: "CausasParo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosFallas_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosFallas_Modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalSchema: "planta",
                        principalTable: "Modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Modelos_Codigo",
                schema: "planta",
                table: "Modelos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosContadores_MaquinaId_FechaHoraLectura",
                schema: "linealytics",
                table: "RegistrosContadores",
                columns: new[] { "MaquinaId", "FechaHoraLectura" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosContadores_ModeloId",
                schema: "linealytics",
                table: "RegistrosContadores",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_FallaId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "FallaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_MaquinaId_FechaHoraLectura",
                schema: "linealytics",
                table: "RegistrosFallas",
                columns: new[] { "MaquinaId", "FechaHoraLectura" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_ModeloId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_DepartamentoId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_Estado",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_MaquinaId_FechaHoraInicio",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                columns: new[] { "MaquinaId", "FechaHoraInicio" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosContadores",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "RegistrosFallas",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "RegistrosParoBotonera",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "Modelos",
                schema: "planta");
        }
    }
}
