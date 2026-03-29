# WebApp Claude - Sistema de Gestión Industrial

## Información General

**WebApp Claude** es una aplicación web empresarial ASP.NET Core MVC diseñada para la gestión integral de plantas industriales, con un sistema avanzado de análisis de métricas de producción llamado **Linealytics**.

### Stack Tecnológico
- **Framework:** .NET 8.0 / ASP.NET Core MVC
- **Base de Datos:** PostgreSQL (con soporte para SQLite y SQL Server)
- **ORM:** Entity Framework Core 8.0
- **Autenticación:** ASP.NET Core Identity + LDAP/Active Directory
- **Patrón Arquitectónico:** MVC multicapa con capa de servicios

## Arquitectura del Sistema

### Estructura de Capas

```
┌─────────────────────────────────────┐
│         Capa de Presentación        │
│    (Controllers + Views Razor)      │
├─────────────────────────────────────┤
│       Capa de Lógica de Negocio     │
│           (Services)                │
├─────────────────────────────────────┤
│        Capa de Acceso a Datos       │
│   (ApplicationDbContext + Models)   │
├─────────────────────────────────────┤
│          Base de Datos              │
│         (PostgreSQL)                │
└─────────────────────────────────────┘
```

### Estructura de Directorios

```
webapp_claude/
├── Controllers/          # 7 controladores MVC
├── Models/              # 28+ modelos de datos + ViewModels
├── Views/               # 40+ vistas Razor
├── Services/            # 3 servicios de lógica de negocio
├── Data/                # Contexto Entity Framework Core
├── Enums/               # Enumeraciones (Roles)
├── Areas/               # ASP.NET Identity páginas
├── Migrations/          # Migraciones EF Core
├── wwwroot/             # Archivos estáticos (JS, CSS, assets)
├── Program.cs           # Configuración y arranque
└── WebApp.csproj        # Definición del proyecto
```

## Módulos Principales

### 1. Módulo de Administración
- Gestión de usuarios del sistema
- Asignación de roles (SuperAdmin, Admin, User)
- Desactivación lógica de usuarios
- Actualización de perfiles de usuario

**Controlador:** [AdminController.cs](Controllers/AdminController.cs)

### 2. Módulo de Operadores
- CRUD completo de operadores
- Gestión de departamentos de operadores
- Asignación de múltiples departamentos por operador
- Sistema de códigos PIN hasheados (PBKDF2)
- Búsqueda y filtrado de operadores

**Controlador:** [OperadoresController.cs](Controllers/OperadoresController.cs)

### 3. Módulo de Infraestructura (Planta)

Gestión jerárquica de la infraestructura industrial:

```
Área
 └── Línea
      └── Estación
           └── Máquina
                └── Botonera
```

Cada nivel incluye operaciones CRUD completas con vistas de detalle.

**Controlador:** [PlantaController.cs](Controllers/PlantaController.cs)

### 4. Módulo Linealytics (Sistema de Métricas y OEE)

Sistema avanzado de análisis de producción en tiempo real. Los datos crudos son capturados por `api-oee` desde hardware (botoneras, PLCs); `webapp-oee` los agrega y calcula las métricas.

#### Cálculo OEE — `OeeCalculationService`

Service registrado como `Scoped` que implementa el cálculo completo de OEE:

| Componente | Fórmula |
|-----------|---------|
| **Disponibilidad** | `T_produccion / T_disponible × 100` |
| **Rendimiento** | `P_total / (T_produccion_min × 60 / C_prom_seg) × 100` |
| **Calidad** | `P_OK / P_total × 100` |
| **OEE** | `Disponibilidad × Rendimiento × Calidad` |

Donde:
- `T_disponible` = duración del turno (o rango libre elegido por el usuario)
- `T_produccion` = `T_disponible` − tiempo en paro (paros recortados al rango, incluye paros abiertos usando `UtcNow`)
- `P_OK / P_total` = acumulados de `CorridasProduccion` activas en la ventana
- `C_prom_seg` = promedio ponderado de `Producto.TiempoCicloSegundos` por unidades producidas
- Soporta turnos nocturnos (HoraFin < HoraInicio → ventana cruza medianoche)
- Upsert en `MetricasMaquina` al guardar (busca por MaquinaId + TurnoId + FechaInicio antes de crear)

