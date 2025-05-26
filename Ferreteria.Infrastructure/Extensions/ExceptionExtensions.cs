using System;
using System.Text;

namespace Ferreteria.Infrastructure.Extensions
{
    public static class ExceptionExtensions
    {
        public static string Message(this Exception value)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(value.Message);

            if (value.InnerException != null)
            {
                builder.AppendLine(value.InnerException.Message);

                if (value.InnerException.InnerException != null)
                    builder.AppendLine(Message(value.InnerException.InnerException));
            }
            return builder.ToString();
        }

        public static string Trace(this Exception value)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(value.StackTrace);

            if (value.InnerException != null)
            {
                builder.AppendLine(value.InnerException.StackTrace);

                if (value.InnerException.InnerException != null)
                    builder.AppendLine(Trace(value.InnerException.InnerException));
            }
            return builder.ToString();
        }
    }
}
