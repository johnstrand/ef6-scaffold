using System;
using System.Collections.Generic;
using System.Text;

namespace ScaffoldEF.Data
{
    static class TypeMap
    {
        private static readonly Dictionary<string, string> typeMap = new Dictionary<string, string>
        {
            { "datetime", "DateTime" },
            { "date", "DateTime" },
            { "datetime2", "DateTime" },
            { "bigint", "long" },
            { "bit", "bool" },
            { "decimal", "decimal" },
            { "float", "float" },
            { "real", "single" },
            { "int", "int" },
            { "char", "string" },
            { "nchar", "string" },
            { "varchar", "string" },
            { "nvarchar", "string" },
            { "smallint", "short" },
            { "tinyint", "short" },
            { "uniqueidentifier", "Guid" },
        };

        internal static bool HasType(string name) => typeMap.ContainsKey(name);
        internal static bool TryGet(string name, out string value) => typeMap.TryGetValue(name, out value);
        internal static string Get(string name) => typeMap[name];
    }
}
