using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public class PlantaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlantaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ========== ÁREAS ==========

        // GET: Planta/Areas
        public async Task<IActionResult> Areas()
        {
            var areas = await _context.Areas
                .Include(a => a.Lineas)
                .OrderBy(a => a.Nombre)
                .ToListAsync();
            return View(areas);
        }

        // GET: Planta/AreaDetails/5
        public async Task<IActionResult> AreaDetails(int? id)
        {
            if (id == null) return NotFound();

            var area = await _context.Areas
                .Include(a => a.Lineas)
                    .ThenInclude(l => l.Estaciones)
                        .ThenInclude(e => e.Maquinas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (area == null) return NotFound();

            return View(area);
        }

        // GET: Planta/CreateArea
        public IActionResult CreateArea()
        {
            return View();
        }

        // POST: Planta/CreateArea
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArea([Bind("Nombre,Descripcion,Codigo,Activo")] Area area)
        {
            // Remover validación de propiedades de navegación
            ModelState.Remove("Lineas");

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(area.Codigo) && await _context.Areas.AnyAsync(a => a.Codigo == area.Codigo))
                {
                    ModelState.AddModelError("Codigo", "Este código ya existe.");
                    return View(area);
                }

                area.FechaCreacion = DateTime.UtcNow;
                _context.Add(area);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Área creada exitosamente.";
                return RedirectToAction(nameof(Areas));
            }
            return View(area);
        }

        // GET: Planta/EditArea/5
        public async Task<IActionResult> EditArea(int? id)
        {
            if (id == null) return NotFound();

            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();
            
            return View(area);
        }

        // POST: Planta/EditArea/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArea(int id, [Bind("Id,Nombre,Descripcion,Codigo,Activo")] Area area)
        {
            if (id != area.Id) return NotFound();

            // Remover validación de propiedades de navegación
            ModelState.Remove("Lineas");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(area.Codigo) && await _context.Areas.AnyAsync(a => a.Codigo == area.Codigo && a.Id != id))
                    {
                        ModelState.AddModelError("Codigo", "Este código ya existe.");
                        return View(area);
                    }

                    var areaExistente = await _context.Areas.FindAsync(id);
                    if (areaExistente == null) return NotFound();

                    areaExistente.Nombre = area.Nombre;
                    areaExistente.Descripcion = area.Descripcion;
                    areaExistente.Codigo = area.Codigo;
                    areaExistente.Activo = area.Activo;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Área actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Areas.AnyAsync(e => e.Id == area.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Areas));
            }
            return View(area);
        }

        // POST: Planta/DeactivateArea/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateArea(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area != null)
            {
                area.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Área desactivada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el área.";
            }

            return RedirectToAction(nameof(Areas));
        }

        // POST: Planta/ActivateArea/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateArea(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area != null)
            {
                area.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Área activada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el área.";
            }

            return RedirectToAction(nameof(Areas));
        }

        // ========== LÍNEAS ==========

        // GET: Planta/Lineas
        public async Task<IActionResult> Lineas()
        {
            var lineas = await _context.Lineas
                .Include(l => l.Area)
                .Include(l => l.Estaciones)
                .OrderBy(l => l.Area.Nombre)
                .ThenBy(l => l.Nombre)
                .ToListAsync();
            return View(lineas);
        }

        // GET: Planta/LineaDetails/5
        public async Task<IActionResult> LineaDetails(int? id)
        {
            if (id == null) return NotFound();

            var linea = await _context.Lineas
                .Include(l => l.Area)
                .Include(l => l.Estaciones)
                    .ThenInclude(e => e.Maquinas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (linea == null) return NotFound();

            return View(linea);
        }

        // GET: Planta/CreateLinea
        public async Task<IActionResult> CreateLinea()
        {
            ViewBag.Areas = new SelectList(await _context.Areas.Where(a => a.Activo).OrderBy(a => a.Nombre).ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: Planta/CreateLinea
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLinea([Bind("Nombre,Descripcion,Codigo,AreaId,Activo")] Linea linea)
        {
            // Remover validación de propiedades de navegación
            ModelState.Remove("Area");
            ModelState.Remove("Estaciones");

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(linea.Codigo) && await _context.Lineas.AnyAsync(l => l.Codigo == linea.Codigo))
                {
                    ModelState.AddModelError("Codigo", "Este código ya existe.");
                    ViewBag.Areas = new SelectList(await _context.Areas.Where(a => a.Activo).ToListAsync(), "Id", "Nombre", linea.AreaId);
                    return View(linea);
                }

                linea.FechaCreacion = DateTime.UtcNow;
                _context.Add(linea);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Línea creada exitosamente.";
                return RedirectToAction(nameof(Lineas));
            }
            ViewBag.Areas = new SelectList(await _context.Areas.Where(a => a.Activo).ToListAsync(), "Id", "Nombre", linea.AreaId);
            return View(linea);
        }

        // GET: Planta/EditLinea/5
        public async Task<IActionResult> EditLinea(int? id)
        {
            if (id == null) return NotFound();

            var linea = await _context.Lineas.FindAsync(id);
            if (linea == null) return NotFound();
            
            ViewBag.Areas = new SelectList(await _context.Areas.Where(a => a.Activo).OrderBy(a => a.Nombre).ToListAsync(), "Id", "Nombre", linea.AreaId);
            return View(linea);
        }

        // POST: Planta/EditLinea/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLinea(int id, [Bind("Id,Nombre,Descripcion,Codigo,AreaId,Activo")] Linea linea)
        {
            if (id != linea.Id) return NotFound();

            // Remover validación de propiedades de navegación
            ModelState.Remove("Area");
            ModelState.Remove("Estaciones");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(linea.Codigo) && await _context.Lineas.AnyAsync(l => l.Codigo == linea.Codigo && l.Id != id))
                    {
                        ModelState.AddModelError("Codigo", "Este código ya existe.");
                        ViewBag.Areas = new SelectList(await _context.Areas.Where(a => a.Activo).ToListAsync(), "Id", "Nombre", linea.AreaId);
                        return View(linea);
                    }

                    var lineaExistente = await _context.Lineas.FindAsync(id);
                    if (lineaExistente == null) return NotFound();

                    lineaExistente.Nombre = linea.Nombre;
                    lineaExistente.Descripcion = linea.Descripcion;
                    lineaExistente.Codigo = linea.Codigo;
                    lineaExistente.AreaId = linea.AreaId;
                    lineaExistente.Activo = linea.Activo;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Línea actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Lineas.AnyAsync(e => e.Id == linea.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Lineas));
            }
            ViewBag.Areas = new SelectList(await _context.Areas.Where(a => a.Activo).ToListAsync(), "Id", "Nombre", linea.AreaId);
            return View(linea);
        }

        // POST: Planta/DeactivateLinea/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateLinea(int id)
        {
            var linea = await _context.Lineas.FindAsync(id);
            if (linea != null)
            {
                linea.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Línea desactivada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la línea.";
            }

            return RedirectToAction(nameof(Lineas));
        }

        // POST: Planta/ActivateLinea/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateLinea(int id)
        {
            var linea = await _context.Lineas.FindAsync(id);
            if (linea != null)
            {
                linea.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Línea activada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la línea.";
            }

            return RedirectToAction(nameof(Lineas));
        }

        // ========== ESTACIONES ==========

        // GET: Planta/Estaciones
        public async Task<IActionResult> Estaciones()
        {
            var estaciones = await _context.Estaciones
                .Include(e => e.Linea)
                    .ThenInclude(l => l.Area)
                .Include(e => e.Maquinas)
                .OrderBy(e => e.Linea.Area.Nombre)
                .ThenBy(e => e.Linea.Nombre)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
            return View(estaciones);
        }

        // GET: Planta/EstacionDetails/5
        public async Task<IActionResult> EstacionDetails(int? id)
        {
            if (id == null) return NotFound();

            var estacion = await _context.Estaciones
                .Include(e => e.Linea)
                    .ThenInclude(l => l.Area)
                .Include(e => e.Maquinas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (estacion == null) return NotFound();

            return View(estacion);
        }

        // GET: Planta/CreateEstacion
        public async Task<IActionResult> CreateEstacion()
        {
            await LoadLineasSelectList();
            return View();
        }

        // POST: Planta/CreateEstacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEstacion([Bind("Nombre,Descripcion,Codigo,LineaId,Activo")] Estacion estacion)
        {
            // Remover validación de propiedades de navegación
            ModelState.Remove("Linea");
            ModelState.Remove("Maquinas");

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(estacion.Codigo) && await _context.Estaciones.AnyAsync(e => e.Codigo == estacion.Codigo))
                {
                    ModelState.AddModelError("Codigo", "Este código ya existe.");
                    await LoadLineasSelectList(estacion.LineaId);
                    return View(estacion);
                }

                estacion.FechaCreacion = DateTime.UtcNow;
                _context.Add(estacion);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Estación creada exitosamente.";
                return RedirectToAction(nameof(Estaciones));
            }
            await LoadLineasSelectList(estacion.LineaId);
            return View(estacion);
        }

        // GET: Planta/EditEstacion/5
        public async Task<IActionResult> EditEstacion(int? id)
        {
            if (id == null) return NotFound();

            var estacion = await _context.Estaciones.FindAsync(id);
            if (estacion == null) return NotFound();
            
            await LoadLineasSelectList(estacion.LineaId);
            return View(estacion);
        }

        // POST: Planta/EditEstacion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEstacion(int id, [Bind("Id,Nombre,Descripcion,Codigo,LineaId,Activo")] Estacion estacion)
        {
            if (id != estacion.Id) return NotFound();

            // Remover validación de propiedades de navegación
            ModelState.Remove("Linea");
            ModelState.Remove("Maquinas");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(estacion.Codigo) && await _context.Estaciones.AnyAsync(e => e.Codigo == estacion.Codigo && e.Id != id))
                    {
                        ModelState.AddModelError("Codigo", "Este código ya existe.");
                        await LoadLineasSelectList(estacion.LineaId);
                        return View(estacion);
                    }

                    var estacionExistente = await _context.Estaciones.FindAsync(id);
                    if (estacionExistente == null) return NotFound();

                    estacionExistente.Nombre = estacion.Nombre;
                    estacionExistente.Descripcion = estacion.Descripcion;
                    estacionExistente.Codigo = estacion.Codigo;
                    estacionExistente.LineaId = estacion.LineaId;
                    estacionExistente.Activo = estacion.Activo;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Estación actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Estaciones.AnyAsync(e => e.Id == estacion.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Estaciones));
            }
            await LoadLineasSelectList(estacion.LineaId);
            return View(estacion);
        }

        // POST: Planta/DeactivateEstacion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateEstacion(int id)
        {
            var estacion = await _context.Estaciones.FindAsync(id);
            if (estacion != null)
            {
                estacion.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Estación desactivada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la estación.";
            }

            return RedirectToAction(nameof(Estaciones));
        }

        // POST: Planta/ActivateEstacion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateEstacion(int id)
        {
            var estacion = await _context.Estaciones.FindAsync(id);
            if (estacion != null)
            {
                estacion.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Estación activada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la estación.";
            }

            return RedirectToAction(nameof(Estaciones));
        }

        // ========== MÁQUINAS ==========

        // GET: Planta/Maquinas
        public async Task<IActionResult> Maquinas()
        {
            var maquinas = await _context.Maquinas
                .Include(m => m.Estacion)
                    .ThenInclude(e => e.Linea)
                        .ThenInclude(l => l.Area)
                .OrderBy(m => m.Estacion.Linea.Area.Nombre)
                .ThenBy(m => m.Estacion.Linea.Nombre)
                .ThenBy(m => m.Estacion.Nombre)
                .ThenBy(m => m.Nombre)
                .ToListAsync();
            return View(maquinas);
        }

        // GET: Planta/MaquinaDetails/5
        public async Task<IActionResult> MaquinaDetails(int? id)
        {
            if (id == null) return NotFound();

            var maquina = await _context.Maquinas
                .Include(m => m.Estacion)
                    .ThenInclude(e => e.Linea)
                        .ThenInclude(l => l.Area)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (maquina == null) return NotFound();

            return View(maquina);
        }

        // GET: Planta/CreateMaquina
        public async Task<IActionResult> CreateMaquina()
        {
            await LoadEstacionesSelectList();
            return View();
        }

        // POST: Planta/CreateMaquina
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaquina([Bind("Nombre,Descripcion,Codigo,NumeroSerie,Modelo,Fabricante,EstacionId,Activo")] Maquina maquina)
        {
            // Remover validación de propiedades de navegación
            ModelState.Remove("Estacion");

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(maquina.Codigo) && await _context.Maquinas.AnyAsync(m => m.Codigo == maquina.Codigo))
                {
                    ModelState.AddModelError("Codigo", "Este código ya existe.");
                    await LoadEstacionesSelectList(maquina.EstacionId);
                    return View(maquina);
                }

                if (!string.IsNullOrEmpty(maquina.NumeroSerie) && await _context.Maquinas.AnyAsync(m => m.NumeroSerie == maquina.NumeroSerie))
                {
                    ModelState.AddModelError("NumeroSerie", "Este número de serie ya existe.");
                    await LoadEstacionesSelectList(maquina.EstacionId);
                    return View(maquina);
                }

                maquina.FechaCreacion = DateTime.UtcNow;
                _context.Add(maquina);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Máquina creada exitosamente.";
                return RedirectToAction(nameof(Maquinas));
            }
            await LoadEstacionesSelectList(maquina.EstacionId);
            return View(maquina);
        }

        // GET: Planta/EditMaquina/5
        public async Task<IActionResult> EditMaquina(int? id)
        {
            if (id == null) return NotFound();

            var maquina = await _context.Maquinas.FindAsync(id);
            if (maquina == null) return NotFound();
            
            await LoadEstacionesSelectList(maquina.EstacionId);
            return View(maquina);
        }

        // POST: Planta/EditMaquina/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaquina(int id, [Bind("Id,Nombre,Descripcion,Codigo,NumeroSerie,Modelo,Fabricante,EstacionId,Activo")] Maquina maquina)
        {
            if (id != maquina.Id) return NotFound();

            // Remover validación de propiedades de navegación
            ModelState.Remove("Estacion");

            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(maquina.Codigo) && await _context.Maquinas.AnyAsync(m => m.Codigo == maquina.Codigo && m.Id != id))
                    {
                        ModelState.AddModelError("Codigo", "Este código ya existe.");
                        await LoadEstacionesSelectList(maquina.EstacionId);
                        return View(maquina);
                    }

                    if (!string.IsNullOrEmpty(maquina.NumeroSerie) && await _context.Maquinas.AnyAsync(m => m.NumeroSerie == maquina.NumeroSerie && m.Id != id))
                    {
                        ModelState.AddModelError("NumeroSerie", "Este número de serie ya existe.");
                        await LoadEstacionesSelectList(maquina.EstacionId);
                        return View(maquina);
                    }

                    var maquinaExistente = await _context.Maquinas.FindAsync(id);
                    if (maquinaExistente == null) return NotFound();

                    maquinaExistente.Nombre = maquina.Nombre;
                    maquinaExistente.Descripcion = maquina.Descripcion;
                    maquinaExistente.Codigo = maquina.Codigo;
                    maquinaExistente.NumeroSerie = maquina.NumeroSerie;
                    maquinaExistente.Modelo = maquina.Modelo;
                    maquinaExistente.Fabricante = maquina.Fabricante;
                    maquinaExistente.EstacionId = maquina.EstacionId;
                    maquinaExistente.Activo = maquina.Activo;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Máquina actualizada exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Maquinas.AnyAsync(e => e.Id == maquina.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Maquinas));
            }
            await LoadEstacionesSelectList(maquina.EstacionId);
            return View(maquina);
        }

        // POST: Planta/DeactivateMaquina/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateMaquina(int id)
        {
            var maquina = await _context.Maquinas.FindAsync(id);
            if (maquina != null)
            {
                maquina.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Máquina desactivada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la máquina.";
            }

            return RedirectToAction(nameof(Maquinas));
        }

        // POST: Planta/ActivateMaquina/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateMaquina(int id)
        {
            var maquina = await _context.Maquinas.FindAsync(id);
            if (maquina != null)
            {
                maquina.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Máquina activada exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar la máquina.";
            }

            return RedirectToAction(nameof(Maquinas));
        }

        // ========== MÉTODOS AUXILIARES ==========

        private async Task LoadLineasSelectList(int? selectedId = null)
        {
            var lineas = await _context.Lineas
                .Include(l => l.Area)
                .Where(l => l.Activo)
                .OrderBy(l => l.Area.Nombre)
                .ThenBy(l => l.Nombre)
                .Select(l => new
                {
                    l.Id,
                    Nombre = $"{l.Area.Nombre} - {l.Nombre}"
                })
                .ToListAsync();

            ViewBag.Lineas = new SelectList(lineas, "Id", "Nombre", selectedId);
        }

        private async Task LoadEstacionesSelectList(int? selectedId = null)
        {
            var estaciones = await _context.Estaciones
                .Include(e => e.Linea)
                    .ThenInclude(l => l.Area)
                .Where(e => e.Activo)
                .OrderBy(e => e.Linea.Area.Nombre)
                .ThenBy(e => e.Linea.Nombre)
                .ThenBy(e => e.Nombre)
                .Select(e => new
                {
                    e.Id,
                    Nombre = $"{e.Linea.Area.Nombre} - {e.Linea.Nombre} - {e.Nombre}"
                })
                .ToListAsync();

            ViewBag.Estaciones = new SelectList(estaciones, "Id", "Nombre", selectedId);
        }
    }
}
