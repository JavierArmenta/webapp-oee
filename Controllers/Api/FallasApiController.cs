using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApp.Data;
using WebApp.Models.Linealytics;

namespace WebApp.Controllers.Api
{
    /// <summary>
    /// API Controller para registrar fallas desde dispositivos/PLCs
    /// Sistema independiente de paros
    /// </summary>
    [ApiController]
    [Route("api/fallas")]
    public class FallasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FallasApiController> _logger;

        public FallasApiController(ApplicationDbContext context, ILogger<FallasApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registrar una falla desde un dispositivo
        /// </summary>
        /// <param name="request">Datos de la falla (codigoMaquina, codigoFalla)</param>
        /// <returns>Confirmación del registro</returns>
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarFalla([FromBody] FallaApiRequest request)
        {
            try
            {
                // Validar request
                if (string.IsNullOrWhiteSpace(request.CodigoMaquina) || string.IsNullOrWhiteSpace(request.CodigoFalla))
                {
                    _logger.LogWarning("Intento de registro de falla con datos incompletos");
                    return BadRequest(new
                    {
                        error = "CodigoMaquina y CodigoFalla son requeridos"
                    });
                }

                // Buscar máquina por código
                var maquina = await _context.Maquinas
                    .Include(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                    .FirstOrDefaultAsync(m => m.Codigo == request.CodigoMaquina && m.Activo);

                if (maquina == null)
                {
                    _logger.LogWarning("Máquina no encontrada: {CodigoMaquina}", request.CodigoMaquina);
                    return NotFound(new
                    {
                        error = "Máquina no encontrada o inactiva",
                        codigoMaquina = request.CodigoMaquina
                    });
                }

                // Buscar catálogo de falla por código
                var catalogoFalla = await _context.CatalogoFallas
                    .FirstOrDefaultAsync(c => c.Codigo == request.CodigoFalla && c.Activo);

                if (catalogoFalla == null)
                {
                    _logger.LogWarning("Código de falla no encontrado: {CodigoFalla}", request.CodigoFalla);
                    return NotFound(new
                    {
                        error = "Código de falla no encontrado o inactivo",
                        codigoFalla = request.CodigoFalla
                    });
                }

                // Crear registro de falla
                var registro = new RegistroFalla
                {
                    CatalogoFallaId = catalogoFalla.Id,
                    MaquinaId = maquina.Id,
                    FechaHoraDeteccion = DateTime.UtcNow,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.UtcNow
                };

                _context.RegistrosFallas.Add(registro);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Falla registrada: ID={RegistroId}, Máquina={Maquina}, Falla={Falla}",
                    registro.Id,
                    maquina.Nombre,
                    catalogoFalla.Nombre
                );

                return Ok(new
                {
                    mensaje = "Falla registrada exitosamente",
                    registroId = registro.Id,
                    maquina = maquina.Nombre,
                    linea = maquina.Estacion?.Linea?.Nombre,
                    falla = catalogoFalla.Nombre,
                    severidad = catalogoFalla.Severidad,
                    categoria = catalogoFalla.Categoria,
                    fechaHora = registro.FechaHoraDeteccion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar falla");
                return StatusCode(500, new
                {
                    error = "Error interno al registrar la falla",
                    detalle = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtener listado de códigos de fallas disponibles
        /// </summary>
        [HttpGet("codigos")]
        public async Task<IActionResult> ObtenerCodigosFallas()
        {
            try
            {
                var catalogos = await _context.CatalogoFallas
                    .Where(c => c.Activo)
                    .OrderBy(c => c.Codigo)
                    .Select(c => new
                    {
                        c.Codigo,
                        c.Nombre,
                        c.Severidad,
                        c.Categoria
                    })
                    .ToListAsync();

                return Ok(catalogos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener códigos de fallas");
                return StatusCode(500, new { error = "Error al obtener códigos de fallas" });
            }
        }

        /// <summary>
        /// Obtener listado de códigos de máquinas disponibles
        /// </summary>
        [HttpGet("maquinas")]
        public async Task<IActionResult> ObtenerCodigosMaquinas()
        {
            try
            {
                var maquinas = await _context.Maquinas
                    .Include(m => m.Estacion)
                        .ThenInclude(e => e.Linea)
                    .Where(m => m.Activo)
                    .OrderBy(m => m.Codigo)
                    .Select(m => new
                    {
                        m.Codigo,
                        m.Nombre,
                        Linea = m.Estacion!.Linea!.Nombre,
                        Estacion = m.Estacion.Nombre
                    })
                    .ToListAsync();

                return Ok(maquinas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener códigos de máquinas");
                return StatusCode(500, new { error = "Error al obtener códigos de máquinas" });
            }
        }
    }

    /// <summary>
    /// DTO para el request de registro de falla desde API
    /// </summary>
    public class FallaApiRequest
    {
        [Required(ErrorMessage = "El código de máquina es requerido")]
        public string CodigoMaquina { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código de falla es requerido")]
        public string CodigoFalla { get; set; } = string.Empty;
    }
}
