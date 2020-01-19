using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ScaffoldEF.Data
{
    [XmlRoot("export")]
    public class Definition
    {
        [XmlArray("schemas")]
        [XmlArrayItem("schema")]
        public List<Schema> Schemas { get; set; }

        [XmlArray("foreign_keys")]
        [XmlArrayItem("foreign_key")]
        public List<ForeignKey> ForeignKeys { get; set; }

        public static Definition Load(Stream str)
        {
            var serializer = new XmlSerializer(typeof(Definition));
            return (serializer.Deserialize(str) as Definition);
        }
    }

    public class Schema
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("table")]
        public List<Table> Tables { get; set; }
    }

    public class Table
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlArray("columns")]
        [XmlArrayItem("column")]
        public List<Column> Columns { get; set; }
    }

    public class Column
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("is_identity")]
        public bool IsIdentity { get; set; }

        [XmlElement("is_nullable")]
        public bool IsNullable { get; set; }

        [XmlElement("is_rowguidcol")]
        public bool IsRowguidcol { get; set; }

        [XmlElement("max_length")]
        public int MaxLength { get; set; }

        [XmlElement("precision")]
        public int Precision { get; set; }

        [XmlElement("scale")]
        public int Scale { get; set; }

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("is_primary_key")]
        public bool IsPrimaryKey { get; set; }
    }

    public class ForeignKey
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("alias")]
        public string Alias { get; set; }

        [XmlElement("from_schema")]
        public string FromSchema { get; set; }

        [XmlElement("from_table")]
        public string FromTable { get; set; }

        [XmlElement("to_schema")]
        public string ToSchema { get; set; }

        [XmlElement("to_table")]
        public string ToTable { get; set; }

        [XmlElement("column")]
        public List<ForeignKeyColumn> Columns { get; set; }
    }

    public class ForeignKeyColumn
    {
        [XmlElement("to")]
        public string To { get; set; }

        [XmlElement("from")]
        public string From { get; set; }
    }
}