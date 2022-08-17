using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId, int id = 0);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }

    public class RepositoriosTiposCuentas: IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositoriosTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");   
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden)
                                                    Values (@Nombre, @UsuarioId, 0); SELECT SCOPE_IDENTITY();"
                                                    , tipoCuenta);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId, int id = 0)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1
                                                                        from TiposCuentas
                                                                 where Nombre = @Nombre and usuarioId = @UsuarioId and Id <> @id;",
                                                                  new {nombre, usuarioId, id});
            return existe == 1;

        }
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"select Id, Nombre, Orden
                                                            from TiposCuentas
                                                            where UsuarioId = @UsuarioId
                                                             order by Orden", new {usuarioId});
        }

        public async Task Actualizar (TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection (connectionString);
            await connection.ExecuteAsync(@"update TiposCuentas
                                            set Nombre=@Nombre
                                            where Id=@Id", tipoCuenta);
        }
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"select Id, UsuarioId, Orden, Nombre
                                                                        from TiposCuentas
                                                                        where Id=@Id AND UsuarioId=@UsuarioId"
                                                                         ,new {id, usuarioId});
        } 
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"delete TiposCuentas
                                            where Id = @Id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "update TiposCuentas set Orden = @Orden where Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }

        
    }

}
