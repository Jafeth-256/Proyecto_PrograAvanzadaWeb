using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Evento
{
    public int Id { get; set; }
    [Required]
    public string Titulo { get; set; }
    [Required]
    public string Tipo { get; set; }
    [Required]
    public string Ubicacion { get; set; }
    [Required]
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; }
    public string ImagenRuta { get; set; }
    public string Contacto { get; set; }
    public string Organizador { get; set; }
    public string UsuarioId { get; set; } // FK a IdentityUser
    [ForeignKey("UsuarioId")]
    public IdentityUser Usuario { get; set; }
}