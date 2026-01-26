using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class SimplificarContadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorridasProduccion_ContadoresDispositivo_ContadorDispositiv~",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturasContador_Dispositivos_DispositivoId",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturasContador_MetricasMaquina_MetricasMaquinaId",
                schema: "linealytics",
                table: "LecturasContador");

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
                name: "ContadoresDispositivo",
                schema: "linealytics");

            migrationBuilder.DropIndex(
                name: "IX_LecturasContador_DispositivoId",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropIndex(
                name: "IX_LecturasContador_MaquinaId_FechaLectura",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropIndex(
                name: "IX_LecturasContador_MetricasMaquinaId",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropIndex(
                name: "IX_CorridasProduccion_ContadorId_Estado",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropIndex(
                name: "IX_CorridasProduccion_ContadorId_FechaInicio",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropColumn(
                name: "Contador",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "ContadorAnterior",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "FechaLectura",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "MetricasMaquinaId",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "UnidadesProducidas",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.RenameColumn(
                name: "DispositivoId",
                schema: "linealytics",
                table: "LecturasContador",
                newName: "CorridaId");

            migrationBuilder.RenameColumn(
                name: "UltimoContadorValor",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "UltimoContadorOK");

            migrationBuilder.RenameColumn(
                name: "ProduccionTotal",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "UltimoContadorNOK");

            migrationBuilder.RenameColumn(
                name: "NumeroResets",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "NumeroResetsOK");

            migrationBuilder.RenameColumn(
                name: "ContadorInicial",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "ProduccionOK");

            migrationBuilder.RenameColumn(
                name: "ContadorFinal",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "ProduccionNOK");

            migrationBuilder.RenameColumn(
                name: "ContadorDispositivoId",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "NumeroResetsNOK");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                schema: "linealytics",
                table: "LecturasContador",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContadorNOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ContadorNOKAnterior",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContadorOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ContadorOKAnterior",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DiferenciaNOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "DiferenciaOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "EsResetNOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsResetOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraLectura",
                schema: "linealytics",
                table: "LecturasContador",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "ProduccionNOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProduccionOK",
                schema: "linealytics",
                table: "LecturasContador",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                schema: "linealytics",
                table: "CorridasProduccion",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContadorNOKFinal",
                schema: "linealytics",
                table: "CorridasProduccion",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ContadorNOKInicial",
                schema: "linealytics",
                table: "CorridasProduccion",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ContadorOKFinal",
                schema: "linealytics",
                table: "CorridasProduccion",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ContadorOKInicial",
                schema: "linealytics",
                table: "CorridasProduccion",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "MaquinaId",
                schema: "linealytics",
                table: "CorridasProduccion",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContador_CorridaId_Fecha",
                schema: "linealytics",
                table: "LecturasContador",
                columns: new[] { "CorridaId", "FechaHoraLectura" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContador_MaquinaId_Fecha",
                schema: "linealytics",
                table: "LecturasContador",
                columns: new[] { "MaquinaId", "FechaHoraLectura" });

            migrationBuilder.CreateIndex(
                name: "IX_CorridasProduccion_MaquinaId_Estado",
                schema: "linealytics",
                table: "CorridasProduccion",
                columns: new[] { "MaquinaId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_CorridasProduccion_MaquinaId_FechaInicio",
                schema: "linealytics",
                table: "CorridasProduccion",
                columns: new[] { "MaquinaId", "FechaInicio" });

            migrationBuilder.AddForeignKey(
                name: "FK_CorridasProduccion_Maquinas_MaquinaId",
                schema: "linealytics",
                table: "CorridasProduccion",
                column: "MaquinaId",
                principalSchema: "planta",
                principalTable: "Maquinas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturasContador_CorridasProduccion_CorridaId",
                schema: "linealytics",
                table: "LecturasContador",
                column: "CorridaId",
                principalSchema: "linealytics",
                principalTable: "CorridasProduccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorridasProduccion_Maquinas_MaquinaId",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropForeignKey(
                name: "FK_LecturasContador_CorridasProduccion_CorridaId",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropIndex(
                name: "IX_LecturasContador_CorridaId_Fecha",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropIndex(
                name: "IX_LecturasContador_MaquinaId_Fecha",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropIndex(
                name: "IX_CorridasProduccion_MaquinaId_Estado",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropIndex(
                name: "IX_CorridasProduccion_MaquinaId_FechaInicio",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropColumn(
                name: "ContadorNOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "ContadorNOKAnterior",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "ContadorOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "ContadorOKAnterior",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "DiferenciaNOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "DiferenciaOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "EsResetNOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "EsResetOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "FechaHoraLectura",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "ProduccionNOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "ProduccionOK",
                schema: "linealytics",
                table: "LecturasContador");

            migrationBuilder.DropColumn(
                name: "ContadorNOKFinal",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropColumn(
                name: "ContadorNOKInicial",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropColumn(
                name: "ContadorOKFinal",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropColumn(
                name: "ContadorOKInicial",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.DropColumn(
                name: "MaquinaId",
                schema: "linealytics",
                table: "CorridasProduccion");

            migrationBuilder.RenameColumn(
                name: "CorridaId",
                schema: "linealytics",
                table: "LecturasContador",
                newName: "DispositivoId");

            migrationBuilder.RenameColumn(
                name: "UltimoContadorOK",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "UltimoContadorValor");

            migrationBuilder.RenameColumn(
                name: "UltimoContadorNOK",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "ProduccionTotal");

            migrationBuilder.RenameColumn(
                name: "ProduccionOK",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "ContadorInicial");

            migrationBuilder.RenameColumn(
                name: "ProduccionNOK",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "ContadorFinal");

            migrationBuilder.RenameColumn(
                name: "NumeroResetsOK",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "NumeroResets");

            migrationBuilder.RenameColumn(
                name: "NumeroResetsNOK",
                schema: "linealytics",
                table: "CorridasProduccion",
                newName: "ContadorDispositivoId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                schema: "linealytics",
                table: "LecturasContador",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Contador",
                schema: "linealytics",
                table: "LecturasContador",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContadorAnterior",
                schema: "linealytics",
                table: "LecturasContador",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLectura",
                schema: "linealytics",
                table: "LecturasContador",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AddColumn<int>(
                name: "MetricasMaquinaId",
                schema: "linealytics",
                table: "LecturasContador",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                schema: "linealytics",
                table: "LecturasContador",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnidadesProducidas",
                schema: "linealytics",
                table: "LecturasContador",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoId",
                schema: "linealytics",
                table: "CorridasProduccion",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "ContadoresDispositivo",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoContador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
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
                name: "LecturasContadorNuevo",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContadorDispositivoId = table.Column<int>(type: "integer", nullable: false),
                    CorridaId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    ContadorAnterior = table.Column<long>(type: "bigint", nullable: true),
                    ContadorValor = table.Column<long>(type: "bigint", nullable: false),
                    Diferencia = table.Column<long>(type: "bigint", nullable: false),
                    EsReset = table.Column<bool>(type: "boolean", nullable: false),
                    EsRuido = table.Column<bool>(type: "boolean", nullable: false),
                    FechaHoraLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProduccionIncremental = table.Column<long>(type: "bigint", nullable: false)
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
                    NumeroCorridasCerradas = table.Column<int>(type: "integer", nullable: false),
                    NumeroCorridasIniciadas = table.Column<int>(type: "integer", nullable: false),
                    NumeroLecturas = table.Column<int>(type: "integer", nullable: false),
                    NumeroResets = table.Column<int>(type: "integer", nullable: false),
                    ProduccionTotal = table.Column<long>(type: "bigint", nullable: false),
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
                    ContadorFin = table.Column<long>(type: "bigint", nullable: false),
                    ContadorInicio = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    Hora = table.Column<int>(type: "integer", nullable: false),
                    NumeroLecturas = table.Column<int>(type: "integer", nullable: false),
                    NumeroResets = table.Column<int>(type: "integer", nullable: false),
                    ProduccionTotal = table.Column<long>(type: "bigint", nullable: false),
                    ValorMaximo = table.Column<long>(type: "bigint", nullable: false),
                    ValorMinimo = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContador_DispositivoId",
                schema: "linealytics",
                table: "LecturasContador",
                column: "DispositivoId");

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContador_MaquinaId_FechaLectura",
                schema: "linealytics",
                table: "LecturasContador",
                columns: new[] { "MaquinaId", "FechaLectura" });

            migrationBuilder.CreateIndex(
                name: "IX_LecturasContador_MetricasMaquinaId",
                schema: "linealytics",
                table: "LecturasContador",
                column: "MetricasMaquinaId");

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
                name: "IX_ContadoresDispositivo_MaquinaId_Nombre",
                schema: "linealytics",
                table: "ContadoresDispositivo",
                columns: new[] { "MaquinaId", "Nombre" },
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_CorridasProduccion_ContadoresDispositivo_ContadorDispositiv~",
                schema: "linealytics",
                table: "CorridasProduccion",
                column: "ContadorDispositivoId",
                principalSchema: "linealytics",
                principalTable: "ContadoresDispositivo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturasContador_Dispositivos_DispositivoId",
                schema: "linealytics",
                table: "LecturasContador",
                column: "DispositivoId",
                principalSchema: "linealytics",
                principalTable: "Dispositivos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LecturasContador_MetricasMaquina_MetricasMaquinaId",
                schema: "linealytics",
                table: "LecturasContador",
                column: "MetricasMaquinaId",
                principalSchema: "linealytics",
                principalTable: "MetricasMaquina",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
