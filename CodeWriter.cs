using ScaffoldEF.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScaffoldEF
{
    class CodeWriter : IDisposable
    {
        readonly StreamWriter writer;
        int indent;
        internal CodeWriter(string filename)
        {
            writer = new StreamWriter(filename);
        }

        public void WriteTable(Table table) => WriteTable(table, new List<ForeignKey>());

        public void WriteTable(Table table, List<ForeignKey> foreignKeys)
        {
            WriteText($"[Table(\"{table.Schema}.{table.Name}\")]");
            BeginBlock($"public class {table.Name}");

            foreach (var column in table.Columns)
            {
                if (column.IsPrimaryKey)
                {
                    WriteText("[Key]");
                    if (!column.IsIdentity)
                    {
                        WriteText("[DatabaseGenerated(DatabaseGeneratedOption.None)]");
                    }
                }

                if (column.Type.Contains("char"))
                {
                    if (!column.IsNullable)
                    {
                        WriteText("[Required]");
                    }

                    WriteText($"[StringLength({(column.MaxLength > 0 ? column.MaxLength.ToString() : "MAX")})]");
                }

                if (TypeMap.TryGet(column.Type, out var type))
                {
                    column.Type = type;
                }
                else
                {
                    throw new Exception($"Found to type map matching {column.Type}");
                }

                if (column.IsNullable && column.Type != "string")
                {
                    column.Type += "?";
                }

                WriteText($"public {column.Type} {column.Name} {{ get; set; }}");
            }

            foreach (var fk in foreignKeys.Where(fk => fk.FromSchema == table.Schema && fk.FromTable == table.Name))
            {
                WriteText($"public virtual {fk.ToSchema}.{fk.ToTable} {FormatFKName(fk, FKDirection.From)} {{ get; set; }}");
                WriteLine();
            }

            foreach (var fk in foreignKeys.Where(fk => fk.ToSchema == table.Schema && fk.ToTable == table.Name))
            {
                WriteText($"public virtual ICollection<{fk.FromSchema}.{fk.FromTable}> {FormatFKName(fk, FKDirection.To)} {{ get; set; }}");
                WriteLine();
            }

            var relations = foreignKeys.Where(fk => fk.ToSchema == table.Schema && fk.ToTable == table.Name).ToList();

            if (relations.Any())
            {
                BeginBlock("public static void Configure(DbModelBuilder modelBuilder");
                foreach (var relation in relations)
                {
                    WriteText($"modelBuilder.Entity<{table.Name}>()");
                    Indent();
                    WriteText($".HasMany(entity => entity.{FormatFKName(relation, FKDirection.To)})");
                    // TODO: WithOptional
                    WriteText($".WithRequired(entity => entity.{FormatFKName(relation, FKDirection.From)})");
                    if (relation.Columns.Count > 1)
                    {
                        WriteText($".HasForeignKey(entity => new {{ {string.Join(", ", relation.Columns.Select(c => $"entity.{c.From}"))} }});");
                    }
                    else
                    {
                        WriteText($".HasForeignKey(entity => entity.{relation.Columns.First().From});");
                    }
                    Deindent();
                }
                EndBlock();
            }

            EndBlock();
            EndBlock();
        }

        public void WriteText(string text)
        {
            WriteIndent();
            writer.WriteLine(text);
        }

        public void BeginBlock(string preamble)
        {
            WriteIndent();
            writer.WriteLine(preamble);
            WriteIndent();
            writer.WriteLine("{");
            indent++;
        }

        public void EndBlock()
        {
            indent--;
            WriteIndent();
            writer.WriteLine("}");
        }

        public void WriteIndent()
        {
            writer.Write(new string('\t', indent));
        }

        public void Indent()
        {
            indent++;
        }

        public void Deindent()
        {
            indent--;
        }

        public void WriteLine(string text = "")
        {
            writer.WriteLine(text);
        }

        public void Dispose()
        {
            writer.Dispose();
        }

        // TODO: Make this less insane
        private enum FKDirection
        {
            To,
            From
        }

        private static string FormatFKName(ForeignKey fk, FKDirection direction)
        {
            var name = (fk.Alias ?? fk.Name);
            if (name.StartsWith("FK_"))
            {
                name = name.Substring(3);
            }

            if (name.Contains("!"))
            {
                var parts = name.Split('!');
                return direction == FKDirection.To ? parts[0] : parts[1];
            }

            return name;
        }
    }
}
