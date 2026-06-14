using System.Threading.Tasks;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Provides access to an external data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SharedRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class SharedRepository(ProFootballDbContext dbContext) : ISharedRepository
    {
        /// <summary>
        /// Saves changes made to the data store.
        /// </summary>
        /// <returns>The number of entities affected.</returns>
        public int SaveChanges()
        {
            return dbContext.SaveChanges();
        }

        /// <summary>
        /// Asynchronously saves changes made to the data store.
        /// </summary>
        /// <returns>The number of entities affected.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
