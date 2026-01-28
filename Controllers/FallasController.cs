using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models.Linealytics;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controlador para gestión de fallas - Sistema independiente de paros
    /// </summary>
    [Authorize]
    public class FallasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FallasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== CATÁLOGO DE FALLAS ==========

        public async Task<IActionResult> CatalogoFallas()
        {
            var catalogos = await _context.CatalogoFallas
                .OrderBy(c => c.Codigo)
                .ToListAsync();
            return View(catalogos);
        }

        public IActionResult CreateCatalogoFalla()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCatalogoFalla([Bind("Codigo,Nombre,Descripcion,Severidad,Categoria,TiempoEstimadoSolucionMinutos,Color")] CatalogoFalla catalogo)
        {
            if (ModelState.IsValid)
            {
                // Verificar que el código no exista
                var existeCodigo = await _context.CatalogoFallas
                    .AnyAsync(c => c.Codigo == catalogo.Codigo);

                if (existeCodigo)
                {
                    ModelState.AddModelError("Codigo", "Ya existe un catálogo de falla con este código.");
                    return View(catalogo);
                }

                catalogo.Activo = true;
                catalogo.FechaCreacion = DateTime.UtcNow;
                _context.Add(catalogo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Catálogo de falla creado exitosamente.";
                return RedirectToAction(nameof(CatalogoFallas));
            }
            return View(catalogo);
        }

        public async Task<IActionResult> EditCatalogoFalla(int? id)
        {
            if (id == null) return NotFound();

            var catalogo = await _context.CatalogoFallas.FindAsync(id);
            if (catalogo == null) return NotFound();

            return View(catalogo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCatalogoFalla(int id, [Bind("Id,Codigo,Nombre,Descripcion,Severidad,Categoria,TiempoEstimadoSolucionMinutos,Color,Activo")] CatalogoFalla catalogo)
        {
            if (id != catalogo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el código no exista en otro registro
                    var existeCodigo = await _context.CatalogoFallas
                        .AnyAsync(c => c.Codigo == catalogo.Codigo && c.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un catálogo de falla con este código.");
                        return View(catalogo);
                    }

                    var catalogoExistente = await _context.CatalogoFallas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                    if (catalogoExistente == null) return NotFound();

                    catalogo.FechaCreacion = catalogoExistente.FechaCreacion;
                    catalogo.FechaModificacion = DateTime.UtcNow;

                    _context.Update(catalogo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Catálogo de falla actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatalogoFallaExists(catalogo.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(CatalogoFallas));
            }
            return View(catalogo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateCatalogoFalla(int id)
        {
            var catalogo = await _context.CatalogoFallas.FindAsync(id);
            if (catalogo != null)
            {
                catalogo.Activo = false;
                catalogo.FechaModificacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Catálogo de falla desactivado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el catálogo de falla.";
            }
            return RedirectToAction(nameof(CatalogoFallas));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateCatalogoFalla(int id)
        {
            var catalogo = await _context.CatalogoFallas.FindAsync(id);
            if (catalogo != null)
            {
                catalogo.Activo = true;
                catalogo.FechaModificacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Catálogo de falla activado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el catálogo de falla.";
            }
            return RedirectToAction(nameof(CatalogoFallas));
        }

        // ========== REGISTROS DE FALLAS ==========

        public async Task<IActionResult> RegistrosFallas(int? maquinaId, string? estado, int? catalogoFallaId)
        {
            var query = _context.RegistrosFallas
                .Include(r => r.CatalogoFalla)
                .Include(r => r.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                .Include(r => r.TecnicoAsignado)
                .AsQueryable();

            if (maquinaId.HasValue)
                query = query.Where(r => r.MaquinaId == maquinaId.Value);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(r => r.Estado == estado);

            if (catalogoFallaId.HasValue)
                query = query.Where(r => r.CatalogoFallaId == catalogoFallaId.Value);

            var registros = await query
                .OrderByDescending(r => r.FechaHoraDeteccion)
                .Take(100)
                .ToListAsync();

            // Cargar datos para filtros
            ViewBag.Maquinas = new SelectList(
                await _context.Maquinas.Where(m => m.Activo).OrderBy(m => m.Nombre).ToListAsync(),
                "Id", "Nombre", maquinaId);

            ViewBag.CatalogosFallas = new SelectList(
                await _context.CatalogoFallas.Where(c => c.Activo).OrderBy(c => c.Nombre).ToListAsync(),
                "Id", "Nombre", catalogoFallaId);

            ViewBag.MaquinaIdSeleccionado = maquinaId;
            ViewBag.EstadoSeleccionado = estado;
            ViewBag.CatalogoFallaIdSeleccionado = catalogoFallaId;

            return View(registros);
        }

        public async Task<IActionResult> DetalleFalla(int? id)
        {
            if (id == null) return NotFound();

            var registro = await _context.RegistrosFallas
                .Include(r => r.CatalogoFalla)
                .Include(r => r.Maquina)
                    .ThenInclude(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                            .ThenInclude(l => l.Area)
                .Include(r => r.TecnicoAsignado)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registro == null) return NotFound();

            // Cargar operadores para asignar como técnicos
            ViewBag.Tecnicos = new SelectList(
                await _context.Operadores.Where(o => o.Activo).OrderBy(o => o.Nombre).ThenBy(o => o.Apellido).ToListAsync(),
                "Id", "NombreCompleto", registro.TecnicoAsignadoId);

            return View(registro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarTecnico(int id, int tecnicoId)
        {
            var registro = await _context.RegistrosFallas.FindAsync(id);

            if (registro == null) return NotFound();

            registro.TecnicoAsignadoId = tecnicoId;
            registro.Estado = "EnAtencion";
            registro.FechaHoraAtencion = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Técnico asignado exitosamente.";
            return RedirectToAction(nameof(DetalleFalla), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolverFalla(int id, string accionesTomadas)
        {
            var registro = await _context.RegistrosFallas.FindAsync(id);

            if (registro == null) return NotFound();

            if (string.IsNullOrWhiteSpace(accionesTomadas))
            {
                TempData["Error"] = "Debe describir las acciones tomadas para resolver la falla.";
                return RedirectToAction(nameof(DetalleFalla), new { id });
            }

            registro.Estado = "Resuelta";
            registro.FechaHoraResolucion = DateTime.UtcNow;
            registro.AccionesTomadas = accionesTomadas;

            if (registro.FechaHoraAtencion.HasValue)
            {
                registro.DuracionMinutos = (int)(registro.FechaHoraResolucion.Value - registro.FechaHoraAtencion.Value).TotalMinutes;
            }
            else
            {
                // Si no hay fecha de atención, usar fecha de detección
                registro.DuracionMinutos = (int)(registro.FechaHoraResolucion.Value - registro.FechaHoraDeteccion).TotalMinutes;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Falla resuelta exitosamente.";
            return RedirectToAction(nameof(RegistrosFallas));
        }

        // ========== DASHBOARD DE FALLAS ==========

        public async Task<IActionResult> DashboardFallas()
        {
            ViewBag.Maquinas = await _context.Maquinas
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            ViewBag.CatalogosFallas = await _context.CatalogoFallas
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDatosFallas(DateTime? fechaInicio, DateTime? fechaFin, int? maquinaId, int? catalogoFallaId)
        {
            try
            {
                fechaInicio ??= DateTime.UtcNow.AddDays(-30);
                fechaFin ??= DateTime.UtcNow;

                // Convertir fechas a UTC para compatibilidad con PostgreSQL timestamptz
                if (fechaInicio.Value.Kind == DateTimeKind.Unspecified)
                    fechaInicio = DateTime.SpecifyKind(fechaInicio.Value, DateTimeKind.Utc);

                if (fechaFin.Value.Kind == DateTimeKind.Unspecified)
                    fechaFin = DateTime.SpecifyKind(fechaFin.Value, DateTimeKind.Utc);

                var query = _context.RegistrosFallas
                    .Include(r => r.CatalogoFalla)
                    .Include(r => r.Maquina)
                    .Where(r => r.FechaHoraDeteccion >= fechaInicio && r.FechaHoraDeteccion <= fechaFin);

                if (maquinaId.HasValue)
                    query = query.Where(r => r.MaquinaId == maquinaId.Value);

                if (catalogoFallaId.HasValue)
                    query = query.Where(r => r.CatalogoFallaId == catalogoFallaId.Value);

                var fallas = await query.ToListAsync();

            // Top 10 fallas más frecuentes
            var top10Fallas = fallas
                .GroupBy(f => new { f.CatalogoFallaId, f.CatalogoFalla.Nombre, f.CatalogoFalla.Color })
                .Select(g => new
                {
                    Nombre = g.Key.Nombre,
                    Color = g.Key.Color ?? "#666666",
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(10)
                .ToList();

            // Fallas por severidad
            var porSeveridad = fallas
                .GroupBy(f => f.CatalogoFalla.Severidad ?? "Sin clasificar")
                .Select(g => new
                {
                    Severidad = g.Key,
                    Cantidad = g.Count()
                })
                .ToList();

            // Fallas por categoría
            var porCategoria = fallas
                .GroupBy(f => f.CatalogoFalla.Categoria ?? "Sin clasificar")
                .Select(g => new
                {
                    Categoria = g.Key,
                    Cantidad = g.Count()
                })
                .ToList();

            // MTTR (Mean Time To Repair) - Solo fallas resueltas
            var fallasResueltas = fallas
                .Where(f => f.Estado == "Resuelta" && f.DuracionMinutos.HasValue)
                .ToList();

            var mttr = fallasResueltas.Any()
                ? fallasResueltas.Average(f => f.DuracionMinutos!.Value)
                : 0;

            // Fallas por máquina
            var porMaquina = fallas
                .GroupBy(f => new { f.MaquinaId, f.Maquina.Nombre })
                .Select(g => new
                {
                    Maquina = g.Key.Nombre,
                    Cantidad = g.Count()
                })
                .OrderByDescending(x => x.Cantidad)
                .Take(10)
                .ToList();

            // Tendencia diaria
            var tendenciaDiaria = fallas
                .GroupBy(f => f.FechaHoraDeteccion.Date)
                .Select(g => new
                {
                    Fecha = g.Key.ToString("dd/MM"),
                    Cantidad = g.Count()
                })
                .OrderBy(x => x.Fecha)
                .ToList();

                return Json(new
                {
                    top10Fallas,
                    porSeveridad,
                    porCategoria,
                    porMaquina,
                    tendenciaDiaria,
                    mttr = Math.Round(mttr, 2),
                    totalFallas = fallas.Count,
                    fallasAbiertas = fallas.Count(f => f.Estado == "Pendiente"),
                    fallasEnAtencion = fallas.Count(f => f.Estado == "EnAtencion"),
                    fallasResueltas = fallasResueltas.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    mensaje = "Error al cargar los datos de fallas",
                    detalle = ex.Message,
                    top10Fallas = new List<object>(),
                    porSeveridad = new List<object>(),
                    porCategoria = new List<object>(),
                    porMaquina = new List<object>(),
                    tendenciaDiaria = new List<object>(),
                    mttr = 0,
                    totalFallas = 0,
                    fallasAbiertas = 0,
                    fallasEnAtencion = 0,
                    fallasResueltas = 0
                });
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool CatalogoFallaExists(int id)
        {
            return _context.CatalogoFallas.Any(e => e.Id == id);
        }
    }
}
