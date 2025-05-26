using System;

namespace Ferreteria.Infrastructure.Extensions
{
    public abstract class Param
    {
        public const string DefaultName = "";

        public Func<string> ExtraMessageFunc
        {
            get;
            set;
        }

        public string Name
        {
            get;
            private set;
        }

        protected Param(string name, Func<string> extraMessageFunc = null)
        {
            this.Name = name;
            this.ExtraMessageFunc = extraMessageFunc;
        }
    }
}
