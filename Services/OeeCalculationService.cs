using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models.Linealytics;

namespace WebApp.Services
{
    public class OeeCalculationService
    {
        private readonly ApplicationDbContext _context;

        public OeeCalculationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── Overload 1: por turno + fecha ──────────────────────────────────────

        public async Task<OeeResultDto> CalcularPorTurnoAsync(
            int maquinaId, int turnoId, DateTime fecha, bool guardar)
        {
            var turno = await _context.Turnos.FindAsync(turnoId)
                ?? throw new ArgumentException($"Turno {turnoId} no encontrado.");

            var maquina = await _context.Maquinas.FindAsync(maquinaId)
                ?? throw new ArgumentException($"Máquina {maquinaId} no encontrada.");

            // Construir ventana temporal
            var fechaBase = fecha.Date;
            var ventanaInicio = fechaBase + turno.HoraInicio;

            DateTime ventanaFin;
            if (turno.HoraFin < turno.HoraInicio)
                // Turno nocturno: cruza medianoche
                ventanaFin = fechaBase.AddDays(1) + turno.HoraFin;
            else
                ventanaFin = fechaBase + turno.HoraFin;

            var resultado = await CalcularInternoAsync(
                maquinaId, maquina.Nombre,
                ventanaInicio, ventanaFin,
                guardar, turnoId, turno.Nombre);

            return resultado;
        }

        // ─── Overload 2: por rango libre ─────────────────────────────────────────

        public async Task<OeeResultDto> CalcularPorRangoAsync(
            int maquinaId, DateTime fechaInicio, DateTime fechaFin, bool guardar)
        {
            var maquina = await _context.Maquinas.FindAsync(maquinaId)
                ?? throw new ArgumentException($"Máquina {maquinaId} no encontrada.");

            return await CalcularInternoAsync(
                maquinaId, maquina.Nombre,
                fechaInicio, fechaFin,
                guardar, turnoId: null, turnoNombre: null);
        }

        // ─── Lógica de cálculo central ────────────────────────────────────────────