#### Componentes del módulo
- Vista OEE en tiempo real (`/Linealytics/Oee`) con gauges Chart.js y polling automático cada 30 s
- Tab "Turno Actual": calcula sin guardar, actualización en vivo
- Tab "Histórico": calcula y guarda (por turno + fecha, o por rango libre)
- Tabla historial de `MetricasMaquina` (últimos 30 días)
- Dashboard de paros y contadores (`/Linealytics/Dashboard`)
- Gestión de turnos, productos, botoneras, botones
- Historial de paros con comentarios (`ParosSinComentar`)
- Corridas de producción con detección de resets de contador

**Controlador:** [LinealyticsController.cs](Controllers/LinealyticsController.cs)
**Servicio:** [Services/OeeCalculationService.cs](Services/OeeCalculationService.cs)

### 5. Módulo de Autenticación

Sistema dual de autenticación:
- **Local:** Usuarios almacenados en base de datos
- **LDAP/Active Directory:** Integración con directorio empresarial

Funcionalidades:
- Registro de usuarios
- Login con validación
- Recuperación de contraseña
- Gestión de perfil
- Logout seguro

**Páginas:** [Areas/Identity/Pages/Account/](Areas/Identity/Pages/Account/)

## Modelo de Datos

### Esquemas de PostgreSQL

El sistema utiliza 4 esquemas para organizar las tablas:

1. **authentication** - Tablas de ASP.NET Identity
2. **operadores** - Datos de operadores y departamentos
3. **planta** - Infraestructura (áreas, líneas, estaciones, máquinas)
4. **linealytics** - Sistema de métricas y monitoreo

### Principales Entidades

#### Autenticación
- **ApplicationUser** - Usuario extendido con FirstName, LastName, IsLdapUser, IsActive

#### Operadores
- **Operador** - Operario (Id, Nombre, Apellido, NumeroEmpleado, CodigoPinHashed)
- **DepartamentoOperador** - Departamentos/Roles de operarios
- **OperadorDepartamento** - Relación muchos a muchos
- **Boton** - Botones vinculados a departamentos

#### Infraestructura (Planta)
- **Area** - Áreas de la planta
- **Linea** - Líneas de producción
- **Estacion** - Estaciones dentro de líneas
- **Maquina** - Máquinas en estaciones
- **Modelo** - Modelos de máquinas

#### Linealytics (Sistema de Métricas)
- **Turno** - Turnos laborales (`Nombre`, `HoraInicio`, `HoraFin`, `DuracionMinutos`)
- **Producto** - Productos fabricados (`TiempoCicloSegundos` usado para calcular Rendimiento)
- **MetricasMaquina** - Snapshot OEE por máquina+turno (escrito por `OeeCalculationService`)
- **SesionProduccion** - Sesiones de producción (schema, pendiente de poblar)
- **RegistroParo** - Paros enriquecidos (schema, no usa api-oee actualmente)
- **RegistroProduccionHora** - Producción horaria (schema)
- **HistorialCambioParo** - Auditoría de cambios en paros
- **Botonera** - Hardware de botoneras IP (gestionado desde webapp)
- **RegistroParoBotonera** - Paros reales capturados por api-oee desde botoneras físicas
- **CorridaProduccion** - Corrida activa por máquina/producto; acumula `ProduccionOK/NOK` con detección de resets
- **LecturaContador** - Lectura individual de contador, vinculada a una `CorridaProduccion`
- **RegistroFalla** - Fallas detectadas por PLCs (vía api-oee)
- **CatalogoFalla** - Tipos de falla con severidad y tiempo estimado de solución
- **ComentarioParoBotonera** - Comentarios de supervisores en paros cerrados

**Archivo DbContext:** [Data/ApplicationDbContext.cs](Data/ApplicationDbContext.cs) (52 DbSets)

## Servicios (Capa de Negocio)

### 1. LdapService
**Archivo:** [Services/LdapService.cs](Services/LdapService.cs)

Integración con Active Directory/LDAP:
- `GetUserByUsernameAsync()` - Obtiene usuario del directorio
- `SearchUsersAsync()` - Búsqueda de usuarios
- `ValidateCredentialsAsync()` - Valida credenciales contra AD

**Características:**
- Compatible con Linux y Windows
- Soporte SSL/TLS
- Escape de caracteres LDAP para prevenir inyección
- Usa `System.DirectoryServices.Protocols`

### 2. OperadorService
**Archivo:** [Services/OperadorService.cs](Services/OperadorService.cs)

