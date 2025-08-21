namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class EventoSocialDisponibleDTO
    {
        public long IdEventoSocial { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public decimal Precio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadPersonas { get; set; }
        public string NombreCreador { get; set; }
        public int PersonasReservadas { get; set; }
        public int CuposDisponibles { get; set; }
    }
}
