using System;

namespace Ferreteria.Infrastructure.Core
{
    public static class GuardStringExtensions
    {

        private static bool StringEquals(string x, string y, StringComparison? comparison = null)
        {
            if (!comparison.HasValue)
            {
                return string.Equals(x, y);
            }
            return string.Equals(x, y, comparison.Value);
        }
    }
}
