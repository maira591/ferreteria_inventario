using System;

namespace Ferreteria.Domain.Utils
{
    public class HandleException : Exception
    {
        public HandleException(string message) : base(message)
        {
        }
    }
}
