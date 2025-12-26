using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize(Roles = "SuperAdmin,Administrator")]
    public class OperadoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOperadorService _operadorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OperadoresController(ApplicationDbContext context, IOperadorService operadorService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _operadorService = operadorService;
            _userManager = userManager;
        }

        // GET: Operadores
        public async Task<IActionResult> Index()
        {
            var operadores = await _context.Operadores
                .Include(o => o.OperadorRoles)
                    .ThenInclude(or => or.RolOperador)
                .OrderBy(o => o.NumeroEmpleado)
                .ToListAsync();
            return View(operadores);
        }

        // GET: Operadores/Lista
        public async Task<IActionResult> Lista()
        {
            var operadores = await _context.Operadores
                .Include(o => o.OperadorRoles)
                    .ThenInclude(or => or.RolOperador)
                .OrderBy(o => o.Id)
                .ToListAsync();
            return View(operadores);
        }

        // GET: Operadores/Usuarios
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();
            return View(usuarios);
        }

        // GET: Operadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operador = await _context.Operadores
                .Include(o => o.OperadorRoles)
                    .ThenInclude(or => or.RolOperador)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (operador == null)
            {
                return NotFound();
            }

            return View(operador);
        }

        // GET: Operadores/Roles/5
        public async Task<IActionResult> Roles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operador = await _context.Operadores
                .Include(o => o.OperadorRoles)
                    .ThenInclude(or => or.RolOperador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (operador == null)
            {
                return NotFound();
            }

            return View(operador);
        }

        // GET: Operadores/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.RolesOperador = await _context.RolesOperador
                .Where(r => r.Activo)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View();
        }

        // POST: Operadores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,NumeroEmpleado")] Operador operador, string CodigoPin, List<int>? RolesSeleccionados)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Operadores.AnyAsync(o => o.NumeroEmpleado == operador.NumeroEmpleado))
                {
                    ModelState.AddModelError("NumeroEmpleado", "Este número de empleado ya existe.");
                    ViewBag.RolesOperador = await _context.RolesOperador.Where(r => r.Activo).ToListAsync();
                    return View(operador);
                }

                if (string.IsNullOrWhiteSpace(CodigoPin))
                {
                    ModelState.AddModelError("CodigoPin", "El código PIN es requerido.");
                    ViewBag.RolesOperador = await _context.RolesOperador.Where(r => r.Activo).ToListAsync();
                    return View(operador);
                }

                if (CodigoPin.Length < 4)
                {
                    ModelState.AddModelError("CodigoPin", "El código PIN debe tener al menos 4 dígitos.");
                    ViewBag.RolesOperador = await _context.RolesOperador.Where(r => r.Activo).ToListAsync();
                    return View(operador);
                }

                operador.CodigoPinHashed = _operadorService.HashPin(CodigoPin);
                operador.FechaCreacion = DateTime.UtcNow;
                operador.Activo = true;

                _context.Add(operador);
                await _context.SaveChangesAsync();

                if (RolesSeleccionados != null && RolesSeleccionados.Any())
                {
                    foreach (var rolId in RolesSeleccionados)
                    {
                        _context.OperadorRolesOperador.Add(new OperadorRolOperador
                        {
                            OperadorId = operador.Id,
                            RolOperadorId = rolId,
                            FechaAsignacion = DateTime.UtcNow
                        });
                    }
                    await _context.SaveChangesAsync();
                }
                
                TempData["Success"] = "Operador creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.RolesOperador = await _context.RolesOperador.Where(r => r.Activo).ToListAsync();
            return View(operador);
        }

        // GET: Operadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operador = await _context.Operadores
                .Include(o => o.OperadorRoles)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (operador == null)
            {
                return NotFound();
            }

            ViewBag.RolesOperador = await _context.RolesOperador
                .Where(r => r.Activo)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            
            ViewBag.RolesSeleccionados = operador.OperadorRoles
                .Select(or => or.RolOperadorId)
                .ToList();

            return View(operador);
        }

        // POST: Operadores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,NumeroEmpleado,Activo")] Operador operador, string? CodigoPin, List<int>? RolesSeleccionados)
        {
            if (id != operador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var operadorExistente = await _context.Operadores
                        .Include(o => o.OperadorRoles)
                        .FirstOrDefaultAsync(o => o.Id == id);

                    if (operadorExistente == null)
                    {
                        return NotFound();
                    }

                    if (await _context.Operadores.AnyAsync(o => o.NumeroEmpleado == operador.NumeroEmpleado && o.Id != id))
                    {
                        ModelState.AddModelError("NumeroEmpleado", "Este número de empleado ya existe.");
                        ViewBag.RolesOperador = await _context.RolesOperador.Where(r => r.Activo).ToListAsync();
                        ViewBag.RolesSeleccionados = RolesSeleccionados ?? new List<int>();
                        return View(operador);
                    }

                    operadorExistente.Nombre = operador.Nombre;
                    operadorExistente.Apellido = operador.Apellido;
                    operadorExistente.NumeroEmpleado = operador.NumeroEmpleado;
                    operadorExistente.Activo = operador.Activo;

                    if (!string.IsNullOrWhiteSpace(CodigoPin))
                    {
                        if (CodigoPin.Length < 4)
                        {
                            ModelState.AddModelError("CodigoPin", "El código PIN debe tener al menos 4 dígitos.");
                            ViewBag.RolesOperador = await _context.RolesOperador.Where(r => r.Activo).ToListAsync();
                            ViewBag.RolesSeleccionados = RolesSeleccionados ?? new List<int>();
                            return View(operador);
                        }
                        operadorExistente.CodigoPinHashed = _operadorService.HashPin(CodigoPin);
                    }

                    _context.OperadorRolesOperador.RemoveRange(operadorExistente.OperadorRoles);

                    if (RolesSeleccionados != null && RolesSeleccionados.Any())
                    {
                        foreach (var rolId in RolesSeleccionados)
                        {
                            _context.OperadorRolesOperador.Add(new OperadorRolOperador
                            {
                                OperadorId = operadorExistente.Id,
                                RolOperadorId = rolId,
                                FechaAsignacion = DateTime.UtcNow
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Operador actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OperadorExists(operador.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            ViewBag.RolesOperador = await _context.RolesOperador.Where(r => r.Activo).ToListAsync();
            ViewBag.RolesSeleccionados = RolesSeleccionados ?? new List<int>();
            return View(operador);
        }

        // GET: Operadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var operador = await _context.Operadores
                .Include(o => o.OperadorRoles)
                    .ThenInclude(or => or.RolOperador)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (operador == null)
            {
                return NotFound();
            }

            return View(operador);
        }

        // POST: Operadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var operador = await _context.Operadores.FindAsync(id);
            if (operador != null)
            {
                _context.Operadores.Remove(operador);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Operador eliminado exitosamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool OperadorExists(int id)
        {
            return _context.Operadores.Any(e => e.Id == id);
        }
    }
}
