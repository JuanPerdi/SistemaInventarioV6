﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        //Task<T> lo utilizamos para que sean métodos asíncronos
        Task<T> Obtener(int id);
        Task<IEnumerable<T>> ObtenerTodos(
            Expression<Func<T,bool>> filtro=null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy=null,
            string incluirPropiedades=null,
            bool isTracking=true
            );

        Task<T> ObtenerPrimero(
            Expression<Func<T, bool>> filtro = null,
            string incluirPropiedades = null,
            bool isTracking = true
            );

        Task Agregar(T entidad);
        void Remove(T entidad);
        void RemoverRango(IEnumerable<T> entidad);
    }
}
