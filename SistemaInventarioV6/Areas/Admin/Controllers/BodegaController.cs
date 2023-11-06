using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;

namespace SistemaInventarioV6.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
                _unidadTrabajo=unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            //? significa que pueude ser nulo
            Bodega bodega = new Bodega();
            if (id == null)
            {
                //nueva bodega
                bodega.Estado = true;
                return View(bodega);
            }
            //Actualizar Bodega
            bodega = await _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());//si el id es nulo que no de error
            if (bodega == null)
            {
                return NotFound();
            }
            return View(bodega);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//evitar falsifiación de solicitudes
        public async Task<IActionResult> Upsert (Bodega bodega)
        {
            if(ModelState.IsValid) //comrpueba si el modelo es válido en todas sus propiedades
            {
                if (bodega.Id==0) //Nuevo registro
                {
                    await _unidadTrabajo.Bodega.Agregar(bodega);
                }
                else //Actualizar registro
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            return View(bodega);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new {data=todos});
        }

        #endregion
    }
}
