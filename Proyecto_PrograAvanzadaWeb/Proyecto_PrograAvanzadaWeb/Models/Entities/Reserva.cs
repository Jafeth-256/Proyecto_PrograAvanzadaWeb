namespace Proyecto_PrograAvanzadaWeb.Models.Entities
{
    public class Reserva
    {
        public long IdReserva { get; set; }
        public long IdTour { get; set; }
        public long IdUsuario { get; set; }
        public int CantidadPersonas { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaReserva { get; set; }
        public string EstadoReserva { get; set; }
        public string? Comentarios { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string? NombreTour { get; set; }
        public string? Destino { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? NombreCreador { get; set; }
    }
}