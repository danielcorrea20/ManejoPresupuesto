namespace ManejoPresupuesto.Models
{
    public class PaginacionViewModel
    {
        public int Pagina { get; set; } = 1;
        private int recordsPorPagina { get; set; } = 10;
        private int cantidadMaximaRecordsPorPagina  { get; set; } = 50;

        public int RecordsPoPagina 
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                recordsPorPagina = (value > cantidadMaximaRecordsPorPagina)
                    ? cantidadMaximaRecordsPorPagina : value;
            }

            

        }
        public int RecordsASaltar => recordsPorPagina * (Pagina - 1);


    }

}
