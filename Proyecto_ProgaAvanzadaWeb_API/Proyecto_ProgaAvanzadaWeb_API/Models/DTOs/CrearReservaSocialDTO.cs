namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class CrearReservaSocialDTO
    {
        public long IdEventoSocial { get; set; }
        public int CantidadPersonas { get; set; }
        public string? Comentarios { get; set; }
    }
}
