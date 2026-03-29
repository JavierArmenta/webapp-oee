namespace WebApp.Models.Linealytics
{
    public class OeeResultDto
    {
        public int MaquinaId { get; set; }
        public string MaquinaNombre { get; set; } = string.Empty;

        // Contexto temporal
        public int? TurnoId { get; set; }
        public string? TurnoNombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        // Tiempos (minutos)
        public double TiempoDisponibleMinutos { get; set; }
        public double TiempoParoMinutos { get; set; }
        public double TiempoProduccionMinutos { get; set; }

        // Conteos de producción
        public long ProduccionOK { get; set; }
        public long ProduccionNOK { get; set; }
        public long ProduccionTotal { get; set; }

        // Parámetro de rendimiento
        public double TiempoCicloPromedioSegundos { get; set; }

        // OEE y componentes (0–100)
        public decimal DisponibilidadPorcentaje { get; set; }
        public decimal RendimientoPorcentaje { get; set; }
        public decimal CalidadPorcentaje { get; set; }
        public decimal OeePorcentaje { get; set; }

        // Metadata del resultado
        public DateTime FechaCalculo { get; set; } = DateTime.UtcNow;
        public int? MetricasMaquinaId { get; set; }
        public bool Guardado { get; set; }

        // Indicadores de calidad del dato
        public bool SinDatosProduccion { get; set; }
        public bool SinTiempoCiclo { get; set; }
    }
}
