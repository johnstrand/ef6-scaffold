using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScaffoldEF.Commands
{
    [Command("export")]
    static class Export
    {
        [Command("fk")]
        internal static void ForeignKeys(string target, string file)
        {
            Console.WriteLine($"Exporting foreign keys for '{target}'");
            Settings.ConnectionManager.TryGet(target, out var connection).AssertIsTrue($"Connection '{target}' is not configured");
            using var db = new Data.DbClient(connection);
            var keys = db.Query<Data.ForeignKeysWrapper>(Query.ExportForeignKeys).ForeignKeys;
            Console.WriteLine($"Exported {keys.Count} key(s)");
            using var export = new OfficeOpenXml.ExcelPackage();
            using var ws = export.Workbook.Worksheets.Add("Exports");
            var row = 2;
            ws.Cells[1, 1].Value = "Name";
            ws.Cells[1, 2].Value = "From";
            ws.Cells[1, 3].Value = "Property";
            ws.Cells[1, 4].Value = "To";
            ws.Cells[1, 5].Value = "Property";
            ws.Cells[1, 6].Value = "Columns";
            foreach (var key in keys)
            {
                ws.Cells[row, 1].Value = key.Name;
                ws.Cells[row, 2].Value = $"{key.FromSchema}.{key.FromTable}";
                ws.Cells[row, 3].Value = key.Alias.Split('!').First();
                ws.Cells[row, 4].Value = $"{key.ToSchema}.{key.ToTable}";
                ws.Cells[row, 5].Value = key.Alias.Split('!').Last();
                ws.Cells[row, 6].Value = string.Join(", ", key.Columns.Select(c => $"{c.From} = {c.To}"));
                row++;
            }

            ws.Cells.AutoFitColumns();
            Console.WriteLine($"Exporting key list to '{file}'");
            export.SaveAs(File.Open(file, FileMode.Create));
        }
    }
}
