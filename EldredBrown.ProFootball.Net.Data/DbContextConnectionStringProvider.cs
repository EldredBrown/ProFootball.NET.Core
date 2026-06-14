using Microsoft.EntityFrameworkCore;

namespace EldredBrown.ProFootball.Net.Data
{
    public class DbContextConnectionStringProvider(ProFootballDbContext dbContext) : IConnectionStringProvider
    {
        public string GetConnectionString()
        {
            return dbContext.Database.GetDbConnection().ConnectionString;
        }
    }
}
