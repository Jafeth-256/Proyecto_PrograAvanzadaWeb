namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class Reserva
    {
        public long IdReserva { get; set; }
        public long IdTour { get; set; }
        public long IdUsuario { get; set; }
        public DateTime FechaReserva { get; set; }
        public DateTime FechaTour { get; set; }
        public int CantidadPersonas { get; set; }
        public decimal PrecioTotal { get; set; }
        public string Estado { get; set; } // Pendiente, Confirmada, Cancelada
        public string NumeroReserva { get; set; }

        // Para vistas
        public string NombreTour { get; set; }
        public string NombreUsuario { get; set; }
        public string CorreoUsuario { get; set; }
    }

    public class CrearReservaViewModel
    {
        public long IdTour { get; set; }
        public DateTime FechaTour { get; set; }
        public int CantidadPersonas { get; set; }
        public string NombreTour { get; set; }
        public decimal PrecioPorPersona { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}
