using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Linealytics;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class LinealyticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OeeCalculationService _oeeService;

        public LinealyticsController(ApplicationDbContext context, OeeCalculationService oeeService)
        {
            _context = context;
            _oeeService = oeeService;
        }

        // ========== DASHBOARD ==========

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            // Cargar datos de planta para los filtros
            ViewBag.Areas = await _context.Areas
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            ViewBag.Lineas = await _context.Lineas
                .Include(l => l.Area)
                .Where(l => l.Activo)
                .OrderBy(l => l.Area.Nombre)
                .ThenBy(l => l.Nombre)
                .ToListAsync();

            ViewBag.Estaciones = await _context.Estaciones
                .Include(e => e.Linea)
                    .ThenInclude(l => l.Area)
                .Where(e => e.Activo)
                .OrderBy(e => e.Linea.Area.Nombre)
                .ThenBy(e => e.Linea.Nombre)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            ViewBag.Maquinas = await _context.Maquinas
                .Include(m => m.Estacion)
                    .ThenInclude(e => e.Linea)
                        .ThenInclude(l => l.Area)
                .Where(m => m.Activo)
                .OrderBy(m => m.Estacion.Linea.Area.Nombre)
                .ThenBy(m => m.Estacion.Linea.Nombre)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDatosParos(int? areaId, int? lineaId, int? estacionId, int? maquinaId)
        {
            var fechaInicio = DateTime.UtcNow.AddDays(-7); // Últimos 7 días

            // Query base de paros de botonera
            var query = _context.RegistrosParoBotonera
                .Include(p => p.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                            .ThenInclude(l => l.Area)
                .Where(p => p.FechaHoraInicio >= fechaInicio && p.DuracionMinutos.HasValue);

            // Aplicar filtros
            if (areaId.HasValue)
            {
                query = query.Where(p => p.Maquina.Estacion.Linea.AreaId == areaId.Value);
            }

            if (lineaId.HasValue)
            {
                query = query.Where(p => p.Maquina.Estacion.LineaId == lineaId.Value);
            }

            if (estacionId.HasValue)
            {
                query = query.Where(p => p.Maquina.EstacionId == estacionId.Value);
            }

            if (maquinaId.HasValue)
            {
                query = query.Where(p => p.MaquinaId == maquinaId.Value);
            }

            var paros = await query.ToListAsync();

            // Agrupar por día y línea
            var datosAgrupados = paros
                .GroupBy(p => new
                {
                    Fecha = p.FechaHoraInicio.Date,
                    Linea = p.Maquina.Estacion.Linea.Nombre,
                    LineaId = p.Maquina.Estacion.LineaId
                })
                .Select(g => new
                {
                    Fecha = g.Key.Fecha.ToString("dd/MM"),
                    Linea = g.Key.Linea,
                    LineaId = g.Key.LineaId,
                    TotalMinutos = g.Sum(p => p.DuracionMinutos ?? 0),
                    CantidadParos = g.Count()
                })
                .OrderBy(x => x.Fecha)
                .ToList();

            // Obtener todas las líneas únicas
            var lineas = datosAgrupados.Select(d => d.Linea).Distinct().ToList();

            // Obtener todas las fechas únicas
            var fechas = datosAgrupados.Select(d => d.Fecha).Distinct().ToList();

            // Construir estructura para la gráfica
            var resultado = new
            {
                Lineas = lineas,
                Fechas = fechas,
                Datos = datosAgrupados
            };

            return Json(resultado);
        }

        // ========== CONTADORES DE PRODUCCION ==========

        public async Task<IActionResult> Contadores()
        {
            // Cargar datos de planta para los filtros
            ViewBag.Areas = await _context.Areas
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            ViewBag.Lineas = await _context.Lineas
                .Include(l => l.Area)
                .Where(l => l.Activo)
                .OrderBy(l => l.Area.Nombre)
                .ThenBy(l => l.Nombre)
                .ToListAsync();

            ViewBag.Estaciones = await _context.Estaciones
                .Include(e => e.Linea)
                    .ThenInclude(l => l.Area)
                .Where(e => e.Activo)
                .OrderBy(e => e.Linea.Area.Nombre)
                .ThenBy(e => e.Linea.Nombre)
                .ThenBy(e => e.Nombre)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDatosContadores(int? areaId, int? lineaId, int? estacionId)
        {
            var fechaInicio = DateTime.UtcNow.AddHours(-24); // Ultimas 24 horas

            // Query base de lecturas de contadores
            var query = _context.LecturasContador
                .Include(l => l.Maquina)
                    .ThenInclude(m => m!.Estacion)
                        .ThenInclude(e => e!.Linea)
                .Where(l => l.FechaHoraLectura >= fechaInicio);

            // Aplicar filtros
            if (areaId.HasValue)
            {
                query = query.Where(l => l.Maquina!.Estacion!.Linea!.AreaId == areaId.Value);
            }

            if (lineaId.HasValue)
            {
                query = query.Where(l => l.Maquina!.Estacion!.LineaId == lineaId.Value);
            }

            if (estacionId.HasValue)
            {
                query = query.Where(l => l.Maquina!.EstacionId == estacionId.Value);
            }

            // Agrupar por maquina y sumar produccion
            var datosPorMaquina = await query
                .GroupBy(l => new
                {
                    MaquinaId = l.MaquinaId,
                    MaquinaNombre = l.Maquina!.Nombre,
                    EstacionNombre = l.Maquina!.Estacion!.Nombre,
                    LineaNombre = l.Maquina!.Estacion!.Linea!.Nombre
                })
                .Select(g => new
                {
                    Id = g.Key.MaquinaId,
                    Nombre = g.Key.MaquinaNombre,
                    Estacion = g.Key.EstacionNombre,
                    Linea = g.Key.LineaNombre,
                    TotalOK = g.Sum(l => l.ProduccionOK),
                    TotalNOK = g.Sum(l => l.ProduccionNOK)
                })
                .OrderBy(m => m.Linea)
                .ThenBy(m => m.Estacion)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

            var resultado = new
            {
                Maquinas = datosPorMaquina
            };

            return Json(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> GetDatosParosPorDepartamento(int? areaId, int? lineaId, int? estacionId, int? maquinaId)
        {
            var fechaInicio = DateTime.UtcNow.AddHours(-24); // Últimas 24 horas
            var ahora = DateTime.UtcNow;

            // Query base de paros de botonera - incluir paros cerrados Y abiertos
            var query = _context.RegistrosParoBotonera
                .Include(p => p.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                            .ThenInclude(l => l.Area)
                .Include(p => p.DepartamentoOperador)
                .Where(p => p.FechaHoraInicio >= fechaInicio);

            // Aplicar filtros
            if (areaId.HasValue)
            {
                query = query.Where(p => p.Maquina.Estacion.Linea.AreaId == areaId.Value);
            }

            if (lineaId.HasValue)
            {
                query = query.Where(p => p.Maquina.Estacion.LineaId == lineaId.Value);
            }

            if (estacionId.HasValue)
            {
                query = query.Where(p => p.Maquina.EstacionId == estacionId.Value);
            }

            if (maquinaId.HasValue)
            {
                query = query.Where(p => p.MaquinaId == maquinaId.Value);
            }

            var paros = await query
                .OrderBy(p => p.FechaHoraInicio)
                .ToListAsync();

            // Preparar datos para la gráfica: cada paro individual con su información
            // Para paros abiertos (sin FechaHoraFin), usar la fecha actual
            var datosParos = paros
                .Where(p => p.DepartamentoOperador != null)
                .Select(p => new
                {
                    Maquina = p.Maquina.Nombre,
                    MaquinaId = p.MaquinaId,
                    Departamento = p.DepartamentoOperador!.Nombre,
                    Color = p.DepartamentoOperador.Color ?? "#666666",
                    FechaInicio = p.FechaHoraInicio,
                    FechaFin = p.FechaHoraFin ?? ahora, // Si está abierto, usar fecha actual
                    DuracionMinutos = p.DuracionMinutos ?? (int)Math.Round((ahora - p.FechaHoraInicio).TotalMinutes),
                    EstaAbierto = p.Estado == "Abierto" // Indicador de paro en curso
                })
                .ToList();

            // Obtener todas las máquinas únicas
            var maquinas = datosParos
                .GroupBy(d => new { d.MaquinaId, d.Maquina })
                .Select(g => new {
                    Id = g.Key.MaquinaId,
                    Nombre = g.Key.Maquina
                })
                .OrderBy(m => m.Nombre)
                .ToList();

            // Obtener todos los departamentos únicos
            var departamentos = datosParos
                .GroupBy(d => new { d.Departamento, d.Color })
                .Select(g => new {
                    Departamento = g.Key.Departamento,
                    Color = g.Key.Color
                })
                .ToList();

            // Calcular fecha mínima y máxima para el rango
            var fechaMin = paros.Any() ? paros.Min(p => p.FechaHoraInicio) : ahora;
            var fechaMax = ahora; // Siempre usar la fecha actual como máximo para mostrar paros abiertos

            // Construir estructura para la gráfica
            var resultado = new
            {
                Maquinas = maquinas,
                Departamentos = departamentos,
                Datos = datosParos,
                FechaMin = fechaMin,
                FechaMax = fechaMax
            };

            return Json(resultado);
        }

        // ========== TURNOS ==========

        public async Task<IActionResult> Turnos()
        {
            var turnos = await _context.Turnos
                .OrderBy(t => t.HoraInicio)
                .ToListAsync();
            return View(turnos);
        }

        public IActionResult CreateTurno()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTurno([Bind("Nombre,HoraInicio,HoraFin")] Turno turno)
        {
            if (ModelState.IsValid)
            {
                turno.Activo = true;
                turno.DuracionMinutos = CalcularDuracionTurno(turno.HoraInicio, turno.HoraFin);
                _context.Add(turno);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Turno creado exitosamente.";
                return RedirectToAction(nameof(Turnos));
            }
            return View(turno);
        }

        public async Task<IActionResult> EditTurno(int? id)
        {
            if (id == null) return NotFound();

            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTurno(int id, [Bind("Id,Nombre,HoraInicio,HoraFin,Activo")] Turno turno)
        {
            if (id != turno.Id) return NotFound();

            if (ModelState.IsValid)
            {
                turno.DuracionMinutos = CalcularDuracionTurno(turno.HoraInicio, turno.HoraFin);
                try
                {
                    _context.Update(turno);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Turno actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnoExists(turno.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Turnos));
            }
            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno != null)
            {
                turno.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Turno desactivado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el turno.";
            }
            return RedirectToAction(nameof(Turnos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateTurno(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno != null)
            {
                turno.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Turno activado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el turno.";
            }
            return RedirectToAction(nameof(Turnos));
        }

        // ========== PRODUCTOS ==========

        public async Task<IActionResult> Productos()
        {
            var productos = await _context.Productos
                .OrderBy(p => p.Nombre)
                .ToListAsync();
            return View(productos);
        }

        public IActionResult CreateProducto()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProducto([Bind("Codigo,Nombre,Descripcion,TiempoCicloSegundos,UnidadesPorCiclo")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.Activo = true;
                _context.Add(producto);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Producto creado exitosamente.";
                return RedirectToAction(nameof(Productos));
            }
            return View(producto);
        }

        public async Task<IActionResult> EditProducto(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProducto(int id, [Bind("Id,Codigo,Nombre,Descripcion,TiempoCicloSegundos,UnidadesPorCiclo,Activo")] Producto producto)
        {
            if (id != producto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Producto actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Productos));
            }
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                producto.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Producto desactivado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el producto.";
            }
            return RedirectToAction(nameof(Productos));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                producto.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Producto activado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el producto.";
            }
            return RedirectToAction(nameof(Productos));
        }

        // ========== BOTONES ==========

        public async Task<IActionResult> Botones()
        {
            var botones = await _context.Botones
                .Include(b => b.DepartamentoOperador)
                .OrderBy(b => b.Codigo)
                .ToListAsync();
            return View(botones);
        }

        public async Task<IActionResult> CreateBoton()
        {
            ViewBag.Departamentos = new SelectList(
                await _context.DepartamentosOperador.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(),
                "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBoton([Bind("Nombre,DepartamentoOperadorId,Descripcion")] Boton boton)
        {
            // Remover validación de la propiedad de navegación
            ModelState.Remove("DepartamentoOperador");
            // Remover validación de Codigo ya que se genera automáticamente
            ModelState.Remove("Codigo");

            if (ModelState.IsValid)
            {
                // Usar una transacción para asegurar consistencia
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        boton.Activo = true;
                        boton.FechaCreacion = DateTime.UtcNow;
                        // Temporalmente asignar un valor único para evitar conflicto con índice único
                        boton.Codigo = $"TEMP-{Guid.NewGuid()}";

                        _context.Add(boton);
                        await _context.SaveChangesAsync();

                        // Generar el código automáticamente: BTN-{Id}
                        boton.Codigo = $"BTN-{boton.Id}";
                        _context.Update(boton);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        TempData["Success"] = "Botón creado exitosamente.";
                        return RedirectToAction(nameof(Botones));
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }

            // Si llegamos aquí, hay errores de validación
            ViewBag.Departamentos = new SelectList(
                await _context.DepartamentosOperador.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(),
                "Id", "Nombre", boton.DepartamentoOperadorId);
            return View(boton);
        }

        public async Task<IActionResult> EditBoton(int? id)
        {
            if (id == null)
                return NotFound();

            var boton = await _context.Botones.FindAsync(id);
            if (boton == null)
                return NotFound();

            ViewBag.Departamentos = new SelectList(
                await _context.DepartamentosOperador.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(),
                "Id", "Nombre", boton.DepartamentoOperadorId);
            return View(boton);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBoton(int id, [Bind("Id,Codigo,Nombre,DepartamentoOperadorId,Descripcion,Activo")] Boton boton)
        {
            if (id != boton.Id)
                return NotFound();

            // Remover validación de la propiedad de navegación
            ModelState.Remove("DepartamentoOperador");

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener la entidad existente para preservar las fechas
                    var botonExistente = await _context.Botones.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (botonExistente == null)
                        return NotFound();

                    // Preservar las fechas originales
                    boton.FechaCreacion = botonExistente.FechaCreacion;
                    boton.FechaUltimaActivacion = botonExistente.FechaUltimaActivacion;

                    // Actualizar la entidad
                    _context.Update(boton);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Botón actualizado exitosamente.";
                    return RedirectToAction(nameof(Botones));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BotonExists(boton.Id))
                        return NotFound();

                    throw;
                }
            }
            ViewBag.Departamentos = new SelectList(
                await _context.DepartamentosOperador.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(),
                "Id", "Nombre", boton.DepartamentoOperadorId);
            return View(boton);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateBoton(int id)
        {
            var boton = await _context.Botones.FindAsync(id);
            if (boton != null)
            {
                boton.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Botón desactivado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el botón.";
            }
            return RedirectToAction(nameof(Botones));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateBoton(int id)
        {
            var boton = await _context.Botones.FindAsync(id);
            if (boton != null)
            {
                boton.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Botón activado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el botón.";
            }
            return RedirectToAction(nameof(Botones));
        }

        // ========== PAROS DE LÍNEA (BOTONERA) ==========

        public async Task<IActionResult> ParosLinea()
        {
            var paros = await _context.RegistrosParoBotonera
                .Include(p => p.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                .Include(p => p.DepartamentoOperador)
                .Include(p => p.Boton)
                .Include(p => p.Operador)
                .Include(p => p.Comentarios)
                .OrderByDescending(p => p.FechaHoraInicio)
                .ToListAsync();
            return View(paros);
        }

        [Authorize]
        public async Task<IActionResult> ParosSinComentar()
        {
            var parosCerrados = await _context.RegistrosParoBotonera
                .Include(p => p.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                .Include(p => p.DepartamentoOperador)
                .Include(p => p.Boton)
                .Include(p => p.Operador)
                .Include(p => p.Comentarios!)
                    .ThenInclude(c => c.User)
                .Where(p => p.Estado == "Cerrado")
                .OrderByDescending(p => p.FechaHoraFin)
                .ToListAsync();

            return View(parosCerrados);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarComentario(int paroId, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
            {
                TempData["Error"] = "El comentario no puede estar vacío.";
                return RedirectToAction(nameof(ParosSinComentar));
            }

            var paro = await _context.RegistrosParoBotonera
                .FirstOrDefaultAsync(p => p.Id == paroId);

            if (paro == null)
            {
                TempData["Error"] = "Paro no encontrado.";
                return RedirectToAction(nameof(ParosSinComentar));
            }

            if (paro.Estado != "Cerrado")
            {
                TempData["Error"] = "Solo se pueden comentar paros cerrados.";
                return RedirectToAction(nameof(ParosSinComentar));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "No se pudo identificar al usuario. Por favor, inicie sesión nuevamente.";
                return RedirectToAction(nameof(ParosSinComentar));
            }

            var nuevoComentario = new ComentarioParoBotonera
            {
                RegistroParoBotoneraId = paroId,
                UserId = userId!,
                Comentario = comentario,
                FechaCreacion = DateTime.UtcNow
            };

            _context.ComentariosParoBotonera.Add(nuevoComentario);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Comentario agregado exitosamente.";
            return RedirectToAction(nameof(ParosSinComentar));
        }

        // ========== BOTONERAS ==========

        public async Task<IActionResult> Botoneras()
        {
            var botoneras = await _context.Botoneras
                .Include(b => b.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                .OrderBy(b => b.Nombre)
                .ToListAsync();
            return View(botoneras);
        }

        public async Task<IActionResult> CreateBotonera()
        {
            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas
                    .Include(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBotonera([Bind("Nombre,Descripcion,DireccionIP,MaquinaId")] Botonera botonera)
        {
            // Remover errores de validación de la propiedad de navegación
            ModelState.Remove("Maquina");
            // Remover validación de NumeroSerie ya que se genera automáticamente
            ModelState.Remove("NumeroSerie");

            if (ModelState.IsValid)
            {
                try
                {
                    botonera.Activo = true;

                    // Usar una transacción para asegurar consistencia
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Temporalmente asignar un valor único para evitar conflicto con índice único
                            botonera.NumeroSerie = $"TEMP-{Guid.NewGuid()}";

                            _context.Add(botonera);
                            await _context.SaveChangesAsync();

                            // Generar el número de serie automáticamente: BTNR-{Id}
                            botonera.NumeroSerie = $"BTNR-{botonera.Id}";
                            _context.Update(botonera);
                            await _context.SaveChangesAsync();

                            await transaction.CommitAsync();

                            TempData["Success"] = "Botonera creada exitosamente.";
                            return RedirectToAction(nameof(Botoneras));
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("IX_Botoneras_DireccionIP") == true)
                    {
                        ModelState.AddModelError("DireccionIP", "Esta dirección IP ya está registrada para otra botonera.");
                    }
                    else if (ex.InnerException?.Message.Contains("IX_Botoneras_NumeroSerie") == true)
                    {
                        ModelState.AddModelError("NumeroSerie", "Este número de serie ya está registrado para otra botonera.");
                    }
                    else
                    {
                        TempData["Error"] = "Error al crear la botonera.";
                    }
                }
            }

            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas
                    .Include(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre",
                botonera.MaquinaId
            );
            return View(botonera);
        }

        public async Task<IActionResult> EditBotonera(int? id)
        {
            if (id == null) return NotFound();

            var botonera = await _context.Botoneras.FindAsync(id);
            if (botonera == null) return NotFound();

            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas
                    .Include(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre",
                botonera.MaquinaId
            );
            return View(botonera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBotonera(int id, [Bind("Id,Nombre,Descripcion,DireccionIP,MaquinaId,Activo,FechaCreacion")] Botonera botonera)
        {
            if (id != botonera.Id) return NotFound();

            // Remover errores de validación de la propiedad de navegación
            ModelState.Remove("Maquina");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBotonera = await _context.Botoneras.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
                    if (existingBotonera == null) return NotFound();

                    botonera.FechaCreacion = existingBotonera.FechaCreacion;
                    botonera.NumeroSerie = existingBotonera.NumeroSerie; // Preservar NumeroSerie

                    _context.Update(botonera);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Botonera actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BotoneraExists(botonera.Id))
                        return NotFound();
                    else
                        throw;
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message.Contains("IX_Botoneras_DireccionIP") == true)
                    {
                        ModelState.AddModelError("DireccionIP", "Esta dirección IP ya está registrada para otra botonera.");
                    }
                    else if (ex.InnerException?.Message.Contains("IX_Botoneras_NumeroSerie") == true)
                    {
                        ModelState.AddModelError("NumeroSerie", "Este número de serie ya está registrado para otra botonera.");
                    }
                    else
                    {
                        TempData["Error"] = "Error al actualizar la botonera.";
                    }

                    ViewBag.Maquinas = new SelectList(
                        await _context.Maquinas
                            .Include(m => m.Estacion)
                                .ThenInclude(e => e.Linea)
                            .Where(m => m.Activo)
                            .OrderBy(m => m.Nombre)
                            .ToListAsync(),
                        "Id",
                        "Nombre",
                        botonera.MaquinaId
                    );
                    return View(botonera);
                }
                return RedirectToAction(nameof(Botoneras));
            }

            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas
                    .Include(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre",
                botonera.MaquinaId
            );
            return View(botonera);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateBotonera(int id)
        {
            var botonera = await _context.Botoneras.FindAsync(id);
            if (botonera != null)
            {
                botonera.Activo = false;
                _context.Update(botonera);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Botonera desactivada exitosamente.";
            }
            return RedirectToAction(nameof(Botoneras));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateBotonera(int id)
        {
            var botonera = await _context.Botoneras.FindAsync(id);
            if (botonera != null)
            {
                botonera.Activo = true;
                _context.Update(botonera);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Botonera activada exitosamente.";
            }
            return RedirectToAction(nameof(Botoneras));
        }

        // ========== CORRIDAS DE PRODUCCIÓN ==========

        public async Task<IActionResult> CorridasProduccion(int? maquinaId)
        {
            var query = _context.CorridasProduccion
                .Include(c => c.Maquina)
                .Include(c => c.Producto)
                .AsQueryable();

            if (maquinaId.HasValue)
            {
                query = query.Where(c => c.MaquinaId == maquinaId.Value);
            }

            var corridas = await query
                .OrderByDescending(c => c.FechaInicio)
                .Take(100)
                .ToListAsync();

            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Nombre)
                    .ToListAsync(),
                "Id", "Nombre", maquinaId);

            ViewBag.MaquinaIdSeleccionado = maquinaId;

            return View(corridas);
        }

        public async Task<IActionResult> DetalleCorrida(int? id)
        {
            if (id == null) return NotFound();

            var corrida = await _context.CorridasProduccion
                .Include(c => c.Maquina)
                .Include(c => c.Producto)
                .Include(c => c.Lecturas.OrderBy(l => l.FechaHoraLectura))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (corrida == null) return NotFound();

            return View(corrida);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarCorrida(int id)
        {
            var corrida = await _context.CorridasProduccion.FindAsync(id);
            if (corrida != null && corrida.Estado == "Activa")
            {
                corrida.Estado = "Cerrada";
                corrida.FechaFin = DateTime.UtcNow;
                _context.Update(corrida);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Corrida cerrada exitosamente.";
            }
            return RedirectToAction(nameof(CorridasProduccion), new { maquinaId = corrida?.MaquinaId });
        }

        // ========== MÉTODOS TEMPORALES DE CONFIGURACIÓN ==========

        [HttpGet]
        public async Task<IActionResult> AsignarColoresDepartamentos()
        {
            // Colores predefinidos para los departamentos
            var colores = new List<string>
            {
                "#42A5F5", // Azul
                "#ef5350", // Rojo
                "#66bb6a", // Verde
                "#ff7043", // Naranja
                "#26a69a", // Turquesa
                "#AB47BC", // Púrpura
                "#FFA726", // Naranja claro
                "#EC407A", // Rosa
                "#5C6BC0", // Índigo
                "#FFCA28"  // Amarillo
            };

            var departamentos = await _context.DepartamentosOperador
                .Where(d => d.Activo)
                .OrderBy(d => d.Id)
                .ToListAsync();

            for (int i = 0; i < departamentos.Count; i++)
            {
                // Asignar color cíclicamente
                departamentos[i].Color = colores[i % colores.Count];
            }

            await _context.SaveChangesAsync();

            var resultado = departamentos.Select(d => new
            {
                d.Id,
                d.Nombre,
                d.Color
            }).ToList();

            return Json(new
            {
                mensaje = "Colores asignados exitosamente",
                departamentos = resultado
            });
        }

        // ========== OEE ==========

        [HttpGet]
        public async Task<IActionResult> Oee()
        {
            ViewBag.Maquinas = await _context.Maquinas
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            ViewBag.Turnos = await _context.Turnos
                .Where(t => t.Activo)
                .OrderBy(t => t.HoraInicio)
                .ToListAsync();

            return View();
        }

        /// <summary>
        /// Vista de directorio OEE - solo muestra el apartado OEE del dashboard
        /// </summary>
        public IActionResult OeeDirectory()
        {
            return View();
        }

        /// <summary>
        /// Vista de directorio Paros - solo muestra el apartado de Paros de Línea del dashboard
        /// </summary>
        public IActionResult ParosDirectory()
        {
            return View();
        }

        /// <summary>
        /// Vista de directorio Contadores - solo muestra el apartado de Contadores y Producción del dashboard
        /// </summary>
        public IActionResult ContadoresDirectory()
        {
            return View();
        }

        /// <summary>
        /// Vista de directorio Fallas - solo muestra el apartado de Gestión de Fallas del dashboard
        /// </summary>
        public IActionResult FallasDirectory()
        {
            return View();
        }

        /// <summary>
        /// Calcula OEE en vivo para el turno actual de una máquina. NO guarda en BD.
        /// GET /Linealytics/GetOeeActual?maquinaId=1&amp;turnoId=2&amp;fecha=2026-03-28
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOeeActual(int maquinaId, int turnoId, DateTime? fecha)
        {
            try
            {
                var fechaCalculo = fecha?.ToUniversalTime() ?? DateTime.UtcNow;
                var resultado = await _oeeService.CalcularPorTurnoAsync(
                    maquinaId, turnoId, fechaCalculo, guardar: false);
                return Json(resultado);
            }
            catch (ArgumentException ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Calcula y guarda OEE. Acepta por turno o por rango libre.
        /// POST /Linealytics/CalcularOee
        /// Body: { maquinaId, turnoId?, fecha?, fechaInicio?, fechaFin? }
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CalcularOee([FromBody] CalcularOeeRequest request)
        {
            if (request == null || request.MaquinaId <= 0)
                return BadRequest(new { error = "MaquinaId es requerido." });

            try
            {
                OeeResultDto resultado;

                if (request.TurnoId.HasValue && request.Fecha.HasValue)
                {
                    // Por turno
                    resultado = await _oeeService.CalcularPorTurnoAsync(
                        request.MaquinaId,
                        request.TurnoId.Value,
                        request.Fecha.Value.ToUniversalTime(),
                        guardar: true);
                }
                else if (request.FechaInicio.HasValue && request.FechaFin.HasValue)
                {
                    // Por rango libre
                    resultado = await _oeeService.CalcularPorRangoAsync(
                        request.MaquinaId,
                        request.FechaInicio.Value.ToUniversalTime(),
                        request.FechaFin.Value.ToUniversalTime(),
                        guardar: true);
                }
                else
                {
                    return BadRequest(new { error = "Especifica (turnoId + fecha) o (fechaInicio + fechaFin)." });
                }

                return Json(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Retorna el historial de MetricasMaquina guardadas para una máquina.
        /// GET /Linealytics/GetOeeHistorial?maquinaId=1&amp;dias=30
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOeeHistorial(int maquinaId, int dias = 30)
        {
            var desde = DateTime.UtcNow.AddDays(-dias);

            var historial = await _context.MetricasMaquina
                .Include(m => m.Turno)
                .Include(m => m.Maquina)
                .Include(m => m.Producto)
                .Where(m => m.MaquinaId == maquinaId && m.FechaInicio >= desde)
                .OrderByDescending(m => m.FechaInicio)
                .Select(m => new
                {
                    m.Id,
                    m.MaquinaId,
                    MaquinaNombre = m.Maquina.Nombre,
                    m.TurnoId,
                    TurnoNombre = m.Turno != null ? m.Turno.Nombre : "Rango libre",
                    m.FechaInicio,
                    m.FechaFin,
                    m.TiempoDisponibleMinutos,
                    m.TiempoParoMinutos,
                    m.TiempoProduccionMinutos,
                    m.UnidadesBuenas,
                    m.UnidadesDefectuosas,
                    m.UnidadesProducidas,
                    m.DisponibilidadPorcentaje,
                    m.RendimientoPorcentaje,
                    m.CalidadPorcentaje,
                    m.OeePorcentaje,
                    m.Cerrada,
                    ProductoNombre = m.Producto != null ? m.Producto.Nombre : null
                })
                .ToListAsync();

            return Json(historial);
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.Id == id);
        }

        private static int CalcularDuracionTurno(TimeSpan inicio, TimeSpan fin)
        {
            // Soporta turnos nocturnos que cruzan medianoche
            var duracion = fin > inicio ? fin - inicio : TimeSpan.FromHours(24) - inicio + fin;
            return (int)Math.Round(duracion.TotalMinutes);
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        private bool BotonExists(int id)
        {
            return _context.Botones.Any(e => e.Id == id);
        }

        private bool BotoneraExists(int id)
        {
            return _context.Botoneras.Any(e => e.Id == id);
        }

        // ========== ESTADO DE MÁQUINAS ==========

        public async Task<IActionResult> EstadoMaquinas()
        {
            ViewBag.Areas = await _context.Areas
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            ViewBag.Lineas = await _context.Lineas
                .Include(l => l.Area)
                .Where(l => l.Activo)
                .OrderBy(l => l.Area.Nombre)
                .ThenBy(l => l.Nombre)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEstadoMaquinas(int? areaId, int? lineaId, string? rangoTiempo)
        {
            var ahora = DateTime.UtcNow;
            
            // Calcular fechas según rango de tiempo
            DateTime fechaInicio = ahora;
            
            if (string.IsNullOrEmpty(rangoTiempo))
                rangoTiempo = "semana";
            
            switch (rangoTiempo.ToLower())
            {
                case "turno-actual":
                    // Obtener el turno actual según la hora actual
                    var horaAhora = ahora.TimeOfDay;
                    var turnoActual = await _context.Turnos
                        .Where(t => t.Activo && t.HoraInicio <= horaAhora && t.HoraFin > horaAhora)
                        .FirstOrDefaultAsync();
                    
                    if (turnoActual != null)
                    {
                        // Calcular la fecha del turno actual
                        var inicio = new DateTime(ahora.Year, ahora.Month, ahora.Day, 
                            turnoActual.HoraInicio.Hours, turnoActual.HoraInicio.Minutes, 0);
                        if (inicio > ahora)
                            inicio = inicio.AddDays(-1); // Si el turno cruza medianoche, ajustar
                        fechaInicio = inicio;
                    }
                    else
                    {
                        fechaInicio = ahora.AddHours(-8); // Default si no hay turno
                    }
                    break;
                    
                case "24h":
                    fechaInicio = ahora.AddHours(-24);
                    break;
                    
                case "semana":
                    fechaInicio = ahora.AddDays(-7);
                    break;
                    
                case "mes":
                    fechaInicio = ahora.AddDays(-30);
                    break;
                    
                default:
                    fechaInicio = ahora.AddDays(-7);
                    break;
            }

            // 1. Máquinas activas con jerarquía completa
            var maquinasQuery = _context.Maquinas
                .Include(m => m.Estacion)
                    .ThenInclude(e => e.Linea)
                        .ThenInclude(l => l.Area)
                .Where(m => m.Activo);

            if (areaId.HasValue)
                maquinasQuery = maquinasQuery.Where(m => m.Estacion.Linea.AreaId == areaId.Value);

            if (lineaId.HasValue)
                maquinasQuery = maquinasQuery.Where(m => m.Estacion.LineaId == lineaId.Value);

            var maquinas = await maquinasQuery
                .OrderBy(m => m.Estacion.Linea.Area.Nombre)
                .ThenBy(m => m.Estacion.Linea.Nombre)
                .ThenBy(m => m.Estacion.Nombre)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

            var maquinaIds = maquinas.Select(m => m.Id).ToList();

            // 2. Paros dentro del rango de tiempo
            var parosAbiertos = await _context.RegistrosParoBotonera
                .Include(p => p.DepartamentoOperador)
                .Include(p => p.Boton)
                .Where(p => maquinaIds.Contains(p.MaquinaId) && p.Estado == "Abierto")
                .ToListAsync();

            var parosCerrados = await _context.RegistrosParoBotonera
                .Include(p => p.DepartamentoOperador)
                .Include(p => p.Boton)
                .Where(p => maquinaIds.Contains(p.MaquinaId) 
                    && p.Estado == "Cerrado"
                    && p.FechaHoraInicio >= fechaInicio
                    && p.DuracionMinutos.HasValue)
                .ToListAsync();

            // 3. Corridas activas con producto (filtradas por rango de tiempo)
            var corridasActivas = await _context.CorridasProduccion
                .Include(c => c.Producto)
                .Where(c => maquinaIds.Contains(c.MaquinaId) 
                    && c.Estado == "Activa"
                    && c.FechaInicio >= fechaInicio)
                .ToListAsync();

            // 4. Fallas activas (Pendiente o EnAtencion)
            var fallasActivas = await _context.RegistrosFallas
                .Include(f => f.CatalogoFalla)
                .Where(f => maquinaIds.Contains(f.MaquinaId) && f.Estado != "Resuelta")
                .ToListAsync();

            // 5. Ensamblar resultado por máquina
            var resultado = maquinas.Select(m =>
            {
                var paro = parosAbiertos.FirstOrDefault(p => p.MaquinaId == m.Id);
                var corrida = corridasActivas.FirstOrDefault(c => c.MaquinaId == m.Id);
                var fallas = fallasActivas.Where(f => f.MaquinaId == m.Id).ToList();

                string estado;
                if (paro != null)
                    estado = "EnParo";
                else if (corrida != null)
                    estado = "EnProduccion";
                else
                    estado = "SinActividad";

                return new
                {
                    maquinaId = m.Id,
                    nombre = m.Nombre,
                    codigo = m.Codigo,
                    area = m.Estacion?.Linea?.Area?.Nombre,
                    linea = m.Estacion?.Linea?.Nombre,
                    estacion = m.Estacion?.Nombre,
                    estado,
                    paro = paro == null ? null : new
                    {
                        id = paro.Id,
                        departamento = paro.DepartamentoOperador?.Nombre,
                        departamentoColor = paro.DepartamentoOperador?.Color ?? "#666666",
                        boton = paro.Boton?.Nombre,
                        inicio = paro.FechaHoraInicio,
                        duracionMinutos = (int)Math.Round((ahora - paro.FechaHoraInicio).TotalMinutes)
                    },
                    corrida = corrida == null ? null : new
                    {
                        id = corrida.Id,
                        productoCodigo = corrida.Producto?.Codigo,
                        productoNombre = corrida.Producto?.Nombre,
                        produccionOK = corrida.ProduccionOK,
                        produccionNOK = corrida.ProduccionNOK,
                        inicio = corrida.FechaInicio,
                        lecturas = corrida.NumeroLecturas
                    },
                    fallas = fallas.Select(f => new
                    {
                        id = f.Id,
                        tipo = f.CatalogoFalla?.Nombre,
                        severidad = f.CatalogoFalla?.Severidad,
                        color = f.CatalogoFalla?.Color ?? "#666666",
                        estadoFalla = f.Estado,
                        inicio = f.FechaHoraDeteccion
                    }).ToList(),
                    totalFallas = fallas.Count,
                    actualizadoEn = ahora
                };
            }).ToList();

            return Json(resultado);
        }

        // ========== DASHBOARD DE LÍNEAS ==========

        public async Task<IActionResult> EstadoLineas()
        {
            ViewBag.Areas = await _context.Areas
                .Where(a => a.Activo)
                .OrderBy(a => a.Nombre)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEstadoLineas(int? areaId, string? rangoTiempo)
        {
            var ahora = DateTime.UtcNow;
            
            // Calcular fechas según rango de tiempo
            DateTime fechaInicio = ahora;
            
            if (string.IsNullOrEmpty(rangoTiempo))
                rangoTiempo = "semana";
            
            switch (rangoTiempo.ToLower())
            {
                case "turno-actual":
                    // Obtener el turno actual según la hora actual
                    var horaAhora = ahora.TimeOfDay;
                    var turnoActual = await _context.Turnos
                        .Where(t => t.Activo && t.HoraInicio <= horaAhora && t.HoraFin > horaAhora)
                        .FirstOrDefaultAsync();
                    
                    if (turnoActual != null)
                    {
                        // Calcular la fecha del turno actual
                        var inicio = new DateTime(ahora.Year, ahora.Month, ahora.Day, 
                            turnoActual.HoraInicio.Hours, turnoActual.HoraInicio.Minutes, 0);
                        if (inicio > ahora)
                            inicio = inicio.AddDays(-1); // Si el turno cruza medianoche, ajustar
                        fechaInicio = inicio;
                    }
                    else
                    {
                        fechaInicio = ahora.AddHours(-8); // Default si no hay turno
                    }
                    break;
                    
                case "24h":
                    fechaInicio = ahora.AddHours(-24);
                    break;
                    
                case "semana":
                    fechaInicio = ahora.AddDays(-7);
                    break;
                    
                case "mes":
                    fechaInicio = ahora.AddDays(-30);
                    break;
                    
                default:
                    fechaInicio = ahora.AddDays(-7);
                    break;
            }

            var lineasQuery = _context.Lineas
                .Include(l => l.Area)
                .Where(l => l.Activo);

            if (areaId.HasValue)
                lineasQuery = lineasQuery.Where(l => l.AreaId == areaId.Value);

            var lineas = await lineasQuery
                .OrderBy(l => l.Area!.Nombre)
                .ThenBy(l => l.Nombre)
                .ToListAsync();

            var lineaIds = lineas.Select(l => l.Id).ToList();

            var maquinas = await _context.Maquinas
                .Include(m => m.Estacion)
                .Where(m => m.Activo && lineaIds.Contains(m.Estacion.LineaId))
                .ToListAsync();

            var maquinaIds = maquinas.Select(m => m.Id).ToList();

            var parosAbiertos = await _context.RegistrosParoBotonera
                .Include(p => p.DepartamentoOperador)
                .Where(p => maquinaIds.Contains(p.MaquinaId) && p.Estado == "Abierto")
                .ToListAsync();

            var corridasActivas = await _context.CorridasProduccion
                .Where(c => maquinaIds.Contains(c.MaquinaId) 
                    && c.Estado == "Activa"
                    && c.FechaInicio >= fechaInicio)
                .ToListAsync();

            var fallasActivas = await _context.RegistrosFallas
                .Include(f => f.CatalogoFalla)
                .Where(f => maquinaIds.Contains(f.MaquinaId) && f.Estado != "Resuelta")
                .ToListAsync();

            var parosCerrados = await _context.RegistrosParoBotonera
                .Where(p => maquinaIds.Contains(p.MaquinaId)
                    && p.Estado == "Cerrado"
                    && p.FechaHoraInicio >= fechaInicio
                    && p.DuracionMinutos.HasValue)
                .ToListAsync();

            var resultado = lineas.Select(l =>
            {
                var maqsLinea  = maquinas.Where(m => m.Estacion.LineaId == l.Id).ToList();
                var idsLinea   = maqsLinea.Select(m => m.Id).ToList();
                var total      = maqsLinea.Count;

                var nParo = idsLinea.Count(id => parosAbiertos.Any(p => p.MaquinaId == id));
                var nProd = idsLinea.Count(id =>
                    !parosAbiertos.Any(p => p.MaquinaId == id) &&
                    corridasActivas.Any(c => c.MaquinaId == id));
                var nSin  = total - nParo - nProd;

                var prodOK  = corridasActivas.Where(c => idsLinea.Contains(c.MaquinaId)).Sum(c => c.ProduccionOK);
                var prodNOK = corridasActivas.Where(c => idsLinea.Contains(c.MaquinaId)).Sum(c => c.ProduccionNOK);

                var minCerrados = parosCerrados
                    .Where(p => idsLinea.Contains(p.MaquinaId))
                    .Sum(p => p.DuracionMinutos ?? 0);
                var minAbiertos = parosAbiertos
                    .Where(p => idsLinea.Contains(p.MaquinaId))
                    .Sum(p => (int)Math.Round((ahora - p.FechaHoraInicio).TotalMinutes));
                var minParosTotal = minCerrados + minAbiertos;

                string semaforo;
                if (total == 0)                               semaforo = "Gris";
                else if (nParo == 0 && nProd > 0)            semaforo = "Verde";
                else if (nParo > 0 && nParo < total)         semaforo = "Amarillo";
                else if (nParo > 0 && nParo == total)        semaforo = "Rojo";
                else                                          semaforo = "Gris";

                var fallasLinea     = fallasActivas.Where(f => idsLinea.Contains(f.MaquinaId)).ToList();
                var fallasCriticas  = fallasLinea.Count(f => f.CatalogoFalla?.Severidad == "Crítica");
                var fallasAltas     = fallasLinea.Count(f => f.CatalogoFalla?.Severidad == "Alta");

                var topDept = parosAbiertos
                    .Where(p => idsLinea.Contains(p.MaquinaId))
                    .GroupBy(p => new { p.DepartamentoOperador?.Nombre, p.DepartamentoOperador?.Color })
                    .OrderByDescending(g => g.Count())
                    .Select(g => new { nombre = g.Key.Nombre, color = g.Key.Color ?? "#666666", cantidad = g.Count() })
                    .FirstOrDefault();

                var maquinasEnParoDetalle = parosAbiertos
                    .Where(p => idsLinea.Contains(p.MaquinaId))
                    .Select(p => new
                    {
                        nombre       = maqsLinea.FirstOrDefault(m => m.Id == p.MaquinaId)?.Nombre ?? "—",
                        departamento = p.DepartamentoOperador?.Nombre,
                        color        = p.DepartamentoOperador?.Color ?? "#666666",
                        duracionMinutos = (int)Math.Round((ahora - p.FechaHoraInicio).TotalMinutes)
                    }).ToList();

                return new
                {
                    lineaId = l.Id,
                    nombre  = l.Nombre,
                    area    = l.Area?.Nombre,
                    semaforo,
                    totalMaquinas          = total,
                    maquinasEnParo         = nParo,
                    maquinasEnProduccion   = nProd,
                    maquinasSinActividad   = nSin,
                    produccionOK           = prodOK,
                    produccionNOK          = prodNOK,
                    minutosParo            = minParosTotal,
                    totalFallas            = fallasLinea.Count,
                    fallasCriticas,
                    fallasAltas,
                    topDepartamento        = topDept,
                    maquinasEnParoDetalle,
                    actualizadoEn          = ahora
                };
            }).ToList();

            return Json(resultado);
        }
    }

    // Request DTO para CalcularOee
    public class CalcularOeeRequest
    {
        public int MaquinaId { get; set; }
        public int? TurnoId { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
