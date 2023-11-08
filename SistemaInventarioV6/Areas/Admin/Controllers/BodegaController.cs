using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

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
                    TempData[DS.Exitosa] = "Bodega creada exitósamente";
                }
                else //Actualizar registro
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                    TempData[DS.Exitosa] = "Bodega actualizada exitósamente";

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar Bodega";
            return View(bodega);
        }

        #region API
        //esto es porque se van a llamar mediante JS
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new {data=todos});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var bodegaDb = await _unidadTrabajo.Bodega.Obtener(id);
            if(bodegaDb==null)
            {
                return Json(new { succes = false, message = "Error al borrar la Bodega" });
            }
            _unidadTrabajo.Bodega.Remover(bodegaDb);//no ponemos await porque no es un método asíncrono
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Bodega borrada exitósamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre,int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Bodega.ObtenerTodos();
            if (id == 0)
            {
                valor=lista.Any(b=>b.Nombre.ToLower().Trim()==nombre.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }
            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new {data=false});
        }

        #endregion
    }
}
