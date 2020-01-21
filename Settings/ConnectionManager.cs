using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ScaffoldEF.Settings
{
    static class ConnectionManager
    {
        private static readonly List<Connection> connections;
        private static readonly string file;

        static ConnectionManager()
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ScaffoldEF");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            file = Path.Combine(directory, "connections.xml");

            if (File.Exists(file))
            {
                connections = Read().ToList();
            }
            else
            {
                connections = new List<Connection>();
            }

        }

        private static IEnumerable<Connection> Read()
        {
            var xDoc = new XmlDocument();
            xDoc.Load(file);
            foreach (var node in xDoc.SelectNodes("connections/connection").Cast<XmlNode>())
            {
                yield return new Connection
                {
                    Name = node.Attributes["name"].Value,
                    String = node.InnerText.Trim()
                };
            }
        }

        internal static IEnumerable<Connection> List() => connections.OrderBy(c => c.Name);

        internal static bool TryRemove(string name) => connections.RemoveAll(c => c.Name == name) > 0;

        internal static bool TryGet(string name, out string value)
        {
            value = connections.FirstOrDefault(c => c.Name == name)?.String;
            return !string.IsNullOrWhiteSpace(value);
        }

        internal static void AddOrUpdate(string name, string value)
        {
            TryRemove(name);
            connections.Add(new Connection { Name = name, String = value });
        }

        internal static void Save()
        {
            using var writer = XmlWriter.Create(file, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true });
            writer.WriteStartElement("connections");
            connections.ForEach(c =>
            {
                writer.WriteStartElement("connection");
                writer.WriteAttributeString("name", c.Name);
                writer.WriteString(c.String);
                writer.WriteEndElement();
            });
            writer.WriteEndElement();
        }
    }

    class Connection
    {
        public string Name { get; set; }
        public string String { get; set; }
    }
}
