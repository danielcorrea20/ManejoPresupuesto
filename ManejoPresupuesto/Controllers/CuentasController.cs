using AutoMapper;
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
        private readonly IMapper mapper;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, 
            IRepositorioCuentas repositorioCuentas, IMapper mapper)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
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



        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);
          
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);
            
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);

        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }
            var tipoCuenta= await repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoDeCuentaId, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }
            await repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");

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

        
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);
            
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");

        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuenta = await repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuenta.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));


        }

       

    }
}