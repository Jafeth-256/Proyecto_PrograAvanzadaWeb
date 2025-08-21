namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class ReservaViewModel
    {
        public long IdReserva { get; set; }
        public long IdTour { get; set; }
        public string NombreTour { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaReserva { get; set; }
        public string EstadoReserva { get; set; } = string.Empty;
        public string? Comentarios { get; set; }
        public string? NombreCreador { get; set; }

        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy");
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy");
        public string FechaReservaFormateada => FechaReserva.ToString("dd/MM/yyyy HH:mm");
        public string PrecioFormateado => $"₡{PrecioTotal:N0}";
        public int DuracionEnDias => (FechaFin - FechaInicio).Days + 1;
        public bool PuedeCancelar => EstadoReserva == "Pendiente" || EstadoReserva == "Confirmada";
        public string EstadoTexto => EstadoReserva switch
        {
            "Pendiente" => "Pendiente",
            "Confirmada" => "Confirmada",
            "Cancelada" => "Cancelada",
            _ => "Desconocido"
        };
    }
}