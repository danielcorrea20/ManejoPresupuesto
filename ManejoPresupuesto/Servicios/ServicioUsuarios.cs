namespace ManejoPresupuesto.Servicios
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioId();
    }
    public class ServicioUsuarios: IServicioUsuarios
    {
        public static object ObtenerUsuario { get; internal set; }

        public int ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
