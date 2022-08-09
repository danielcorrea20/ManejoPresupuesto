using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEamil(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }

    public class RepositorioUsuarios: IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }   

        public async Task<int> CrearUsuario (Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                                    insert into Usuarios (Email, EmailNormalizado, PasswordHash)
                                    values (@Email, @EmailNormalizado, @PasswordHash);
                                    select scope_identity();", usuario);
            return id;
        }

        public async Task<Usuario> BuscarUsuarioPorEamil(string emailNormalizado)
        {
            var usuario = new Usuario(); using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                "Select * from Usuarios where EmailNormalizado = @EmailNormalizado", new { emailNormalizado });
        }
    }

}
