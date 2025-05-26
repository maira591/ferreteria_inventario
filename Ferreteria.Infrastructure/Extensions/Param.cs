using System;

namespace Ferreteria.Infrastructure.Extensions
{
    public class Param<T> : Param
    {
        public T Value
        {
            get;
            private set;
        }

        public Param(string name, T value, Func<string> extraMessageFunc = null) : base(name, extraMessageFunc)
        {
            this.Value = value;
        }
    }
}
