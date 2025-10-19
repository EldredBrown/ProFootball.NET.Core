using System.Threading.Tasks;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a data store.
    /// </summary>
    public interface ISharedRepository
    {
        /// <summary>
        /// Saves changes made to the data store.
        /// </summary>
        /// <returns>The number of entities affected.</returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronously saves changes made to the data store.
        /// </summary>
        /// <returns>The number of entities affected.</returns>
        Task<int> SaveChangesAsync();
    }
}
