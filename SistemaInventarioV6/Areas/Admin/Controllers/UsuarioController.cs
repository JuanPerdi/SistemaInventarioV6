﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Utilidades;

namespace SistemaInventarioV6.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]

    public class UsuarioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db; //necesitamos _db para acceder a los roles

        public UsuarioController(IUnidadTrabajo unidadTrabajo,ApplicationDbContext db)
        {
            _unidadTrabajo = unidadTrabajo;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarioLista=await _unidadTrabajo.UsuarioAplicacion.ObtenerTodos();
            var usersRole=await _db.UserRoles.ToListAsync();
            var roles = await _db.Roles.ToListAsync(); 

            foreach(var usuario in usuarioLista)
            {
                var roleId = usersRole.FirstOrDefault(u => u.UserId == usuario.Id).RoleId;
                usuario.Role=roles.FirstOrDefault(r=>r.Id==roleId).Name;
            }
            return Json(new { data = usuarioLista } );
        }

        [HttpPost]
        public async Task<IActionResult> BloquearDesbloquear([FromBody] string id)
        {
            var usuario = await _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Error de Usuario" });
            }
            if(usuario.LockoutEnd!=null && usuario.LockoutEnd > DateTime.Now)
            {
                //Usuario bloqueado
                usuario.LockoutEnd=DateTime.Now;
            }
            else
            {
                //Usuario desbloqueado
                usuario.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Operación exitosa" });
        }
        #endregion
    }
}
