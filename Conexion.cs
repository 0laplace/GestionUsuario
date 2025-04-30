using System.Data.SqlClient;

namespace GestionUsuario
{
    public static class Conexion
    {
        private static readonly string connectionString = "Server=VIOLET\\SQLEXPRESS;Database=UsuariosDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(connectionString);
        }
    }
}
