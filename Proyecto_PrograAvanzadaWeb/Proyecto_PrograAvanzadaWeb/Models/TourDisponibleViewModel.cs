namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class TourDisponibleViewModel
    {
        public long IdTour { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public string NombreCreador { get; set; } = string.Empty;
        public int PersonasReservadas { get; set; }
        public int CuposDisponibles { get; set; }

        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy");
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy");
        public int DuracionEnDias => (FechaFin - FechaInicio).Days + 1;
        public string PrecioFormateado => $"₡{Precio:N0}";
        public double PorcentajeOcupacion => CantidadPersonas > 0 ? (double)PersonasReservadas / CantidadPersonas * 100 : 0;
        public bool TieneCuposDisponibles => CuposDisponibles > 0;
    }
}