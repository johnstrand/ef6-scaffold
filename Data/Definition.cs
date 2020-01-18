using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ScaffoldEF.Data
{
    [XmlRoot("schemas")]
    public class Definition
    {
        [XmlElement("schema")]
        public List<Schema> Schemas { get; set; }

        public static List<Schema> Load(Stream str)
        {
            var serializer = new XmlSerializer(typeof(Definition));
            return (serializer.Deserialize(str) as Definition).Schemas;
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
        [XmlArray("foreign_keys")]
        [XmlArrayItem("foreign_key")]
        public List<ForeignKey> ForeignKeys { get; set; }
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
        public string MaxLength { get; set; }
        [XmlElement("precision")]
        public string Precision { get; set; }
        [XmlElement("primary_key")]
        public bool PrimaryKey { get; set; }
    }

    public class ForeignKey
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("column")]
        public List<ForeignKeyColumn> Column { get; set; }
    }

    public class ForeignKeyColumn
    {
        [XmlElement("ref_table")]
        public string RefTable { get; set; }

        [XmlElement("ref_column")]
        public string RefColumn { get; set; }

        [XmlElement("parent_table")]
        public string ParentTable { get; set; }

        [XmlElement("parent_column")]
        public string ParentColumn { get; set; }
    }


}
