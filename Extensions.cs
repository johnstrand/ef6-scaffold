using System;
using System.IO;

namespace ScaffoldEF
{
    internal static class Extensions
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

        internal static string ReadAndClose(this Stream stream)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
