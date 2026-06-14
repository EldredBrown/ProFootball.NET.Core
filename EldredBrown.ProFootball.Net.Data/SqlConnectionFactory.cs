using System.Data;

using Microsoft.Data.SqlClient;

namespace EldredBrown.ProFootball.Net.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
