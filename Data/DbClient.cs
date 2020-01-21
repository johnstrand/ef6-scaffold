using System;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace ScaffoldEF.Data
{
    internal class DbClient : IDisposable
    {
        private readonly SqlConnection connection;
        public DbClient(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public T Query<T>(string query)
        {
            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteXmlReader();
            //reader.MoveToContent();
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(reader);
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
