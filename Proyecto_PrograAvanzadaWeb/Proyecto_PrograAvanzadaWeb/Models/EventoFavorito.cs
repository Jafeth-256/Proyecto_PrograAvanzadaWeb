namespace Proyecto_PrograAvanzadaWeb.Models
{
    public class EventoFavorito
    {
        public long IdFavorito { get; set; }
        public long IdUsuario { get; set; }
        public int IdEvento { get; set; }
        public DateTime FechaAgregado { get; set; }
        public bool MeInteresa { get; set; }

        // Para vistas
        public string TituloEvento { get; set; }
        public string TipoEvento { get; set; }
        public DateTime FechaEvento { get; set; }
        public string UbicacionEvento { get; set; }
    }
}