Gestión de seguridad de operadores:
- `HashPin()` - Genera hash seguro de PIN usando PBKDF2
- `VerifyPin()` - Verifica PIN contra hash

**Seguridad:**
- PBKDF2 con HMACSHA256
- 10,000 iteraciones
- Salt aleatorio de 128 bits

### 3. OeeCalculationService
**Archivo:** [Services/OeeCalculationService.cs](Services/OeeCalculationService.cs)

Cálculo OEE bajo demanda o en vivo:
- `CalcularPorTurnoAsync(maquinaId, turnoId, fecha, guardar)` — ventana definida por `Turno.HoraInicio/HoraFin` + fecha; soporta turnos nocturnos
- `CalcularPorRangoAsync(maquinaId, fechaInicio, fechaFin, guardar)` — ventana libre

Cuando `guardar = true`, hace upsert en `MetricasMaquina`. Retorna `OeeResultDto` con todos los indicadores y flags de calidad de dato (`SinDatosProduccion`, `SinTiempoCiclo`).

### 4. Seed Service
**Archivo:** [Services/Seed.cs](Services/Seed.cs)

Inicialización de datos:
- `SeedRoles()` - Crea roles del sistema (SuperAdmin, Admin, User)
- `SeedGodAdmin()` - Crea usuario administrador inicial

Se ejecuta automáticamente en [Program.cs](Program.cs) al iniciar la aplicación.

## Seguridad

### Autenticación
- **ASP.NET Core Identity** para gestión de usuarios local
- **LDAP/Active Directory** para autenticación empresarial
- Soporte de autenticación dual simultánea

### Autorización
- Sistema de roles basado en Claims
- Roles disponibles: SuperAdmin, Administrator, User
- Protección de rutas con `[Authorize(Roles = "...")]`

### Hash y Encriptación
- **Contraseñas de usuarios:** ASP.NET Identity (PBKDF2)
- **PINs de operadores:** PBKDF2-HMACSHA256 con 10,000 iteraciones
- **Salt único** por cada PIN/contraseña

### Protecciones
- **SQL Injection:** Protegido por Entity Framework Core
- **CSRF:** Tokens anti-forgery en formularios
- **LDAP Injection:** Escape de caracteres especiales
- **XSS:** Razor automáticamente codifica HTML
- **Borrado lógico:** Campo `IsActive` en lugar de DELETE

### Validación de Contraseñas
- Longitud mínima: 6 caracteres
- Email único obligatorio
- No requiere caracteres especiales por defecto

## Instalación y Configuración

### Requisitos Previos
- .NET SDK 8.0 o superior
- PostgreSQL 12+ (o SQLite/SQL Server)
- Servidor LDAP/Active Directory (opcional)

### Instalación

1. **Clonar el repositorio**
```bash
git clone <repository-url>
cd webapp_claude
```

2. **Restaurar dependencias**
```bash
dotnet restore
```

3. **Configurar variables de entorno**

Copiar el archivo de ejemplo y editar:
```bash
cp .env.example .env
```

Editar `.env` con tus valores:
```env
# Base de datos
CONNECTION_STRING=Host=localhost;Database=webapp_claude;Username=postgres;Password=yourpassword

# LDAP/Active Directory (opcional)
LDAP_SERVER=ldap.example.com
LDAP_PORT=389
LDAP_BASE_DN=DC=example,DC=com
LDAP_BIND_USER=CN=admin,DC=example,DC=com
LDAP_BIND_PASSWORD=password
LDAP_DOMAIN=EXAMPLE
LDAP_USE_SSL=false

# Usuario administrador inicial
SEED_ADMIN_EMAIL=admin@example.com
SEED_ADMIN_PASSWORD=Admin123!
SEED_ADMIN_USERNAME=admin
SEED_ADMIN_FIRSTNAME=Admin
SEED_ADMIN_LASTNAME=User
```

4. **Aplicar migraciones**
```bash
dotnet ef database update
```

5. **Ejecutar la aplicación**
```bash
dotnet run
```

La aplicación estará disponible en `https://localhost:5001` o `http://localhost:5000`

### Primera Ejecución

Al iniciar por primera vez:
1. Se crearán los roles automáticamente
2. Se creará el usuario administrador desde las variables de entorno
3. Accede con las credenciales de `SEED_ADMIN_EMAIL` y `SEED_ADMIN_PASSWORD`

## Dependencias Principales

### NuGet Packages

