namespace Proyecto_ProgaAvanzadaWeb_API.Models.Entities
{
    public class ReservaSocial
    {
        public long IdReservaSocial { get; set; }
        public long IdEventoSocial { get; set; }
        public string NombreEvento { get; set; }
        public string Ubicacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaReserva { get; set; }
        public string EstadoReserva { get; set; }
        public string Comentarios { get; set; }
        public string NombreCreador { get; set; }
    }
}
