using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using System.Diagnostics;
using WebApp.Models;
using WebApp.Services;
using DotNetEnv; // 👈 para leer .env
using Microsoft.AspNetCore.HttpOverrides; // (opcional) si irás detrás de Nginx/IIS

var builder = WebApplication.CreateBuilder(args);

// 👇 Hacer que Kestrel escuche en todas las IPs (0.0.0.0) en el puerto 5000
builder.WebHost.UseKestrel();
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Cargar variables desde .env
Env.Load();

// Leer cadena de conexión desde variable de entorno
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

// Configurar DbContext con PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString,
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "authentication")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();

// Registrar servicio LDAP
builder.Services.AddScoped<ILdapService, LdapService>();
// Registrar servicio de Operadores
builder.Services.AddScoped<IOperadorService, OperadorService>();
// Registrar servicio de Menús dinámicos
builder.Services.AddScoped<IMenuService, MenuService>();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ❌ DESPUÉS DE ESTA LÍNEA NO SE PUEDEN AGREGAR MÁS SERVICIOS
var app = builder.Build();

// (Opcional) si estás detrás de Nginx/IIS con TLS terminado en el proxy:
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Seed roles/usuarios/menús
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Ejecutando Seed...");
        Seed.SeedDB(userManager, roleManager, dbContext);
        Console.WriteLine("Seed ejecutado correctamente.");
    }
    catch (Exception ex)
    {
        Debug.WriteLine(ex.Message);
        Console.WriteLine(ex.Message);
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // 👈 necesario cuando usas Identity
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();