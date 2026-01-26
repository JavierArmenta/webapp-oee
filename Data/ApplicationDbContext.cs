using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Models.Linealytics;

namespace WebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet para Operadores
        public DbSet<Operador> Operadores { get; set; }

        // DbSet para DepartamentosOperador
        public DbSet<DepartamentoOperador> DepartamentosOperador { get; set; }

        // DbSet para la tabla intermedia
        public DbSet<OperadorDepartamento> OperadorDepartamentos { get; set; }

        // DbSet para Planta
        public DbSet<Area> Areas { get; set; }
        public DbSet<Linea> Lineas { get; set; }
        public DbSet<Estacion> Estaciones { get; set; }
        public DbSet<Maquina> Maquinas { get; set; }
        public DbSet<Boton> Botones { get; set; }
        public DbSet<Modelo> Modelos { get; set; }

        // DbSet para Linealytics
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CategoriaParo> CategoriasParo { get; set; }
        public DbSet<CausaParo> CausasParo { get; set; }
        public DbSet<MetricasMaquina> MetricasMaquina { get; set; }
        public DbSet<SesionProduccion> SesionesProduccion { get; set; }
        public DbSet<RegistroParo> RegistrosParos { get; set; }
        public DbSet<RegistroProduccionHora> RegistrosProduccionHora { get; set; }
        public DbSet<HistorialCambioParo> HistorialCambiosParos { get; set; }

        // DbSet para Nuevo Sistema Linealytics
        public DbSet<Botonera> Botoneras { get; set; }
        public DbSet<RegistroParoBotonera> RegistrosParoBotonera { get; set; }
        public DbSet<RegistroFalla> RegistrosFallas { get; set; }
        public DbSet<RegistroContador> RegistrosContadores { get; set; }
        public DbSet<ComentarioParoBotonera> ComentariosParoBotonera { get; set; }

        // DbSet para Sistema de Contadores con Corridas
        public DbSet<CorridaProduccion> CorridasProduccion { get; set; }
        public DbSet<LecturaContador> LecturasContador { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurar explícitamente cada tabla de Identity en el schema "authentication"
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers", "authentication");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles", "authentication");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AspNetUserRoles", "authentication");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("AspNetUserClaims", "authentication");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AspNetUserLogins", "authentication");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("AspNetRoleClaims", "authentication");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("AspNetUserTokens", "authentication");
            });

            // Configuración para Operadores en schema "operadores"
            builder.Entity<Operador>(entity =>
            {
                entity.ToTable("Operadores", "operadores");

                entity.HasIndex(e => e.NumeroEmpleado)
                    .IsUnique()
                    .HasDatabaseName("IX_Operadores_NumeroEmpleado");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para DepartamentosOperador en schema "operadores"
            builder.Entity<DepartamentoOperador>(entity =>
            {
                entity.ToTable("DepartamentosOperador", "operadores");

                entity.HasIndex(e => e.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_DepartamentosOperador_Nombre");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para la tabla intermedia en schema "operadores"
            builder.Entity<OperadorDepartamento>(entity =>
            {
                entity.ToTable("OperadorDepartamentos", "operadores");

                entity.HasIndex(e => new { e.OperadorId, e.DepartamentoOperadorId })
                    .IsUnique()
                    .HasDatabaseName("IX_OperadorDepartamentos_OperadorId_DepartamentoOperadorId");

                entity.Property(e => e.FechaAsignacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Operador)
                    .WithMany(o => o.OperadorDepartamentos)
                    .HasForeignKey(e => e.OperadorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.DepartamentoOperador)
                    .WithMany(d => d.OperadorDepartamentos)
                    .HasForeignKey(e => e.DepartamentoOperadorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración para Areas en schema "planta"
            builder.Entity<Area>(entity =>
            {
                entity.ToTable("Areas", "planta");

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Areas_Codigo");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para Lineas en schema "planta"
            builder.Entity<Linea>(entity =>
            {
                entity.ToTable("Lineas", "planta");

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Lineas_Codigo");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Area)
                    .WithMany(a => a.Lineas)
                    .HasForeignKey(e => e.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para Estaciones en schema "planta"
            builder.Entity<Estacion>(entity =>
            {
                entity.ToTable("Estaciones", "planta");

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Estaciones_Codigo");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Linea)
                    .WithMany(l => l.Estaciones)
                    .HasForeignKey(e => e.LineaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para Maquinas en schema "planta"
            builder.Entity<Maquina>(entity =>
            {
                entity.ToTable("Maquinas", "planta");

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Maquinas_Codigo");

                entity.HasIndex(e => e.NumeroSerie)
                    .IsUnique()
                    .HasDatabaseName("IX_Maquinas_NumeroSerie");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Estacion)
                    .WithMany(es => es.Maquinas)
                    .HasForeignKey(e => e.EstacionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para Botones en schema "planta"
            builder.Entity<Boton>(entity =>
            {
                entity.ToTable("Botones", "planta");

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Botones_Codigo");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.DepartamentoOperador)
                    .WithMany()
                    .HasForeignKey(e => e.DepartamentoOperadorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para Modelos en schema "planta"
            builder.Entity<Modelo>(entity =>
            {
                entity.ToTable("Modelos", "planta");

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Modelos_Codigo");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // ========== CONFIGURACIÓN LINEALYTICS ==========

            // Configuración para Turnos
            builder.Entity<Turno>(entity =>
            {
                entity.ToTable("Turnos", "linealytics");

                entity.HasIndex(e => e.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_Turnos_Nombre");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para Productos
            builder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos", "linealytics");

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("IX_Productos_Codigo");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para CategoriasParo
            builder.Entity<CategoriaParo>(entity =>
            {
                entity.ToTable("CategoriasParo", "linealytics");

                entity.HasIndex(e => e.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_CategoriasParo_Nombre");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para CausasParo
            builder.Entity<CausaParo>(entity =>
            {
                entity.ToTable("CausasParo", "linealytics");

                entity.HasIndex(e => new { e.CategoriaParoId, e.Nombre })
                    .IsUnique()
                    .HasDatabaseName("IX_CausasParo_CategoriaParoId_Nombre");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.CategoriaParo)
                    .WithMany(c => c.CausasParo)
                    .HasForeignKey(e => e.CategoriaParoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para MetricasMaquina
            builder.Entity<MetricasMaquina>(entity =>
            {
                entity.ToTable("MetricasMaquina", "linealytics");

                entity.HasIndex(e => new { e.MaquinaId, e.FechaInicio })
                    .HasDatabaseName("IX_MetricasMaquina_MaquinaId_FechaInicio");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Turno)
                    .WithMany()
                    .HasForeignKey(e => e.TurnoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para SesionesProduccion
            builder.Entity<SesionProduccion>(entity =>
            {
                entity.ToTable("SesionesProduccion", "linealytics");

                entity.HasIndex(e => new { e.MaquinaId, e.FechaInicio })
                    .HasDatabaseName("IX_SesionesProduccion_MaquinaId_FechaInicio");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Turno)
                    .WithMany(t => t.SesionesProduccion)
                    .HasForeignKey(e => e.TurnoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.SesionesProduccion)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para RegistrosParos
            builder.Entity<RegistroParo>(entity =>
            {
                entity.ToTable("RegistrosParos", "linealytics");

                entity.HasIndex(e => new { e.MaquinaId, e.FechaHoraInicio })
                    .HasDatabaseName("IX_RegistrosParos_MaquinaId_FechaHoraInicio");

                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_RegistrosParos_Estado");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.MetricasMaquina)
                    .WithMany(m => m.RegistrosParos)
                    .HasForeignKey(e => e.MetricasMaquinaId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.CausaParo)
                    .WithMany(c => c.RegistrosParos)
                    .HasForeignKey(e => e.CausaParoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.OperadorResponsable)
                    .WithMany()
                    .HasForeignKey(e => e.OperadorResponsableId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.OperadorSoluciona)
                    .WithMany()
                    .HasForeignKey(e => e.OperadorSolucionaId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración para RegistrosProduccionHora
            builder.Entity<RegistroProduccionHora>(entity =>
            {
                entity.ToTable("RegistrosProduccionHora", "linealytics");

                entity.HasIndex(e => new { e.SesionProduccionId, e.FechaHora })
                    .IsUnique()
                    .HasDatabaseName("IX_RegistrosProduccionHora_SesionProduccionId_FechaHora");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.SesionProduccion)
                    .WithMany(s => s.RegistrosProduccionHora)
                    .HasForeignKey(e => e.SesionProduccionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.RegistrosProduccionHora)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Operador)
                    .WithMany()
                    .HasForeignKey(e => e.OperadorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración para HistorialCambiosParos
            builder.Entity<HistorialCambioParo>(entity =>
            {
                entity.ToTable("HistorialCambiosParos", "linealytics");

                entity.HasIndex(e => e.RegistroParoId)
                    .HasDatabaseName("IX_HistorialCambiosParos_RegistroParoId");

                entity.Property(e => e.FechaModificacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.RegistroParo)
                    .WithMany(r => r.HistorialCambios)
                    .HasForeignKey(e => e.RegistroParoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración para Dispositivos
            builder.Entity<Dispositivo>(entity =>
            {
                entity.ToTable("Dispositivos", "linealytics");

                entity.HasIndex(e => new { e.MaquinaId, e.CodigoDispositivo })
                    .IsUnique()
                    .HasDatabaseName("IX_Dispositivos_MaquinaId_CodigoDispositivo");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ========== CONFIGURACIÓN NUEVO SISTEMA LINEALYTICS ==========

            // Configuración para Botoneras
            builder.Entity<Botonera>(entity =>
            {
                entity.ToTable("Botoneras", "linealytics");

                entity.HasIndex(e => e.DireccionIP)
                    .IsUnique()
                    .HasDatabaseName("IX_Botoneras_DireccionIP");

                entity.HasIndex(e => e.NumeroSerie)
                    .IsUnique()
                    .HasDatabaseName("IX_Botoneras_NumeroSerie");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para RegistrosParoBotonera
            builder.Entity<RegistroParoBotonera>(entity =>
            {
                entity.ToTable("RegistrosParoBotonera", "linealytics");

                entity.HasIndex(e => new { e.MaquinaId, e.FechaHoraInicio })
                    .HasDatabaseName("IX_RegistrosParoBotonera_MaquinaId_FechaHoraInicio");

                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_RegistrosParoBotonera_Estado");

                entity.Property(e => e.FechaHoraInicio)
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.Estado)
                    .HasDefaultValue("Abierto");

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.DepartamentoOperador)
                    .WithMany()
                    .HasForeignKey(e => e.DepartamentoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para RegistrosFallas
            builder.Entity<RegistroFalla>(entity =>
            {
                entity.ToTable("RegistrosFallas", "linealytics");

                entity.HasIndex(e => new { e.MaquinaId, e.FechaHoraLectura })
                    .HasDatabaseName("IX_RegistrosFallas_MaquinaId_FechaHoraLectura");

                entity.HasIndex(e => e.FallaId)
                    .HasDatabaseName("IX_RegistrosFallas_FallaId");

                entity.Property(e => e.FechaHoraLectura)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CausaParo)
                    .WithMany()
                    .HasForeignKey(e => e.FallaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Modelo)
                    .WithMany()
                    .HasForeignKey(e => e.ModeloId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración para RegistrosContadores
            builder.Entity<RegistroContador>(entity =>
            {
                entity.ToTable("RegistrosContadores", "linealytics");

                entity.HasIndex(e => new { e.MaquinaId, e.FechaHoraLectura })
                    .HasDatabaseName("IX_RegistrosContadores_MaquinaId_FechaHoraLectura");

                entity.Property(e => e.FechaHoraLectura)
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.ContadorOK)
                    .HasDefaultValue(0);

                entity.Property(e => e.ContadorNOK)
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Modelo)
                    .WithMany()
                    .HasForeignKey(e => e.ModeloId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración para ComentariosParoBotonera
            builder.Entity<ComentarioParoBotonera>(entity =>
            {
                entity.ToTable("ComentariosParoBotonera", "linealytics");

                entity.Property(e => e.Comentario)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.RegistroParoBotonera)
                    .WithMany(p => p.Comentarios)
                    .HasForeignKey(e => e.RegistroParoBotoneraId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.RegistroParoBotoneraId)
                    .HasDatabaseName("IX_ComentariosParoBotonera_RegistroParoBotoneraId");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("IX_ComentariosParoBotonera_UserId");
            });

            // ========== SISTEMA DE CORRIDAS DE PRODUCCIÓN ==========

            // Configuración de CorridaProduccion
            builder.Entity<CorridaProduccion>(entity =>
            {
                entity.ToTable("CorridasProduccion", "linealytics");

                entity.Property(e => e.Estado)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.MaquinaId, e.Estado })
                    .HasDatabaseName("IX_CorridasProduccion_MaquinaId_Estado");

                entity.HasIndex(e => new { e.MaquinaId, e.FechaInicio })
                    .HasDatabaseName("IX_CorridasProduccion_MaquinaId_FechaInicio");
            });

            // Configuración de LecturaContador
            builder.Entity<LecturaContador>(entity =>
            {
                entity.ToTable("LecturasContador", "linealytics");

                entity.HasOne(e => e.Corrida)
                    .WithMany(c => c.Lecturas)
                    .HasForeignKey(e => e.CorridaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Maquina)
                    .WithMany()
                    .HasForeignKey(e => e.MaquinaId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.MaquinaId, e.FechaHoraLectura })
                    .HasDatabaseName("IX_LecturasContador_MaquinaId_Fecha");

                entity.HasIndex(e => new { e.CorridaId, e.FechaHoraLectura })
                    .HasDatabaseName("IX_LecturasContador_CorridaId_Fecha");
            });
        }
    }
}
