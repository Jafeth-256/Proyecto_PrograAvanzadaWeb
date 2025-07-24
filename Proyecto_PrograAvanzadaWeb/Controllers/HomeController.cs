using Proyecto_PrograAvanzadaWeb.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Proyecto_PrograAvanzadaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Index - Login
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor ingrese correo y contraseña";
                return View();
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                string passwordEncriptada = EncriptarContrasena(password);

                var usuario = context.QueryFirstOrDefault<Usuario>(
                    "ValidarInicioSesion",
                    new { Correo = email, Contrasenna = passwordEncriptada },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (usuario != null)
                {
                    HttpContext.Session.SetString("IdUsuario", usuario.IdUsuario.ToString());
                    HttpContext.Session.SetString("Nombre", usuario.Nombre);
                    HttpContext.Session.SetString("Correo", usuario.Correo);
                    HttpContext.Session.SetString("IdRol", usuario.IdRol.ToString());
                    HttpContext.Session.SetString("NombreRol", usuario.NombreRol);

                    return RedirectToAction("Inicio");
                }
                else
                {
                    ViewBag.Error = "Correo o contraseña incorrectos";
                    return View();
                }
            }
        }
        #endregion

        #region Inicio - Página Principal
        public IActionResult Inicio()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IdUsuario")))
            {
                return RedirectToAction("Index");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("Nombre");
            return View();
        }
        #endregion

        #region Registro - Crear Usuario
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registro(string nombre, string correo, string identificacion, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo) ||
                string.IsNullOrEmpty(identificacion) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Todos los campos son obligatorios";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            if (password.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres";
                return View();
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                string passwordEncriptada = EncriptarContrasena(password);

                var resultado = context.QueryFirstOrDefault<dynamic>(
                    "RegistrarUsuario",
                    new
                    {
                        Nombre = nombre,
                        Correo = correo,
                        Identificacion = identificacion,
                        Contrasenna = passwordEncriptada
                    },
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (resultado.Resultado > 0)
                {
                    ViewBag.Exito = "Usuario registrado exitosamente. Ya puedes iniciar sesión.";
                    return View();
                }
                else
                {
                    ViewBag.Error = resultado.Mensaje;
                    return View();
                }
            }
        }
        #endregion

        #region Logout - Cerrar Sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        #endregion

        #region Métodos Privados
        private string EncriptarContrasena(string contrasena)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(contrasena);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        #endregion
    }
}