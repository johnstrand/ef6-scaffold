using ScaffoldEF.Data;
using System;
using System.IO;
using System.Linq;

namespace ScaffoldEF
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            args = new[] { "configure" };
            var definition = Definition.Load(File.OpenRead("export.xml"));
            using var output = new CodeWriter("export.txt");
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