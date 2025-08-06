using System.Security.Cryptography;
using System.Text;

namespace Proyecto_ProgaAvanzadaWeb_API.Helpers
{
    public class PasswordHelper
    {
        public static string EncriptarContrasena(string contrasena)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(contrasena);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
