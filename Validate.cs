using System;

namespace ScaffoldEF
{
    internal static class Validate
    {
        internal static bool AssertIsTrue(this bool value, params string[] error)
        {
            if (!value)
            {
                throw new Exception(string.Join(Environment.NewLine, error));
            }
            return value;
        }

        internal static bool AssertIsFalse(this bool value, params string[] error)
        {
            if (value)
            {
                throw new Exception(string.Join(Environment.NewLine, error));
            }
            return value;
        }
    }
}
