namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class LoginDTO
    {
        public string Correo { get; set; }
        public string Contrasenna { get; set; }
    }

    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public UsuarioDTO? Usuario { get; set; }
    }
}
