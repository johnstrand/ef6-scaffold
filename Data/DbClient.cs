using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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

        public T Query<T>(string query, object args = null)
        {
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddRange(GetParameters(args).ToArray());
            using var reader = command.ExecuteXmlReader();
            //reader.MoveToContent();
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(reader);
        }

        private IEnumerable<SqlParameter> GetParameters(object args)
        {
            if (args == null)
            {
                yield break;
            }

            foreach (var prop in args.GetType().GetProperties())
            {
                yield return new SqlParameter("@" + prop.Name, prop.GetValue(args) ?? DBNull.Value);
            }
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
