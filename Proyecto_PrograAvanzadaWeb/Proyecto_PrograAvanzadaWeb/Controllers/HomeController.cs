using Proyecto_PrograAvanzadaWeb.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;

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

        // Nuevos Metodos

        #region Recuperar Contraseña
        [HttpGet]
        public IActionResult RecuperarContrasena()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecuperarContrasena(string correo)
        {
            if (string.IsNullOrEmpty(correo))
            {
                ViewBag.Error = "Por favor ingrese su correo electrónico";
                return View();
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                // Verificar si el correo existe
                var usuario = context.QueryFirstOrDefault<Usuario>(
                    "SELECT * FROM TUsuario WHERE Correo = @Correo AND Estado = 1",
                    new { Correo = correo }
                );

                if (usuario == null)
                {
                    ViewBag.Error = "No existe una cuenta asociada a ese correo electrónico";
                    return View();
                }

                // Generar token de recuperación
                string token = GenerarTokenRecuperacion();
                DateTime expiracion = DateTime.Now.AddHours(2);

                // Guardar token en la base de datos
                var resultado = context.Execute(
                    @"INSERT INTO TTokenRecuperacion (IdUsuario, Token, FechaExpiracion, Usado) 
              VALUES (@IdUsuario, @Token, @FechaExpiracion, 0)",
                    new
                    {
                        IdUsuario = usuario.IdUsuario,
                        Token = token,
                        FechaExpiracion = expiracion
                    }
                );

                if (resultado > 0)
                {
                    // Enviar correo con el link de recuperación
                    EnviarCorreoRecuperacion(correo, usuario.Nombre, token);
                    ViewBag.Exito = "Se ha enviado un correo con las instrucciones para recuperar tu contraseña";
                    return View();
                }
                else
                {
                    ViewBag.Error = "Error al procesar la solicitud. Por favor intente nuevamente";
                    return View();
                }
            }
        }

        [HttpGet]
        public IActionResult RestablecerContrasena(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index");
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                // Verificar token válido
                var tokenInfo = context.QueryFirstOrDefault<dynamic>(
                    @"SELECT t.*, u.Correo 
              FROM TTokenRecuperacion t
              INNER JOIN TUsuario u ON t.IdUsuario = u.IdUsuario
              WHERE t.Token = @Token AND t.Usado = 0 AND t.FechaExpiracion > GETDATE()",
                    new { Token = token }
                );

                if (tokenInfo == null)
                {
                    ViewBag.Error = "El enlace de recuperación no es válido o ha expirado";
                    return View("RecuperarContrasena");
                }

                ViewBag.Token = token;
                ViewBag.Correo = tokenInfo.Correo;
                return View();
            }
        }

        [HttpPost]
        public IActionResult RestablecerContrasena(string token, string nuevaContrasena, string confirmarContrasena)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(nuevaContrasena) || string.IsNullOrEmpty(confirmarContrasena))
            {
                ViewBag.Error = "Todos los campos son obligatorios";
                ViewBag.Token = token;
                return View();
            }

            if (nuevaContrasena != confirmarContrasena)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                ViewBag.Token = token;
                return View();
            }

            if (nuevaContrasena.Length < 6)
            {
                ViewBag.Error = "La contraseña debe tener al menos 6 caracteres";
                ViewBag.Token = token;
                return View();
            }

            using (var context = new SqlConnection(_configuration.GetSection("ConnectionStrings:Connection").Value))
            {
                // Verificar token válido nuevamente
                var tokenInfo = context.QueryFirstOrDefault<dynamic>(
                    @"SELECT IdUsuario 
              FROM TTokenRecuperacion 
              WHERE Token = @Token AND Usado = 0 AND FechaExpiracion > GETDATE()",
                    new { Token = token }
                );

                if (tokenInfo == null)
                {
                    ViewBag.Error = "El enlace de recuperación no es válido o ha expirado";
                    return View();
                }

                using (var transaction = context.BeginTransaction())
                {
                    try
                    {
                        // Actualizar contraseña
                        string contrasenaEncriptada = EncriptarContrasena(nuevaContrasena);

                        context.Execute(
                            "UPDATE TUsuario SET Contrasenna = @Contrasenna WHERE IdUsuario = @IdUsuario",
                            new { Contrasenna = contrasenaEncriptada, IdUsuario = tokenInfo.IdUsuario },
                            transaction
                        );

                        // Marcar token como usado
                        context.Execute(
                            "UPDATE TTokenRecuperacion SET Usado = 1 WHERE Token = @Token",
                            new { Token = token },
                            transaction
                        );

                        transaction.Commit();

                        ViewBag.Exito = "Contraseña restablecida exitosamente. Ya puedes iniciar sesión.";
                        return View("Index");
                    }
                    catch
                    {
                        transaction.Rollback();
                        ViewBag.Error = "Error al restablecer la contraseña";
                        ViewBag.Token = token;
                        return View();
                    }
                }
            }
        }

        private string GenerarTokenRecuperacion()
        {
            return Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString("x");
        }

        private void EnviarCorreoRecuperacion(string correo, string nombre, string token)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential("tucorreo@gmail.com", "tupassword")
                };

                string urlRecuperacion = $"https://tudominio.com/Home/RestablecerContrasena?token={token}";

                var mensaje = new MailMessage
                {
                    From = new MailAddress("tucorreo@gmail.com", "Tico Tours"),
                    Subject = "Recuperación de Contraseña - Tico Tours",
                    Body = $@"
                <h2>Hola {nombre},</h2>
                <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                <p>Para crear una nueva contraseña, haz clic en el siguiente enlace:</p>
                <p><a href='{urlRecuperacion}' style='background-color: #10b981; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>Restablecer Contraseña</a></p>
                <p>Este enlace expirará en 2 horas.</p>
                <p>Si no solicitaste este cambio, puedes ignorar este correo.</p>
                <br>
                <p>Saludos,<br>El equipo de Tico Tours</p>
            ",
                    IsBodyHtml = true
                };

                mensaje.To.Add(correo);
                smtpClient.Send(mensaje);
            }
            catch (Exception ex)
            {
                // Log del error
                Console.WriteLine($"Error al enviar correo: {ex.Message}");
            }
        }
        #endregion
    }

}