using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventarioV6.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public MarcaController(IUnidadTrabajo unidadTrabajo)
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
            Marca marca = new Marca();
            if (id == null)
            {
                //nueva bodega
                marca.Estado = true;
                return View(marca);
            }
            //Actualizar Bodega
            marca = await _unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());//si el id es nulo que no de error
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//evitar falsifiación de solicitudes
        public async Task<IActionResult> Upsert (Marca marca)
        {
            if(ModelState.IsValid) //comrpueba si el modelo es válido en todas sus propiedades
            {
                if (marca.Id==0) //Nuevo registro
                {
                    await _unidadTrabajo.Marca.Agregar(marca);
                    TempData[DS.Exitosa] = "Marca creada exitósamente";
                }
                else //Actualizar registro
                {
                    _unidadTrabajo.Marca.Actualizar(marca);
                    TempData[DS.Exitosa] = "Marca actualizada exitósamente";

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar Marca";
            return View(marca);
        }

        #region API
        //esto es porque se van a llamar mediante JS
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Marca.ObtenerTodos();
            return Json(new {data=todos});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var marcaDB = await _unidadTrabajo.Marca.Obtener(id);
            if(marcaDB == null)
            {
                return Json(new { succes = false, message = "Error al borrar la Marca" });
            }
            _unidadTrabajo.Marca.Remover(marcaDB);//no ponemos await porque no es un método asíncrono
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Marca borrada exitósamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre,int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Marca.ObtenerTodos();
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
