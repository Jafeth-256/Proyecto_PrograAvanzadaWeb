using System.ComponentModel.DataAnnotations;

namespace Proyecto_ProgaAvanzadaWeb_API.Models.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
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