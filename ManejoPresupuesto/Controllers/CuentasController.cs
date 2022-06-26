using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController: Controller
  
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, 
            IRepositorioCuentas repositorioCuentas)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioTiposCuentas = repositorioTiposCuentas;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(Grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = Grupo.Key,
                    Cuentas = Grupo.AsEnumerable()
                }).ToList();

            return View(modelo);




        }

        [HttpGet]
        public async Task<IActionResult> Crear()

        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
          
            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);

        }

        [HttpPost]
        public async Task<IActionResult> Crear (CuentaCreacionViewModel Cuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(Cuenta.TipoDeCuentaId, usuarioId);

            if (tipoCuenta is null){
                return RedirectToAction("NoEncontrado", "Home");

            }
            if (!ModelState.IsValid)
            {
                Cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(Cuenta);
            }
            await repositorioCuentas.crear(Cuenta);
            return RedirectToAction("Index");
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuenta = await repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuenta.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));


        }

       

    }
}