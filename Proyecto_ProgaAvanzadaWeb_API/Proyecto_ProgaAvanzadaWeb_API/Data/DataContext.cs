using Microsoft.Data.SqlClient;
using System.Data;

namespace Proyecto_PrograAvanzadaWeb_API.Data
{
    public class DataContext
    {
        private readonly string _connectionString;

        public DataContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
