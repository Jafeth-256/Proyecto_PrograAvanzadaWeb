namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class CrearReservaViewModel
    {
        public long IdTour { get; set; }
        public int CantidadPersonas { get; set; } = 1;
        public string? Comentarios { get; set; }
        public TourDisponibleViewModel? Tour { get; set; }
    }
}