namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class EventoUniversitarioViewModel
    {
        public long IdEventoUniversitario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public string Universidad { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public string? NombreCreador { get; set; }

        // Propiedades calculadas
        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy HH:mm");
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy HH:mm");
        public string PrecioFormateado => $"₡{Precio:N0}";
        public int DuracionEnHoras => (int)(FechaFin - FechaInicio).TotalHours;
        public bool EsEventoFuturo => FechaInicio > DateTime.Now;
        public bool EsEventoEnCurso => DateTime.Now >= FechaInicio && DateTime.Now <= FechaFin;
        public bool EventoTerminado => DateTime.Now > FechaFin;
        public string EstadoEvento => EventoTerminado ? "Finalizado" : EsEventoEnCurso ? "En Curso" : "Próximo";
    }
}