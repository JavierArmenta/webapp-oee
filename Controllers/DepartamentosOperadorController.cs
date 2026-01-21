using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public class DepartamentosOperadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartamentosOperadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DepartamentosOperador
        public async Task<IActionResult> Index()
        {
            var departamentos = await _context.DepartamentosOperador
                .OrderBy(d => d.Nombre)
                .ToListAsync();
            return View(departamentos);
        }

        // GET: DepartamentosOperador/Lista
        public async Task<IActionResult> Lista()
        {
            var departamentos = await _context.DepartamentosOperador
                .Include(d => d.OperadorDepartamentos)
                    .ThenInclude(od => od.Operador)
                .OrderBy(d => d.Id)
                .ToListAsync();
            return View(departamentos);
        }

        // GET: DepartamentosOperador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.DepartamentosOperador
                .Include(d => d.OperadorDepartamentos)
                    .ThenInclude(od => od.Operador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // GET: DepartamentosOperador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DepartamentosOperador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Activo")] DepartamentoOperador departamento)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el nombre ya existe
                if (await _context.DepartamentosOperador.AnyAsync(d => d.Nombre == departamento.Nombre))
                {
                    ModelState.AddModelError("Nombre", "Este nombre de departamento ya existe.");
                    return View(departamento);
                }

                departamento.FechaCreacion = DateTime.UtcNow;
                _context.Add(departamento);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Departamento creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }

        // GET: DepartamentosOperador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.DepartamentosOperador.FindAsync(id);
            if (departamento == null)
            {
                return NotFound();
            }
            return View(departamento);
        }

        // POST: DepartamentosOperador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Color,Activo")] DepartamentoOperador departamento)
        {
            if (id != departamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el nombre ya existe en otro registro
                    if (await _context.DepartamentosOperador.AnyAsync(d => d.Nombre == departamento.Nombre && d.Id != id))
                    {
                        ModelState.AddModelError("Nombre", "Este nombre de departamento ya existe.");
                        return View(departamento);
                    }

                    var departamentoExistente = await _context.DepartamentosOperador.FindAsync(id);
                    if (departamentoExistente == null)
                    {
                        return NotFound();
                    }

                    departamentoExistente.Nombre = departamento.Nombre;
                    departamentoExistente.Descripcion = departamento.Descripcion;
                    departamentoExistente.Color = departamento.Color;
                    departamentoExistente.Activo = departamento.Activo;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Departamento actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartamentoExists(departamento.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }

        // POST: DepartamentosOperador/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var departamento = await _context.DepartamentosOperador.FindAsync(id);
            if (departamento != null)
            {
                departamento.Activo = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Departamento desactivado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el departamento.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: DepartamentosOperador/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var departamento = await _context.DepartamentosOperador.FindAsync(id);
            if (departamento != null)
            {
                departamento.Activo = true;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Departamento activado exitosamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo encontrar el departamento.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DepartamentoExists(int id)
        {
            return _context.DepartamentosOperador.Any(e => e.Id == id);
        }
    }
}