```xml
<!-- Entity Framework Core -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.12" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.12" />

<!-- ASP.NET Core Identity -->
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.12" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.12" />

<!-- LDAP -->
<PackageReference Include="System.DirectoryServices.Protocols" Version="10.0.1" />

<!-- Utilidades -->
<PackageReference Include="DotNetEnv" Version="3.1.1" />
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="8.0.7" />
```

## Migraciones de Base de Datos

### Historial de Migraciones

Las principales migraciones incluyen:

1. **InitialCreate** - Estructura inicial
2. **AddIsLdapUser** - Campo para usuarios LDAP
3. **AddIsActive** - Borrado lógico de usuarios
4. **AddLinealyticsTriggersAndViews** - Sistema de métricas
5. **RefactorToMetricasMaquina** - Refactorización a modelo unificado
6. **AddDispositivosYLecturas** - Sistema de dispositivos IoT
7. **RenameRolesToDepartamentos** - Cambio de nomenclatura
8. **AddBotonIdToRegistroParoBotonera** - Vinculación de botones

### Comandos Útiles

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir a migración específica
dotnet ef database update NombreMigracion

# Generar script SQL
dotnet ef migrations script

# Eliminar última migración (si no se aplicó)
dotnet ef migrations remove
```

## Rutas y Endpoints Principales

### Públicas
- `/` - Página de inicio
- `/Identity/Account/Login` - Login
- `/Identity/Account/Register` - Registro

### Administración (Requiere SuperAdmin/Admin)
- `/Admin` - Gestión de usuarios del sistema
- `/Admin/Update/{id}` - Actualizar usuario

### Operadores (Requiere SuperAdmin/Admin)
- `/Operadores` - Listado de operadores
- `/Operadores/Create` - Crear operador
- `/Operadores/Edit/{id}` - Editar operador
- `/Operadores/Details/{id}` - Detalles de operador

### Planta (Requiere SuperAdmin/Admin)
- `/Planta/Areas` - Gestión de áreas
- `/Planta/Lineas` - Gestión de líneas
- `/Planta/Estaciones` - Gestión de estaciones
- `/Planta/Maquinas` - Gestión de máquinas
- `/Planta/Botones` - Gestión de botones
- `/Planta/Modelos` - Gestión de modelos

### Linealytics (Requiere autenticación)
- `/Linealytics` - Índice del módulo
- `/Linealytics/Oee` - **Vista OEE** con gauges en tiempo real e histórico
- `/Linealytics/Dashboard` - Timeline de paros y contadores (últimas 24 h)
- `/Linealytics/Turnos` - Gestión de turnos
- `/Linealytics/Productos` - Gestión de productos
- `/Linealytics/Botoneras` - Gestión de botoneras
- `/Linealytics/Botones` - Gestión de botones
- `/Linealytics/ParosLinea` - Historial de paros
- `/Linealytics/ParosSinComentar` - Paros cerrados sin comentar
- `/Linealytics/Contadores` - Dashboard de producción OK/NOK
- `/Linealytics/CorridasProduccion` - Historial de corridas

#### Endpoints JSON del módulo OEE
- `GET /Linealytics/GetOeeActual?maquinaId=&turnoId=` — Calcula OEE en vivo (no guarda)
- `POST /Linealytics/CalcularOee` — Calcula y guarda en `MetricasMaquina`
- `GET /Linealytics/GetOeeHistorial?maquinaId=&dias=` — Historial de `MetricasMaquina`
- `GET /Linealytics/GetDatosParos` — Minutos de paro por línea/día (últimos 7 días)
- `GET /Linealytics/GetDatosContadores` — Producción OK/NOK por máquina (últimas 24 h)
- `GET /Linealytics/GetDatosParosPorDepartamento` — Segmentos Gantt de paros por departamento

### Departamentos (Requiere SuperAdmin/Admin)
- `/DepartamentosOperador` - Gestión de departamentos

## Roles y Permisos

### SuperAdmin
- Acceso total al sistema
- Puede crear otros SuperAdmins
- Gestión completa de usuarios

### Administrator (Admin)
- Acceso a todos los módulos
- Gestión de operadores, planta y métricas
- No puede crear SuperAdmins

### User
- Acceso limitado de solo lectura
- No puede modificar configuración

### Configuración de Roles

Los roles se definen en [Enums/Roles.cs](Enums/Roles.cs):

```csharp
public enum Roles
{
    SuperAdmin,
    Administrator,
    User
}
```

## Flujo de la Aplicación

```
┌─────────────────┐
│   Program.cs    │
│  (Punto entrada)│
└────────┬────────┘
         │
         ├─► Cargar variables de entorno (.env)
         │
         ├─► Configurar DbContext (PostgreSQL)
         │
         ├─► Configurar Identity + LDAP
         │
         ├─► Seed: Crear roles y admin
         │
         ├─► Configurar middleware (auth, static files, routing)
         │
         ├─► Mapear rutas (controllers + Razor Pages)
         │
         └─► Escuchar requests HTTP
              │
              ▼
        ┌──────────────┐
        │ Request HTTP │
        └──────┬───────┘
               │
               ├─► Middleware de autenticación
               │
               ├─► Routing
               │
               ├─► Controller → Action
               │
               ├─► Service (si aplica)
               │
               ├─► DbContext → PostgreSQL
               │
               ├─► View Razor
               │
               └─► Response HTTP
