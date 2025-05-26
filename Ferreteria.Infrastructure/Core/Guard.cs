using Ferreteria.Infrastructure.Extensions;
using System.Diagnostics;

namespace Ferreteria.Infrastructure.Core
{
    [DebuggerStepThrough]
    public static class Guard
    {
        public static Param<T> Check<T>(T value, string name = "")
        {
            return new Param<T>(name, value, null);
        }
    }
}