        private async Task<OeeResultDto> CalcularInternoAsync(
            int maquinaId, string maquinaNombre,
            DateTime ventanaInicio, DateTime ventanaFin,
            bool guardar, int? turnoId, string? turnoNombre)
        {
            var ahora = DateTime.UtcNow;
            var finEfectivo = ventanaFin > ahora ? ahora : ventanaFin;

            // ── 1. TiempoDisponible ──────────────────────────────────────────────
            var tiempoDisponibleMinutos = (finEfectivo - ventanaInicio).TotalMinutes;
            if (tiempoDisponibleMinutos <= 0)
                tiempoDisponibleMinutos = 0;

            // ── 2. TiempoParo ────────────────────────────────────────────────────
            // Paros que se solapan con la ventana
            var paros = await _context.RegistrosParoBotonera
                .Where(p =>
                    p.MaquinaId == maquinaId &&
                    p.FechaHoraInicio < finEfectivo &&
                    (p.FechaHoraFin == null || p.FechaHoraFin > ventanaInicio))
                .ToListAsync();

            double tiempoParoMinutos = 0;
            foreach (var paro in paros)
            {
                var inicioEfectivo = paro.FechaHoraInicio < ventanaInicio
                    ? ventanaInicio
                    : paro.FechaHoraInicio;

                var finParoEfectivo = paro.FechaHoraFin.HasValue
                    ? paro.FechaHoraFin.Value
                    : finEfectivo; // paro abierto: usar finEfectivo

                if (finParoEfectivo > finEfectivo)
                    finParoEfectivo = finEfectivo;

                var duracion = (finParoEfectivo - inicioEfectivo).TotalMinutes;
                if (duracion > 0)
                    tiempoParoMinutos += duracion;
            }

            var tiempoProduccionMinutos = tiempoDisponibleMinutos - tiempoParoMinutos;
            if (tiempoProduccionMinutos < 0)
                tiempoProduccionMinutos = 0;

            // ── 3. Producción (corridas activas en la ventana) ───────────────────
            var corridas = await _context.CorridasProduccion
                .Include(c => c.Producto)
                .Where(c =>
                    c.MaquinaId == maquinaId &&
                    c.FechaInicio < finEfectivo &&
                    (c.FechaFin == null || c.FechaFin > ventanaInicio))
                .ToListAsync();

            long produccionOK = 0;
            long produccionNOK = 0;
            double sumaPonderadaCiclo = 0;
            long totalPonderado = 0;
            bool sinTiempoCiclo = false;

            foreach (var corrida in corridas)
            {
                produccionOK += corrida.ProduccionOK;
                produccionNOK += corrida.ProduccionNOK;

                var totalCorrida = corrida.ProduccionOK + corrida.ProduccionNOK;
                if (corrida.Producto != null && corrida.Producto.TiempoCicloSegundos > 0)
                {
                    sumaPonderadaCiclo += totalCorrida * corrida.Producto.TiempoCicloSegundos;
                    totalPonderado += totalCorrida;
                }
                else
                {
                    sinTiempoCiclo = true;
                }
            }

            var produccionTotal = produccionOK + produccionNOK;
            bool sinDatosProduccion = produccionTotal == 0 && !corridas.Any();

            // TiempoCiclo promedio ponderado
            double tiempoCicloPromedio = 0;
            if (totalPonderado > 0)
                tiempoCicloPromedio = sumaPonderadaCiclo / totalPonderado;
            else if (corridas.Any() && corridas.First().Producto?.TiempoCicloSegundos > 0)
                tiempoCicloPromedio = corridas.First().Producto!.TiempoCicloSegundos;

            // ── 4. Componentes OEE ───────────────────────────────────────────────
            decimal disponibilidad = tiempoDisponibleMinutos > 0
                ? Math.Round((decimal)(tiempoProduccionMinutos / tiempoDisponibleMinutos) * 100, 2)
                : 0m;

            decimal rendimiento = 0m;
            if (tiempoProduccionMinutos > 0 && tiempoCicloPromedio > 0)
            {
                var produccionTeorica = tiempoProduccionMinutos * 60.0 / tiempoCicloPromedio;
                rendimiento = produccionTeorica > 0
                    ? Math.Round((decimal)(produccionTotal / produccionTeorica) * 100, 2)
                    : 0m;
            }

            decimal calidad = produccionTotal > 0
                ? Math.Round((decimal)produccionOK / produccionTotal * 100, 2)
                : (sinDatosProduccion ? 0m : 100m);

            // Limitar componentes a 100%
            disponibilidad = Math.Min(disponibilidad, 100m);
            rendimiento = Math.Min(rendimiento, 100m);
            calidad = Math.Min(calidad, 100m);

            decimal oee = Math.Round(disponibilidad / 100m * rendimiento / 100m * calidad / 100m * 100m, 2);

            // ── 5. Guardar en MetricasMaquina (upsert) ───────────────────────────
            int? metricasId = null;
            if (guardar)
            {
                // Buscar registro existente para este turno/rango
                MetricasMaquina? existente = null;
                if (turnoId.HasValue)
                {
                    existente = await _context.MetricasMaquina
                        .FirstOrDefaultAsync(m =>
                            m.MaquinaId == maquinaId &&
                            m.TurnoId == turnoId.Value &&
                            m.FechaInicio == ventanaInicio);
                }
                else
                {
                    existente = await _context.MetricasMaquina
                        .FirstOrDefaultAsync(m =>
                            m.MaquinaId == maquinaId &&
                            m.TurnoId == 0 &&
                            m.FechaInicio == ventanaInicio);
                }

                if (existente != null)
                {
                    // Actualizar
                    existente.FechaFin = ventanaFin;
                    existente.TiempoDisponibleMinutos = (int)Math.Round(tiempoDisponibleMinutos);
                    existente.TiempoProduccionMinutos = (int)Math.Round(tiempoProduccionMinutos);
                    existente.TiempoParoMinutos = (int)Math.Round(tiempoParoMinutos);
                    existente.UnidadesProducidas = (int)produccionTotal;
                    existente.UnidadesDefectuosas = (int)produccionNOK;
                    existente.UnidadesBuenas = (int)produccionOK;
                    existente.DisponibilidadPorcentaje = disponibilidad;
                    existente.RendimientoPorcentaje = rendimiento;
                    existente.CalidadPorcentaje = calidad;
                    existente.OeePorcentaje = oee;
                    existente.Cerrada = ventanaFin <= ahora;
                    metricasId = existente.Id;
                }
                else
                {
                    // Crear nuevo
                    var nuevaMetrica = new MetricasMaquina
                    {
                        MaquinaId = maquinaId,
                        TurnoId = turnoId ?? 0,
                        ProductoId = corridas.FirstOrDefault()?.ProductoId,
                        FechaInicio = ventanaInicio,
                        FechaFin = ventanaFin,
                        TiempoDisponibleMinutos = (int)Math.Round(tiempoDisponibleMinutos),
                        TiempoProduccionMinutos = (int)Math.Round(tiempoProduccionMinutos),
                        TiempoParoMinutos = (int)Math.Round(tiempoParoMinutos),
                        UnidadesProducidas = (int)produccionTotal,
                        UnidadesDefectuosas = (int)produccionNOK,
                        UnidadesBuenas = (int)produccionOK,
                        DisponibilidadPorcentaje = disponibilidad,
                        RendimientoPorcentaje = rendimiento,
                        CalidadPorcentaje = calidad,
                        OeePorcentaje = oee,
                        Cerrada = ventanaFin <= ahora,
                        FechaCreacion = DateTime.UtcNow
                    };
                    _context.MetricasMaquina.Add(nuevaMetrica);
                    await _context.SaveChangesAsync();
                    metricasId = nuevaMetrica.Id;
                }

                await _context.SaveChangesAsync();
            }

            return new OeeResultDto
            {
                MaquinaId = maquinaId,
                MaquinaNombre = maquinaNombre,
                TurnoId = turnoId,
                TurnoNombre = turnoNombre,
                FechaInicio = ventanaInicio,
                FechaFin = ventanaFin,
                TiempoDisponibleMinutos = Math.Round(tiempoDisponibleMinutos, 2),
                TiempoParoMinutos = Math.Round(tiempoParoMinutos, 2),
                TiempoProduccionMinutos = Math.Round(tiempoProduccionMinutos, 2),
                ProduccionOK = produccionOK,
                ProduccionNOK = produccionNOK,
                ProduccionTotal = produccionTotal,
                TiempoCicloPromedioSegundos = Math.Round(tiempoCicloPromedio, 2),
                DisponibilidadPorcentaje = disponibilidad,
                RendimientoPorcentaje = rendimiento,
                CalidadPorcentaje = calidad,
                OeePorcentaje = oee,
                FechaCalculo = DateTime.UtcNow,
                MetricasMaquinaId = metricasId,
                Guardado = guardar,
                SinDatosProduccion = sinDatosProduccion,
                SinTiempoCiclo = sinTiempoCiclo
            };
        }
    }
}