```

## Base de Datos - Esquema PostgreSQL

### Esquema: `authentication`

**Tablas de ASP.NET Identity:**
- AspNetUsers
- AspNetRoles
- AspNetUserRoles
- AspNetUserClaims
- AspNetUserLogins
- AspNetRoleClaims
- AspNetUserTokens

### Esquema: `operadores`

- Operadores
- DepartamentosOperador
- OperadorDepartamentos (tabla intermedia)

### Esquema: `planta`

- Areas
- Lineas
- Estaciones
- Maquinas
- Botones
- Modelos

### Esquema: `linealytics`

**Configuración:**
- Turnos
- Productos
- CategoriasParo
- CausasParo

**Métricas:**
- MetricasMaquina
- SesionesProduccion
- RegistrosParos
- RegistrosProduccionHora
- HistorialCambiosParos

**Hardware:**
- Botoneras
- RegistrosParoBotonera
- RegistrosFallas
- RegistrosContadores
- ComentariosParoBotonera
- Dispositivos
- LecturasContador

## Desarrollo

### Estructura de Código

**Convenciones:**
- **Controllers:** Plurales, sufijo "Controller" (ej: OperadoresController)
- **Models:** Singulares (ej: Operador, Maquina)
- **Services:** Interfaz + Implementación (ILdapService + LdapService)
- **Views:** Carpeta por controlador, nombre de vista = nombre de acción

### Agregar Nueva Funcionalidad

1. **Crear modelo** en `Models/`
2. **Agregar DbSet** en `ApplicationDbContext.cs`
3. **Crear migración:** `dotnet ef migrations add NombreMigration`
4. **Aplicar migración:** `dotnet ef database update`
5. **Crear controlador** con acciones CRUD
6. **Crear vistas Razor** en `Views/NombreControlador/`
7. **Agregar navegación** en `_Layout.cshtml` si aplica

### Agregar Nuevo Servicio

1. Crear interfaz `IMyService.cs` en `Services/`
2. Implementar `MyService.cs` en `Services/`
3. Registrar en `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IMyService, MyService>();
   ```

## Características del Sistema Linealytics

### Cálculo de OEE

**OEE (Overall Equipment Effectiveness)** es el indicador clave de rendimiento:

```
OEE = Disponibilidad × Rendimiento × Calidad
```

Donde:

- **Disponibilidad** = Tiempo Operativo / Tiempo Programado
- **Rendimiento** = (Unidades Producidas × Tiempo Ciclo Ideal) / Tiempo Operativo
- **Calidad** = Unidades Buenas / Unidades Totales

### Sesiones de Producción

Las sesiones registran:
- Máquina asociada
- Turno y producto
- Cantidad objetivo y producida
- Hora de inicio y fin
- Operadores asignados

### Registros de Paro

Cada paro incluye:
- Timestamp de inicio y fin
- Duración en minutos
- Categoría y causa
- Máquina afectada
- Auditoría de cambios (quién, cuándo, qué cambió)

### Sistema de Botoneras

Hardware de botoneras IP que:
- Registra paros en tiempo real
- Captura fallas de máquinas
- Cuenta unidades OK/NOK
- Permite comentarios de operadores

**Tabla:** RegistroParoBotonera, RegistroFalla, RegistroContador

## Comandos Útiles

### Desarrollo

```bash
# Ejecutar en modo desarrollo
dotnet run

# Ejecutar en modo desarrollo con watch (auto-reload)
dotnet watch run

# Compilar
dotnet build

