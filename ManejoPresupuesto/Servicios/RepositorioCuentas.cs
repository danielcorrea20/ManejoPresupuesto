﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }


    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionstring;
   
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionstring = configuration.GetConnectionString("DefaultConnection");    
        }

        public async Task crear (Cuenta cuenta)
        {

            using var connection = new SqlConnection(connectionstring);
            var Id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas (Nombre, TipoDeCuentaId, Descripcion, Balance)
                                                            VALUES (@Nombre, @TipoDeCuentaId, @Descripcion, @Balance);

                                                            SELECT SCOPE_IDENTITY();", cuenta);



        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionstring);
            return await connection.QueryAsync<Cuenta>(@"select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre AS TipoCuenta
                                                        from Cuentas
                                                        inner join TiposCuentas Tc
                                                        on tc .Id = Cuentas.TipoDeCuentaId
                                                        where tc.UsuarioId = @UsuarioId
                                                        order by tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection= new SqlConnection(connectionstring);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"select Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, tc.Id
                                                        inner join TiposCuentas Tc
                                                        on tc .Id = Cuentas.TipoDeCuentaId
                                                        where tc.UsuarioId = @UsuarioId and Cuentas.Id=@Id", new { id, usuarioId } );

        }
    }

}
