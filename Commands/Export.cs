using System;
using System.Collections.Generic;
using System.Text;

namespace ScaffoldEF.Commands
{
    [Command("export")]
    static class Export
    {
        [Command("fk")]
        internal static void ForeignKeys(string target)
        {
            Settings.ConnectionManager.TryGet(target, out var connection).AssertIsTrue($"Connection '{target}' is not configured");
            using var db = new Data.DbClient(connection);
            var keys = db.Query<Data.ForeignKeysWrapper>(Query.ExportForeignKeys).ForeignKeys;
        }
    }
}