# Limpiar artefactos
dotnet clean

# Publicar para producción
dotnet publish -c Release -o ./publish
```

### Entity Framework

```bash
# Ver migraciones aplicadas
dotnet ef migrations list

# Generar script SQL de todas las migraciones
dotnet ef migrations script -o migration.sql

# Actualizar a migración específica
dotnet ef database update NombreMigracion

# Eliminar base de datos
dotnet ef database drop
```

### Git

```bash
# Ver estado
git status

# Crear commit
git add .
git commit -m "Descripción del cambio"

# Ver historial
git log --oneline
```

## Notas de Desarrollo

### Variables de Entorno

El archivo `.env` **NO debe** incluirse en el repositorio (está en `.gitignore`).

Para nuevos desarrolladores:
1. Copiar `.env.example` a `.env`
2. Completar con valores reales
3. Nunca commitear `.env`

### Conexión a PostgreSQL

Formato de CONNECTION_STRING:
```
Host=servidor;Port=5432;Database=nombre_bd;Username=usuario;Password=contraseña
```

### Integración LDAP

Si no se usa LDAP:
- Dejar variables LDAP vacías
- Los usuarios solo podrán autenticarse localmente
- El registro funcionará normalmente

### Desarrollo Local

Para desarrollo sin PostgreSQL:
1. Cambiar a SQLite en `Program.cs`
2. Actualizar CONNECTION_STRING a formato SQLite:
   ```
   Data Source=app.db
   ```

## Troubleshooting

### Error: "No connection string found"
- Verificar que existe `.env` en la raíz del proyecto
- Verificar que `CONNECTION_STRING` está definida

### Error: "Unable to connect to PostgreSQL"
- Verificar que PostgreSQL está ejecutándose
- Verificar credenciales en CONNECTION_STRING
- Verificar firewall/puerto 5432

### Error: "Pending migrations"
- Ejecutar: `dotnet ef database update`

### Error: "LDAP server unavailable"
- Verificar LDAP_SERVER y LDAP_PORT
- Verificar que el servidor LDAP está accesible
- Si no se usa LDAP, ignorar (solo afecta login AD)

### Error: "Role already exists"
- Los roles se crean automáticamente al iniciar
- Si hay error, eliminar roles de la BD y reiniciar

## Estructura del Proyecto por Responsabilidad

### Controllers
Responsables del flujo de control HTTP y mapeo de rutas. Utilizan servicios para lógica de negocio y devuelven vistas o datos.

### Models
Entidades de dominio que mapean a tablas de base de datos mediante EF Core. Organizados por esquema de BD.

### Views
Plantillas Razor que generan HTML dinámico. Organizadas por controlador con vistas compartidas en /Shared.

### Services
Capa de lógica de negocio. Contiene operaciones complejas, validaciones y reglas de negocio reutilizables.

### Data
Contexto de Entity Framework Core que gestiona la conexión a BD y configuración de entidades.

### Migrations
Historial versionado de cambios en el esquema de base de datos, aplicables mediante EF Core.

## Patrones y Convenciones

### Inyección de Dependencias
Todos los servicios se registran en Program.cs y se inyectan vía constructor en controladores.

### Repository Pattern
DbContext actúa como Unit of Work y DbSets como repositorios genéricos.

### Async/Await
Todas las operaciones de BD y operaciones I/O utilizan programación asíncrona.

### Validación
- Data Annotations en modelos para validación básica
- Validación custom en servicios para reglas complejas
- ModelState para validación en controladores

## Próximas Mejoras Técnicas

### Backend
- Implementar patrón Repository explícito para mejor testabilidad
- Agregar capa de DTOs para separar modelos de dominio de ViewModels
- Implementar logging estructurado con Serilog
- Agregar API REST con versionado
- Implementar caché distribuido con Redis
- Agregar Jobs en background con Hangfire para reportes

### Frontend
- Migrar a SPA con React/Vue para dashboard Linealytics
- Implementar WebSockets para actualizaciones en tiempo real
- Agregar PWA para acceso offline de operadores
- Implementar gráficas interactivas con Chart.js/D3.js

### DevOps
- Containerización con Docker
- CI/CD con GitHub Actions
- Monitoreo con Application Insights
- Tests unitarios y de integración

### Seguridad
- Implementar rate limiting
- Agregar auditoría completa de cambios
- Implementar 2FA para usuarios administrativos
- Reforzar CSP headers
