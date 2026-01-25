using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class contadoresMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContadoresDispositivo",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TipoContador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContadoresDispositivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContadoresDispositivo_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CorridasProduccion",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContadorDispositivoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContadorInicial = table.Column<long>(type: "bigint", nullable: false),
                    ContadorFinal = table.Column<long>(type: "bigint", nullable: false),
                    UltimoContadorValor = table.Column<long>(type: "bigint", nullable: false),
                    ProduccionTotal = table.Column<long>(type: "bigint", nullable: false),
                    NumeroResets = table.Column<int>(type: "integer", nullable: false),
                    NumeroLecturas = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorridasProduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorridasProduccion_ContadoresDispositivo_ContadorDispositiv~",
                        column: x => x.ContadorDispositivoId,
                        principalSchema: "linealytics",
                        principalTable: "ContadoresDispositivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorridasProduccion_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumenesProduccionDia",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContadorDispositivoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    ProduccionTotal = table.Column<long>(type: "bigint", nullable: false),
                    NumeroLecturas = table.Column<int>(type: "integer", nullable: false),
                    NumeroResets = table.Column<int>(type: "integer", nullable: false),
                    NumeroCorridasIniciadas = table.Column<int>(type: "integer", nullable: false),
                    NumeroCorridasCerradas = table.Column<int>(type: "integer", nullable: false),
                    TiempoProduccionMinutos = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumenesProduccionDia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumenesProduccionDia_ContadoresDispositivo_ContadorDispos~",
                        column: x => x.ContadorDispositivoId,
                        principalSchema: "linealytics",
                        principalTable: "ContadoresDispositivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResumenesProduccionDia_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumenesProduccionHora",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContadorDispositivoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<int>(type: "integer", nullable: false),
                    ProduccionTotal = table.Column<long>(type: "bigint", nullable: false),
                    NumeroLecturas = table.Column<int>(type: "integer", nullable: false),
                    NumeroResets = table.Column<int>(type: "integer", nullable: false),
                    ContadorInicio = table.Column<long>(type: "bigint", nullable: false),
                    ContadorFin = table.Column<long>(type: "bigint", nullable: false),
                    ValorMinimo = table.Column<long>(type: "bigint", nullable: false),
                    ValorMaximo = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumenesProduccionHora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumenesProduccionHora_ContadoresDispositivo_ContadorDispo~",
                        column: x => x.ContadorDispositivoId,
                        principalSchema: "linealytics",
                        principalTable: "ContadoresDispositivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ResumenesProduccionHora_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LecturasContadorNuevo",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CorridaId = table.Column<int>(type: "integer", nullable: false),
                    ContadorDispositivoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    ContadorValor = table.Column<long>(type: "bigint", nullable: false),
                    ContadorAnterior = table.Column<long>(type: "bigint", nullable: true),
                    Diferencia = table.Column<long>(type: "bigint", nullable: false),
                    ProduccionIncremental = table.Column<long>(type: "bigint", nullable: false),
                    EsReset = table.Column<bool>(type: "boolean", nullable: false),
                    EsRuido = table.Column<bool>(type: "boolean", nullable: false),
                    FechaHoraLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturasContadorNuevo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LecturasContadorNuevo_ContadoresDispositivo_ContadorDisposi~",
                        column: x => x.ContadorDispositivoId,
                        principalSchema: "linealytics",
                        principalTable: "ContadoresDispositivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LecturasContadorNuevo_CorridasProduccion_CorridaId",
                        column: x => x.CorridaId,
                        principalSchema: "linealytics",
                        principalTable: "CorridasProduccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LecturasContadorNuevo_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContadoresDispositivo_MaquinaId_Nombre",
                schema: "linealytics",
                table: "ContadoresDispositivo",
                columns: new[] { "MaquinaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorridasProduccion_ContadorId_Estado",
                schema: "linealytics",
                table: "CorridasProduccion",
                columns: new[] { "ContadorDispositivoId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_CorridasProduccion_ContadorId_FechaInicio",
                schema: "linealytics",
                table: "CorridasProduccion",
                columns: new[] { "ContadorDispositivoId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_CorridasProduccion_ProductoId",
                schema: "linealytics",
                table: "CorridasProduccion",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContadorNuevo_ContadorId_Fecha",
                schema: "linealytics",
                table: "LecturasContadorNuevo",
                columns: new[] { "ContadorDispositivoId", "FechaHoraLectura" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContadorNuevo_CorridaId_Fecha",
                schema: "linealytics",
                table: "LecturasContadorNuevo",
                columns: new[] { "CorridaId", "FechaHoraLectura" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContadorNuevo_ProductoId",
                schema: "linealytics",
                table: "LecturasContadorNuevo",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumenesProduccionDia_ContadorId_Fecha",
                schema: "linealytics",
                table: "ResumenesProduccionDia",
                columns: new[] { "ContadorDispositivoId", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResumenesProduccionDia_ProductoId",
                schema: "linealytics",
                table: "ResumenesProduccionDia",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumenesProduccionHora_ContadorId_Fecha_Hora",
                schema: "linealytics",
                table: "ResumenesProduccionHora",
                columns: new[] { "ContadorDispositivoId", "Fecha", "Hora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResumenesProduccionHora_ProductoId",
                schema: "linealytics",
                table: "ResumenesProduccionHora",
                column: "ProductoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LecturasContadorNuevo",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "ResumenesProduccionDia",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "ResumenesProduccionHora",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "CorridasProduccion",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "ContadoresDispositivo",
                schema: "linealytics");
        }
    }
}
