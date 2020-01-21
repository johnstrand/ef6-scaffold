using System;
using System.Data.SqlClient;

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

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
