using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos.Especificaciones
{
    public class PagedList<T> :List<T>
    {
        public MetaData MetaData { get; set; }
        public PagedList(List<T> items,int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                PageSize = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize) //1,5 lo transformaría a 2
            };
            AddRange(items);//Añade los elementos de la colección al final de la lista
        }
        public static PagedList<T> ToPagedList(IEnumerable<T> entidad,int pageNumber,int pageSize)
        {
            var count = entidad.Count();
            var items=entidad.Skip((pageNumber-1)*pageSize).Take(pageSize).ToList(); //saltar al número de página que coresponda
            return new PagedList<T>(items,count,pageNumber,pageSize);
        }
    }
}
