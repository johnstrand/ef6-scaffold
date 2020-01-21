using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ScaffoldEF.Settings
{
    static class ConnectionManager
    {
        private static readonly Dictionary<string, string> connections;
        private static readonly string file;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "No can do")]
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
                connections = Read();
            }
            else
            {
                connections = new Dictionary<string, string>();
            }

        }

        private static Dictionary<string, string> Read()
        {
            var xDoc = new XmlDocument();
            xDoc.Load(file);
            var result = new Dictionary<string, string>();
            foreach (var node in xDoc.SelectNodes("connections/connection").Cast<XmlNode>())
            {
                result.Add(node.Attributes["name"].Value.ToLower(), node.InnerText.Trim());
            }

            return result;
        }

        internal static bool Exists(string name) => connections.ContainsKey(name.ToLower());
        internal static IEnumerable<KeyValuePair<string, string>> List() => connections.OrderBy(c => c.Key);

        internal static bool TryRemove(string name) => connections.Remove(name.ToLower());

        internal static bool TryGet(string name, out string value)
        {
            return connections.TryGetValue(name.ToLower(), out value);
        }

        internal static void AddOrUpdate(string name, string value)
        {
            TryRemove(name);
            connections.Add(name.ToLower(), value);
        }

        internal static void Save()
        {
            using var writer = XmlWriter.Create(file, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true });
            writer.WriteStartElement("connections");
            connections.ToList().ForEach(c =>
            {
                writer.WriteStartElement("connection");
                writer.WriteAttributeString("name", c.Key);
                writer.WriteString(c.Value);
                writer.WriteEndElement();
            });
            writer.WriteEndElement();
        }
    }
}
