using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        public void Agregar(T entidad)
        {
            throw new NotImplementedException();
        }

        public Task<T> Obtener(int id)
        {
            throw new NotImplementedException();
        }

        public Task<T> ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> ObtenerTodos(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string incluirPropiedades = null, bool isTracking = true)
        {
            throw new NotImplementedException();
        }

        public void Remove(T entidad)
        {
            throw new NotImplementedException();
        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
            throw new NotImplementedException();
        }
    }
}
