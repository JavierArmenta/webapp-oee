using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public class RolesOperadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RolesOperadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RolesOperador
        public async Task<IActionResult> Index()
        {
            var roles = await _context.RolesOperador
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View(roles);
        }

        // GET: RolesOperador/Lista
        public async Task<IActionResult> Lista()
        {
            var roles = await _context.RolesOperador
                .Include(r => r.OperadorRoles)
                    .ThenInclude(or => or.Operador)
                .OrderBy(r => r.Id)
                .ToListAsync();
            return View(roles);
        }

        // GET: RolesOperador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.RolesOperador
                .Include(r => r.OperadorRoles)
                    .ThenInclude(or => or.Operador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }

        // GET: RolesOperador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RolesOperador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Activo")] RolOperador rol)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el nombre ya existe
                if (await _context.RolesOperador.AnyAsync(r => r.Nombre == rol.Nombre))
                {
                    ModelState.AddModelError("Nombre", "Este nombre de rol ya existe.");
                    return View(rol);
                }

                rol.FechaCreacion = DateTime.UtcNow;
                _context.Add(rol);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Rol creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(rol);
        }

        // GET: RolesOperador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.RolesOperador.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            return View(rol);
        }

        // POST: RolesOperador/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Activo")] RolOperador rol)
        {
            if (id != rol.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el nombre ya existe en otro registro
                    if (await _context.RolesOperador.AnyAsync(r => r.Nombre == rol.Nombre && r.Id != id))
                    {
                        ModelState.AddModelError("Nombre", "Este nombre de rol ya existe.");
                        return View(rol);
                    }

                    var rolExistente = await _context.RolesOperador.FindAsync(id);
                    if (rolExistente == null)
                    {
                        return NotFound();
                    }

                    rolExistente.Nombre = rol.Nombre;
                    rolExistente.Descripcion = rol.Descripcion;
                    rolExistente.Activo = rol.Activo;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Rol actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RolExists(rol.Id))
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
            return View(rol);
        }

        // GET: RolesOperador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.RolesOperador
                .Include(r => r.OperadorRoles)
                    .ThenInclude(or => or.Operador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }

        // POST: RolesOperador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = await _context.RolesOperador
                .Include(r => r.OperadorRoles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rol != null)
            {
                if (rol.OperadorRoles.Any())
                {
                    TempData["Error"] = "No se puede eliminar el rol porque tiene operadores asignados.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.RolesOperador.Remove(rol);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Rol eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RolExists(int id)
        {
            return _context.RolesOperador.Any(e => e.Id == id);
        }
    }
}
