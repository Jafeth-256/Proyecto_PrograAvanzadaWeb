namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class CrearReservaSocialViewModel
    {
        public long IdEventoSocial { get; set; }
        public int CantidadPersonas { get; set; } = 1;
        public string? Comentarios { get; set; }
        public EventoSocialDisponibleViewModel? Evento { get; set; }
    }
}
