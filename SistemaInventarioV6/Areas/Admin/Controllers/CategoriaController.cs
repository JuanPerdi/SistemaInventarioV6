using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;

namespace SistemaInventarioV6.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]

    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriaController(IUnidadTrabajo unidadTrabajo)
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
            Categoria categoria = new Categoria();
            if (id == null)
            {
                //nueva bodega
                categoria.Estado = true;
                return View(categoria);
            }
            //Actualizar Bodega
            categoria = await _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());//si el id es nulo que no de error
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]//evitar falsifiación de solicitudes
        public async Task<IActionResult> Upsert (Categoria categoria)
        {
            if(ModelState.IsValid) //comprueba si el modelo es válido en todas sus propiedades
            {
                if (categoria.Id==0) //Nuevo registro
                {
                    await _unidadTrabajo.Categoria.Agregar(categoria);
                    TempData[DS.Exitosa] = "Categoria creada exitósamente";
                }
                else //Actualizar registro
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                    TempData[DS.Exitosa] = "Categoria actualizada exitósamente";

                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar Categoria";
            return View(categoria);
        }

        #region API
        //esto es porque se van a llamar mediante JS
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new {data=todos});
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaDB = await _unidadTrabajo.Categoria.Obtener(id);
            if(categoriaDB == null)
            {
                return Json(new { succes = false, message = "Error al borrar la Categoria" });
            }
            _unidadTrabajo.Categoria.Remover(categoriaDB);//no ponemos await porque no es un método asíncrono
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitósamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre,int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Categoria.ObtenerTodos();
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
