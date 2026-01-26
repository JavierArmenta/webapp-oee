using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Linealytics;

namespace WebApp.Controllers
{
    [Authorize]
    public class LinealyticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LinealyticsController(ApplicationDbContext context)
        {
            _context = context;
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

        [HttpGet]
        public async Task<IActionResult> GetDatosParosPorDepartamento(int? areaId, int? lineaId, int? estacionId, int? maquinaId)
        {
            var fechaInicio = DateTime.UtcNow.AddDays(-7); // Últimos 7 días

            // Query base de paros de botonera
            var query = _context.RegistrosParoBotonera
                .Include(p => p.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                            .ThenInclude(l => l.Area)
                .Include(p => p.DepartamentoOperador)
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

            var paros = await query
                .OrderBy(p => p.FechaHoraInicio)
                .ToListAsync();

            // Preparar datos para la gráfica: cada paro individual con su información
            var datosParos = paros
                .Where(p => p.DepartamentoOperador != null)
                .Select(p => new
                {
                    Maquina = p.Maquina.Nombre,
                    MaquinaId = p.MaquinaId,
                    Departamento = p.DepartamentoOperador!.Nombre,
                    Color = p.DepartamentoOperador.Color ?? "#666666",
                    FechaInicio = p.FechaHoraInicio,
                    FechaFin = p.FechaHoraFin,
                    DuracionMinutos = p.DuracionMinutos ?? 0
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
            var fechaMin = paros.Any() ? paros.Min(p => p.FechaHoraInicio) : DateTime.UtcNow;
            var fechaMax = paros.Any() ? paros.Max(p => p.FechaHoraFin ?? p.FechaHoraInicio) : DateTime.UtcNow;

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
        public async Task<IActionResult> CreateTurno([Bind("Nombre,HoraInicio,HoraFin,DuracionMinutos")] Turno turno)
        {
            if (ModelState.IsValid)
            {
                turno.Activo = true;
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
        public async Task<IActionResult> EditTurno(int id, [Bind("Id,Nombre,HoraInicio,HoraFin,DuracionMinutos,Activo")] Turno turno)
        {
            if (id != turno.Id) return NotFound();

            if (ModelState.IsValid)
            {
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

        // ========== CATEGORÍAS DE PARO ==========

        public async Task<IActionResult> CategoriasParo()
        {
            var categorias = await _context.CategoriasParo
                .Include(c => c.CausasParo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            return View(categorias);
        }

        public IActionResult CreateCategoriaParo()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategoriaParo([Bind("Nombre,Descripcion,Color,EsPlaneado")] CategoriaParo categoria)
        {
            if (ModelState.IsValid)
            {
                categoria.Activo = true;
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoría de paro creada exitosamente.";
                return RedirectToAction(nameof(CategoriasParo));
            }
            return View(categoria);
        }

        public async Task<IActionResult> EditCategoriaParo(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _context.CategoriasParo.FindAsync(id);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategoriaParo(int id, [Bind("Id,Nombre,Descripcion,Color,EsPlaneado,Activo")] CategoriaParo categoria)
        {
            if (id != categoria.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Categoría de paro actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaParoExists(categoria.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(CategoriasParo));
            }
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateCategoriaParo(int id)
        {
            var categoria = await _context.CategoriasParo.FindAsync(id);
            if (categoria != null)
            {
                categoria.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoría de paro desactivada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la categoría de paro.";
            }
            return RedirectToAction(nameof(CategoriasParo));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateCategoriaParo(int id)
        {
            var categoria = await _context.CategoriasParo.FindAsync(id);
            if (categoria != null)
            {
                categoria.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoría de paro activada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la categoría de paro.";
            }
            return RedirectToAction(nameof(CategoriasParo));
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

        // ========== MÉTODOS AUXILIARES ==========

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.Id == id);
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }

        private bool CategoriaParoExists(int id)
        {
            return _context.CategoriasParo.Any(e => e.Id == id);
        }

        private bool BotonExists(int id)
        {
            return _context.Botones.Any(e => e.Id == id);
        }

        private bool BotoneraExists(int id)
        {
            return _context.Botoneras.Any(e => e.Id == id);
        }
    }
}
