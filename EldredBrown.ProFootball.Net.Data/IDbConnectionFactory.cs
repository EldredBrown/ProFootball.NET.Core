using System.Data;

namespace EldredBrown.ProFootball.Net.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionString);
    }
}
