using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

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
        
        // DbSet para RolesOperador
        public DbSet<RolOperador> RolesOperador { get; set; }
        
        // DbSet para la tabla intermedia
        public DbSet<OperadorRolOperador> OperadorRolesOperador { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // IMPORTANTE: Configurar cada tabla de Identity DESPUÉS de base.OnModelCreating
            
            // Usuarios
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers", "authentication");
            });

            // Roles
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("AspNetRoles", "authentication");
            });

            // UserRoles
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AspNetUserRoles", "authentication");
            });

            // UserClaims
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("AspNetUserClaims", "authentication");
            });

            // UserLogins
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AspNetUserLogins", "authentication");
            });

            // RoleClaims
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("AspNetRoleClaims", "authentication");
            });

            // UserTokens
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("AspNetUserTokens", "authentication");
            });

            // Configuración para Operadores en schema "authentication"
            builder.Entity<Operador>(entity =>
            {
                entity.ToTable("Operadores", "operadores");
                
                entity.HasIndex(e => e.NumeroEmpleado)
                    .IsUnique()
                    .HasDatabaseName("IX_Operadores_NumeroEmpleado");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para RolesOperador en schema "authentication"
            builder.Entity<RolOperador>(entity =>
            {
                entity.ToTable("RolesOperador", "operadores");
                
                entity.HasIndex(e => e.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_RolesOperador_Nombre");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("NOW()");
            });

            // Configuración para la tabla intermedia en schema "authentication"
            builder.Entity<OperadorRolOperador>(entity =>
            {
                entity.ToTable("OperadorRolesOperador", "operadores");

                // Índice compuesto único para evitar duplicados
                entity.HasIndex(e => new { e.OperadorId, e.RolOperadorId })
                    .IsUnique()
                    .HasDatabaseName("IX_OperadorRolesOperador_OperadorId_RolOperadorId");

                entity.Property(e => e.FechaAsignacion)
                    .HasDefaultValueSql("NOW()");

                // Configurar relaciones
                entity.HasOne(e => e.Operador)
                    .WithMany(o => o.OperadorRoles)
                    .HasForeignKey(e => e.OperadorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.RolOperador)
                    .WithMany(r => r.OperadorRoles)
                    .HasForeignKey(e => e.RolOperadorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}