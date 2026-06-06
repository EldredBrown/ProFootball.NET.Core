using Microsoft.EntityFrameworkCore;

namespace EldredBrown.ProFootball.Net.Data.Tests.RepositoryTests
{
    internal class TestDbContext : ProFootballDbContext
    {
        internal static ProFootballDbContext CreateFakeDbContextWithInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ProFootballDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var fakeDbContext = new ProFootballDbContext(options);
            return fakeDbContext;
        }
    }
}
