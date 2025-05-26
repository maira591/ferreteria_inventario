using System;
using System.Collections.Generic;
using System.Linq;

namespace Ferreteria.Infrastructure.Core
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            object list = @this as IList<T>;
            if (list == null)
            {
                list = @this.ToList<T>();
            }
            foreach (T t in (IEnumerable<T>)list)
            {
                action(t);
            }
        }
    }
}
