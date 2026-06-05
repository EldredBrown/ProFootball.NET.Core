using Microsoft.EntityFrameworkCore;

namespace EldredBrown.ProFootball.Net.Data
{
    public class DbContextConnectionStringProvider : IConnectionStringProvider
    {
        private readonly ProFootballDbContext _dbContext;

        public DbContextConnectionStringProvider(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string GetConnectionString()
        {
            return _dbContext.Database.GetDbConnection().ConnectionString;
        }
    }
}
