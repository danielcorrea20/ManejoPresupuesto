namespace ManejoPresupuesto.Models
{
    public class TransaccionActualizacionViewModel: TransaccionCreacionViewModel
    {
        public int CuentaAnterior { get; set; }
        public decimal MontoAnterior { get; set; }

    }
}
