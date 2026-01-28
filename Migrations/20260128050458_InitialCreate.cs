using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "planta");

            migrationBuilder.EnsureSchema(
                name: "authentication");

            migrationBuilder.EnsureSchema(
                name: "linealytics");

            migrationBuilder.EnsureSchema(
                name: "operadores");

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "planta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "authentication",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "authentication",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    IsLdapUser = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "DepartamentosOperador",
                schema: "operadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartamentosOperador", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                schema: "authentication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Icono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItems_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "authentication",
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "Operadores",
                schema: "operadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NumeroEmpleado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CodigoPinHashed = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TiempoCicloSegundos = table.Column<int>(type: "integer", nullable: false),
                    UnidadesPorCiclo = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Turnos",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DuracionMinutos = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lineas",
                schema: "planta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    AreaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lineas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalSchema: "planta",
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "authentication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "authentication",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "authentication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "authentication",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "authentication",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "authentication",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "authentication",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "authentication",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "authentication",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "authentication",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "authentication",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Botones",
                schema: "planta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DepartamentoOperadorId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    FechaUltimaActivacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Botones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Botones_DepartamentosOperador_DepartamentoOperadorId",
                        column: x => x.DepartamentoOperadorId,
                        principalSchema: "operadores",
                        principalTable: "DepartamentosOperador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MenuRolePermissions",
                schema: "authentication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuRolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "authentication",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuRolePermissions_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalSchema: "authentication",
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperadorDepartamentos",
                schema: "operadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OperadorId = table.Column<int>(type: "integer", nullable: false),
                    DepartamentoOperadorId = table.Column<int>(type: "integer", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperadorDepartamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperadorDepartamentos_DepartamentosOperador_DepartamentoOpe~",
                        column: x => x.DepartamentoOperadorId,
                        principalSchema: "operadores",
                        principalTable: "DepartamentosOperador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperadorDepartamentos_Operadores_OperadorId",
                        column: x => x.OperadorId,
                        principalSchema: "operadores",
                        principalTable: "Operadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estaciones",
                schema: "planta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    LineaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estaciones_Lineas_LineaId",
                        column: x => x.LineaId,
                        principalSchema: "planta",
                        principalTable: "Lineas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Maquinas",
                schema: "planta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NumeroSerie = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Fabricante = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    EstacionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maquinas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maquinas_Estaciones_EstacionId",
                        column: x => x.EstacionId,
                        principalSchema: "planta",
                        principalTable: "Estaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Botoneras",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DireccionIP = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NumeroSerie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Botoneras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Botoneras_Maquinas_MaquinaId",
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
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ContadorOKInicial = table.Column<long>(type: "bigint", nullable: false),
                    ContadorOKFinal = table.Column<long>(type: "bigint", nullable: false),
                    UltimoContadorOK = table.Column<long>(type: "bigint", nullable: false),
                    ProduccionOK = table.Column<long>(type: "bigint", nullable: false),
                    NumeroResetsOK = table.Column<int>(type: "integer", nullable: false),
                    ContadorNOKInicial = table.Column<long>(type: "bigint", nullable: false),
                    ContadorNOKFinal = table.Column<long>(type: "bigint", nullable: false),
                    UltimoContadorNOK = table.Column<long>(type: "bigint", nullable: false),
                    ProduccionNOK = table.Column<long>(type: "bigint", nullable: false),
                    NumeroResetsNOK = table.Column<int>(type: "integer", nullable: false),
                    NumeroLecturas = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorridasProduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorridasProduccion_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
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
                name: "Dispositivos",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CodigoDispositivo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TipoDispositivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UltimoContador = table.Column<int>(type: "integer", nullable: true),
                    FechaUltimaLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispositivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispositivos_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MetricasMaquina",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    TurnoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TiempoDisponibleMinutos = table.Column<int>(type: "integer", nullable: false),
                    TiempoProduccionMinutos = table.Column<int>(type: "integer", nullable: false),
                    TiempoParoMinutos = table.Column<int>(type: "integer", nullable: false),
                    UnidadesProducidas = table.Column<int>(type: "integer", nullable: false),
                    UnidadesDefectuosas = table.Column<int>(type: "integer", nullable: false),
                    UnidadesBuenas = table.Column<int>(type: "integer", nullable: false),
                    DisponibilidadPorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    RendimientoPorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    CalidadPorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    OeePorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Cerrada = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricasMaquina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetricasMaquina_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MetricasMaquina_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MetricasMaquina_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalSchema: "linealytics",
                        principalTable: "Turnos",
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
                    CatalogoFallaId = table.Column<int>(type: "integer", nullable: false),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraDeteccion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pendiente"),
                    TecnicoAsignadoId = table.Column<int>(type: "integer", nullable: true),
                    FechaHoraAtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaHoraResolucion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "integer", nullable: true),
                    AccionesTomadas = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosFallas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosFallas_CatalogoFallas_CatalogoFallaId",
                        column: x => x.CatalogoFallaId,
                        principalSchema: "linealytics",
                        principalTable: "CatalogoFallas",
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
                        name: "FK_RegistrosFallas_Operadores_TecnicoAsignadoId",
                        column: x => x.TecnicoAsignadoId,
                        principalSchema: "operadores",
                        principalTable: "Operadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SesionesProduccion",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    TurnoId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TiempoDisponibleMinutos = table.Column<int>(type: "integer", nullable: false),
                    TiempoProduccionMinutos = table.Column<int>(type: "integer", nullable: false),
                    TiempoParoMinutos = table.Column<int>(type: "integer", nullable: false),
                    UnidadesProducidas = table.Column<int>(type: "integer", nullable: false),
                    UnidadesDefectuosas = table.Column<int>(type: "integer", nullable: false),
                    UnidadesBuenas = table.Column<int>(type: "integer", nullable: false),
                    DisponibilidadPorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    RendimientoPorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    CalidadPorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    OeePorcentaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Cerrada = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionesProduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SesionesProduccion_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SesionesProduccion_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SesionesProduccion_Turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalSchema: "linealytics",
                        principalTable: "Turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    OperadorId = table.Column<int>(type: "integer", nullable: true),
                    BotonId = table.Column<int>(type: "integer", nullable: true),
                    BotoneraId = table.Column<int>(type: "integer", nullable: false),
                    FechaHoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    FechaHoraFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "integer", nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Abierto")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosParoBotonera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosParoBotonera_Botoneras_BotoneraId",
                        column: x => x.BotoneraId,
                        principalSchema: "linealytics",
                        principalTable: "Botoneras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosParoBotonera_Botones_BotonId",
                        column: x => x.BotonId,
                        principalSchema: "planta",
                        principalTable: "Botones",
                        principalColumn: "Id");
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
                    table.ForeignKey(
                        name: "FK_RegistrosParoBotonera_Operadores_OperadorId",
                        column: x => x.OperadorId,
                        principalSchema: "operadores",
                        principalTable: "Operadores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LecturasContador",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CorridaId = table.Column<int>(type: "integer", nullable: false),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    ContadorOK = table.Column<long>(type: "bigint", nullable: false),
                    ContadorOKAnterior = table.Column<long>(type: "bigint", nullable: true),
                    DiferenciaOK = table.Column<long>(type: "bigint", nullable: false),
                    ProduccionOK = table.Column<long>(type: "bigint", nullable: false),
                    EsResetOK = table.Column<bool>(type: "boolean", nullable: false),
                    ContadorNOK = table.Column<long>(type: "bigint", nullable: false),
                    ContadorNOKAnterior = table.Column<long>(type: "bigint", nullable: true),
                    DiferenciaNOK = table.Column<long>(type: "bigint", nullable: false),
                    ProduccionNOK = table.Column<long>(type: "bigint", nullable: false),
                    EsResetNOK = table.Column<bool>(type: "boolean", nullable: false),
                    FechaHoraLectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LecturasContador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LecturasContador_CorridasProduccion_CorridaId",
                        column: x => x.CorridaId,
                        principalSchema: "linealytics",
                        principalTable: "CorridasProduccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LecturasContador_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LecturasContador_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosParos",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaquinaId = table.Column<int>(type: "integer", nullable: false),
                    MetricasMaquinaId = table.Column<int>(type: "integer", nullable: true),
                    CausaParoId = table.Column<int>(type: "integer", nullable: true),
                    FechaHoraInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaHoraFin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DuracionMinutos = table.Column<int>(type: "integer", nullable: true),
                    OperadorResponsableId = table.Column<int>(type: "integer", nullable: true),
                    OperadorSolucionaId = table.Column<int>(type: "integer", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Solucion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EsMicroParo = table.Column<bool>(type: "boolean", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaAtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SesionProduccionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosParos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosParos_Maquinas_MaquinaId",
                        column: x => x.MaquinaId,
                        principalSchema: "planta",
                        principalTable: "Maquinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosParos_MetricasMaquina_MetricasMaquinaId",
                        column: x => x.MetricasMaquinaId,
                        principalSchema: "linealytics",
                        principalTable: "MetricasMaquina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosParos_Operadores_OperadorResponsableId",
                        column: x => x.OperadorResponsableId,
                        principalSchema: "operadores",
                        principalTable: "Operadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosParos_Operadores_OperadorSolucionaId",
                        column: x => x.OperadorSolucionaId,
                        principalSchema: "operadores",
                        principalTable: "Operadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosParos_SesionesProduccion_SesionProduccionId",
                        column: x => x.SesionProduccionId,
                        principalSchema: "linealytics",
                        principalTable: "SesionesProduccion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RegistrosProduccionHora",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SesionProduccionId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnidadesProducidas = table.Column<int>(type: "integer", nullable: false),
                    UnidadesDefectuosas = table.Column<int>(type: "integer", nullable: false),
                    UnidadesBuenas = table.Column<int>(type: "integer", nullable: false),
                    TiempoProduccionMinutos = table.Column<int>(type: "integer", nullable: false),
                    TiempoParoMinutos = table.Column<int>(type: "integer", nullable: false),
                    OperadorId = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosProduccionHora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosProduccionHora_Operadores_OperadorId",
                        column: x => x.OperadorId,
                        principalSchema: "operadores",
                        principalTable: "Operadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosProduccionHora_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalSchema: "linealytics",
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosProduccionHora_SesionesProduccion_SesionProduccion~",
                        column: x => x.SesionProduccionId,
                        principalSchema: "linealytics",
                        principalTable: "SesionesProduccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "HistorialCambiosParos",
                schema: "linealytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistroParoId = table.Column<int>(type: "integer", nullable: false),
                    CampoModificado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ValorAnterior = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ValorNuevo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UsuarioModifica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCambiosParos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialCambiosParos_RegistrosParos_RegistroParoId",
                        column: x => x.RegistroParoId,
                        principalSchema: "linealytics",
                        principalTable: "RegistrosParos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_Codigo",
                schema: "planta",
                table: "Areas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "authentication",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "authentication",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "authentication",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "authentication",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "authentication",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "authentication",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "authentication",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Botoneras_DireccionIP",
                schema: "linealytics",
                table: "Botoneras",
                column: "DireccionIP",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Botoneras_MaquinaId",
                schema: "linealytics",
                table: "Botoneras",
                column: "MaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_Botoneras_NumeroSerie",
                schema: "linealytics",
                table: "Botoneras",
                column: "NumeroSerie",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Botones_Codigo",
                schema: "planta",
                table: "Botones",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Botones_DepartamentoOperadorId",
                schema: "planta",
                table: "Botones",
                column: "DepartamentoOperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogoFallas_Codigo",
                schema: "linealytics",
                table: "CatalogoFallas",
                column: "Codigo",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_CorridasProduccion_ProductoId",
                schema: "linealytics",
                table: "CorridasProduccion",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartamentosOperador_Nombre",
                schema: "operadores",
                table: "DepartamentosOperador",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositivos_MaquinaId_CodigoDispositivo",
                schema: "linealytics",
                table: "Dispositivos",
                columns: new[] { "MaquinaId", "CodigoDispositivo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estaciones_Codigo",
                schema: "planta",
                table: "Estaciones",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estaciones_LineaId",
                schema: "planta",
                table: "Estaciones",
                column: "LineaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCambiosParos_RegistroParoId",
                schema: "linealytics",
                table: "HistorialCambiosParos",
                column: "RegistroParoId");

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
                name: "IX_LecturasContador_ProductoId",
                schema: "linealytics",
                table: "LecturasContador",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineas_AreaId",
                schema: "planta",
                table: "Lineas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineas_Codigo",
                schema: "planta",
                table: "Lineas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maquinas_Codigo",
                schema: "planta",
                table: "Maquinas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maquinas_EstacionId",
                schema: "planta",
                table: "Maquinas",
                column: "EstacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Maquinas_NumeroSerie",
                schema: "planta",
                table: "Maquinas",
                column: "NumeroSerie",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Orden",
                schema: "authentication",
                table: "MenuItems",
                column: "Orden");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ParentId",
                schema: "authentication",
                table: "MenuItems",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuRolePermissions_MenuItemId_RoleId",
                schema: "authentication",
                table: "MenuRolePermissions",
                columns: new[] { "MenuItemId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuRolePermissions_RoleId",
                schema: "authentication",
                table: "MenuRolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricasMaquina_MaquinaId_FechaInicio",
                schema: "linealytics",
                table: "MetricasMaquina",
                columns: new[] { "MaquinaId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_MetricasMaquina_ProductoId",
                schema: "linealytics",
                table: "MetricasMaquina",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_MetricasMaquina_TurnoId",
                schema: "linealytics",
                table: "MetricasMaquina",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Modelos_Codigo",
                schema: "planta",
                table: "Modelos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperadorDepartamentos_DepartamentoOperadorId",
                schema: "operadores",
                table: "OperadorDepartamentos",
                column: "DepartamentoOperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_OperadorDepartamentos_OperadorId_DepartamentoOperadorId",
                schema: "operadores",
                table: "OperadorDepartamentos",
                columns: new[] { "OperadorId", "DepartamentoOperadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operadores_NumeroEmpleado",
                schema: "operadores",
                table: "Operadores",
                column: "NumeroEmpleado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Codigo",
                schema: "linealytics",
                table: "Productos",
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
                name: "IX_RegistrosFallas_MaquinaId_FechaHoraDeteccion",
                schema: "linealytics",
                table: "RegistrosFallas",
                columns: new[] { "MaquinaId", "FechaHoraDeteccion" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFallas_TecnicoAsignadoId",
                schema: "linealytics",
                table: "RegistrosFallas",
                column: "TecnicoAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_BotoneraId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "BotoneraId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_BotonId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "BotonId");

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

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParoBotonera_OperadorId",
                schema: "linealytics",
                table: "RegistrosParoBotonera",
                column: "OperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParos_Estado",
                schema: "linealytics",
                table: "RegistrosParos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParos_MaquinaId_FechaHoraInicio",
                schema: "linealytics",
                table: "RegistrosParos",
                columns: new[] { "MaquinaId", "FechaHoraInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParos_MetricasMaquinaId",
                schema: "linealytics",
                table: "RegistrosParos",
                column: "MetricasMaquinaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParos_OperadorResponsableId",
                schema: "linealytics",
                table: "RegistrosParos",
                column: "OperadorResponsableId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParos_OperadorSolucionaId",
                schema: "linealytics",
                table: "RegistrosParos",
                column: "OperadorSolucionaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosParos_SesionProduccionId",
                schema: "linealytics",
                table: "RegistrosParos",
                column: "SesionProduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosProduccionHora_OperadorId",
                schema: "linealytics",
                table: "RegistrosProduccionHora",
                column: "OperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosProduccionHora_ProductoId",
                schema: "linealytics",
                table: "RegistrosProduccionHora",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosProduccionHora_SesionProduccionId_FechaHora",
                schema: "linealytics",
                table: "RegistrosProduccionHora",
                columns: new[] { "SesionProduccionId", "FechaHora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SesionesProduccion_MaquinaId_FechaInicio",
                schema: "linealytics",
                table: "SesionesProduccion",
                columns: new[] { "MaquinaId", "FechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_SesionesProduccion_ProductoId",
                schema: "linealytics",
                table: "SesionesProduccion",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_SesionesProduccion_TurnoId",
                schema: "linealytics",
                table: "SesionesProduccion",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_Nombre",
                schema: "linealytics",
                table: "Turnos",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "ComentariosParoBotonera",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "Dispositivos",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "HistorialCambiosParos",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "LecturasContador",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "MenuRolePermissions",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "OperadorDepartamentos",
                schema: "operadores");

            migrationBuilder.DropTable(
                name: "RegistrosContadores",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "RegistrosFallas",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "RegistrosProduccionHora",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "RegistrosParoBotonera",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "RegistrosParos",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "CorridasProduccion",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "MenuItems",
                schema: "authentication");

            migrationBuilder.DropTable(
                name: "Modelos",
                schema: "planta");

            migrationBuilder.DropTable(
                name: "CatalogoFallas",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "Botoneras",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "Botones",
                schema: "planta");

            migrationBuilder.DropTable(
                name: "MetricasMaquina",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "Operadores",
                schema: "operadores");

            migrationBuilder.DropTable(
                name: "SesionesProduccion",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "DepartamentosOperador",
                schema: "operadores");

            migrationBuilder.DropTable(
                name: "Maquinas",
                schema: "planta");

            migrationBuilder.DropTable(
                name: "Productos",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "Turnos",
                schema: "linealytics");

            migrationBuilder.DropTable(
                name: "Estaciones",
                schema: "planta");

            migrationBuilder.DropTable(
                name: "Lineas",
                schema: "planta");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "planta");
        }
    }
}
