using Ferreteria.Infrastructure.Extensions;
using System.Diagnostics;

namespace Ferreteria.Infrastructure.Core
{
    public static class GuardObjectExtensions
    {
        [DebuggerStepThrough]
        public static Param<T> IsNotNull<T>(this Param<T> param)
        where T : class
        {
            if (param.Value == null)
            {
                throw new System.Exception("The value is null");
            }
            return param;
        }
    }
}
