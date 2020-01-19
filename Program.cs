using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScaffoldEF
{
    internal class Program
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

        private static void Main(string[] args)
        {
            var definition = Data.Definition.Load(File.OpenRead("export.xml"));
            using var output = new StreamWriter("export.txt");
            foreach (var schema in definition.Schemas)
            {
                foreach (var table in schema.Tables)
                {
                    output.BeginBlock($"namespace DataLayer.{schema.Name}");
                    output.WriteText($"[Table(\"{schema.Name}.{table.Name}\")]");
                    output.BeginBlock($"public class {table.Name}");

                    foreach (var column in table.Columns)
                    {
                        if (column.IsPrimaryKey)
                        {
                            output.WriteText("[Key]");
                            if (!column.IsIdentity)
                            {
                                output.WriteText("[DatabaseGenerated(DatabaseGeneratedOption.None)]");
                            }
                        }

                        if (column.Type.Contains("char"))
                        {
                            if (!column.IsNullable)
                            {
                                output.WriteText("[Required]");
                            }

                            output.WriteText($"[StringLength({(column.MaxLength > 0 ? column.MaxLength.ToString() : "MAX")})]");
                        }

                        if (typeMap.ContainsKey(column.Type))
                        {
                            column.Type = typeMap[column.Type];
                        }
                        else
                        {
                            Console.WriteLine(column.Type);
                        }

                        if (column.IsNullable && column.Type != "string")
                        {
                            column.Type += "?";
                        }

                        output.WriteText($"public {column.Type} {column.Name} {{ get; set; }}");
                    }

                    foreach (var fk in definition.ForeignKeys.Where(fk => fk.FromSchema == schema.Name && fk.FromTable == table.Name))
                    {
                        output.WriteText($"public virtual {fk.ToSchema}.{fk.ToTable} {FormatFKName(fk, FKDirection.From)} {{ get; set; }}");
                        output.WriteLine();
                    }

                    foreach (var fk in definition.ForeignKeys.Where(fk => fk.ToSchema == schema.Name && fk.ToTable == table.Name))
                    {
                        output.WriteText($"public virtual ICollection<{fk.FromSchema}.{fk.FromTable}> {FormatFKName(fk, FKDirection.To)} {{ get; set; }}");
                        output.WriteLine();
                    }

                    var relations = definition.ForeignKeys.Where(fk => fk.ToSchema == schema.Name && fk.ToTable == table.Name).ToList();

                    if (relations.Any())
                    {
                        output.BeginBlock("public static void Configure(DbModelBuilder modelBuilder");
                        foreach (var relation in relations)
                        {
                            output.WriteText($"modelBuilder.Entity<{table.Name}>()");
                            output.Indent();
                            output.WriteText($".HasMany(entity => entity.{FormatFKName(relation, FKDirection.To)})");
                            // TODO: WithOptional
                            output.WriteText($".WithRequired(entity => entity.{FormatFKName(relation, FKDirection.From)})");
                            if (relation.Columns.Count > 1)
                            {
                                output.WriteText($".HasForeignKey(entity => new {{ {string.Join(", ", relation.Columns.Select(c => $"entity.{c.From}"))} }});");
                            }
                            else
                            {
                                output.WriteText($".HasForeignKey(entity => entity.{relation.Columns.First().From});");
                            }
                            output.Deindent();
                        }
                        output.EndBlock();
                    }

                    output.EndBlock();
                    output.EndBlock();
                }
            }
        }

        private enum FKDirection
        {
            To,
            From
        }

        private static string FormatFKName(Data.ForeignKey fk, FKDirection direction)
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

    internal static class Extensions
    {
        private static int indent = 0;

        public static void WriteText(this StreamWriter writer, string text)
        {
            writer.WriteIndent();
            writer.WriteLine(text);
        }

        public static void BeginBlock(this StreamWriter writer, string preamble)
        {
            writer.WriteIndent();
            writer.WriteLine(preamble);
            writer.WriteIndent();
            writer.WriteLine("{");
            indent++;
        }

        public static void EndBlock(this StreamWriter writer)
        {
            indent--;
            writer.WriteIndent();
            writer.WriteLine("}");
        }

        public static void WriteIndent(this StreamWriter writer)
        {
            writer.Write(new string('\t', indent));
        }

        public static void Indent(this StreamWriter _)
        {
            indent++;
        }

        public static void Deindent(this StreamWriter _)
        {
            indent--;
        }
    }
}