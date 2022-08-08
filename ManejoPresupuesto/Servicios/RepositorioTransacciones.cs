using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
        Task ObtenerPorCuentaId(ParametroObtenerTransaccionesPorUsuario parametro);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año);
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
            var Id = await connection.QuerySingleAsync<int>("Transaciones_Insertar", 
                new { transaccion.UsuarioId, transaccion.FechaTransacion, transaccion.Monto, 
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
                    transaccion.FechaTransacion,
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
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"select Transaciones. *,
                                                cat.TipoOperacionId
                                                from Transaciones
                                                inner join Categorias cat
                                                on cat.Id = Transaciones.CategoriaId
                                                where Transaciones.Id = @Id and 
                                                Transaciones.UsuarioId = @UsuarioId",
                                                new {id, usuarioId});
        }

       

        public async Task Borrar (int id)
        {
            using var connection = new SqlConnection(connectionstring);
            await connection.ExecuteAsync("Transaciones_Borrar", 
                new {id}, commandType: System.Data.CommandType.StoredProcedure);

        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryAsync<Transaccion>
                                (@"select t.Id, t.Monto, t.FechaTransacion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId
                                    from Transaciones t
                                    inner join categorias c
                                    on c.Id = t.CategoriaId
                                    inner join cuentas cu
                                    on cu.Id = t.CuentaId
                                    where t.CuentaId = @CuentaId and t.UsuarioId = @UsuarioId  
                                    and FechaTransacion between 
                                    @FechaInicio and @FechaFin", modelo);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryAsync<Transaccion>
                                (@"select t.Id, t.Monto, t.FechaTransacion, c.Nombre as Categoria, cu.Nombre as Cuenta, c.TipoOperacionId, Nota
                                    from Transaciones t
                                    inner join categorias c
                                    on c.Id = t.CategoriaId
                                    inner join cuentas cu
                                    on cu.Id = t.CuentaId
                                    where t.UsuarioId = @UsuarioId  
                                    and FechaTransacion between 
                                    @FechaInicio and @FechaFin
                                    order by t.FechaTransacion desc", modelo);
        }

        public Task ObtenerPorCuentaId(ParametroObtenerTransaccionesPorUsuario parametro)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"select datediff (d, @fechaInicio, FechaTransacion) /7 + 1 as Semana,
                                sum(Monto) as Monto, cat.TipoOperacionId
                                from Transaciones
                                inner join categorias cat
                                on cat.Id = Transaciones.CategoriaId
                                where Transaciones.UsuarioId = @usuarioId and
                                FechaTransacion between @fechaInicio and @fechaFin
                                group by datediff(d, @fechaInicio, FechaTransacion) / 7, cat.TipoOperacionId", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"select MONTH(FechaTransacion) as Mes,
                                                                        sum(Monto) as Monto, cat.TipoOperacionId
                                                                        from Transaciones
                                                                        inner join Categorias cat
                                                                        on cat.Id = Transaciones.CategoriaId
                                                                        where Transaciones.UsuarioId = @usuarioId and year(FechaTransacion) = @Año
                                                                        Group by MONTH(FechaTransacion), cat.TipoOperacionId", new {usuarioId, año});
        }
    }

}
