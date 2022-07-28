﻿namespace ManejoPresupuesto.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin    { get; set; }
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(x => x.BalanceDeposito);
        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public decimal Total => BalanceDepositos - BalanceRetiros;



        public class TransaccionesPorFecha 

        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }
            public decimal BalanceDeposito => Transacciones.Where(XmlConfigurationExtensions
                => XmlConfigurationExtensions.TipoOperacion == TipoOperacion.Ingreso).Sum(x => x.Monto);
            public decimal BalanceRetiros => Transacciones.Where(XmlConfigurationExtensions
               => XmlConfigurationExtensions.TipoOperacion == TipoOperacion.Gasto).Sum(x => x.Monto);
        }

    }
}