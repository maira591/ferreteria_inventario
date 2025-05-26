using System.Collections.Generic;

namespace Ferreteria.DataAccess.Model.Generics
{
    public class GenericJsonList<T>
    {
        public IEnumerable<T> List { get; set; }
    }
}
