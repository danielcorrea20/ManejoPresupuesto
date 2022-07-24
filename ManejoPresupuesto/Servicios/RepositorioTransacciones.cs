using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Crear(Transaccion transaccion);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task Borrar(int id);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly String connectionstring;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionstring = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionstring);
            var Id = await connection.QuerySingleAsync<int>("Transacciones_insertar", 
                new { transaccion.UsuarioId, transaccion.FechaTransaccion, transaccion.Monto, 
                    transaccion.CategoriaId, transaccion.CuentaId, transaccion.Nota }, commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = Id;
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionstring);
            await connection.ExecuteAsync("Transaciones_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"select Transaciones. *, cat.TipoOperacionId
                                                from Transaciones
                                                inner join Categorias cat
                                                on cat.Id = Transaciones.CategoriaId
                                                where Transaciones.Id = @Id and Transaciones.UsuarioId = @UsuarioId");
        }

        public Task Borrar(int id)
        {
            return null;
        }
    }
}
