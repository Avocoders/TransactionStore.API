using System;
using System.Data;
using System.Data.SqlClient;

namespace TransactionStore.Data
{
    static class Connection
    {
        public static IDbConnection GetConnection()
        {
            string connectionString = @"Data Source=80.78.240.16;Initial Catalog=TransactionStore.DB;User Id = tSystem;Password = qwe!23";
            IDbConnection connection = new SqlConnection(connectionString);
            return connection;
        }
    }
}
