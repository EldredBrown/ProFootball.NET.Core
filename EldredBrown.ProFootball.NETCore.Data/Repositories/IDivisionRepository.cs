using System.Collections.Generic;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="Division"/> data store.
    /// </summary>
    public interface IDivisionRepository
    {
        /// <summary>
        /// Gets all <see cref="Division"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Division}"/> of all fetched entities.</returns>
        Task<IEnumerable<Division>> GetDivisionsAsync();

        /// <summary>
        /// Gets a single <see cref="Division"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Division"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Division"/> entity.</returns>
        Task<Division?> GetDivisionAsync(int id);

        /// <summary>
        /// Adds a <see cref="Division"/> entity to the data store.
        /// </summary>
        /// <param name="division">The <see cref="Division"/> entity to add.</param>
        /// <returns>The added <see cref="Division"/> entity.</returns>
        Task<Division> AddAsync(Division division);

        /// <summary>
        /// Updates a <see cref="Division"/> entity in the data store.
        /// </summary>
        /// <param name="division">The <see cref="Division"/> to update.</param>
        /// <returns>The updated <see cref="Division"/> entity.</returns>
        Division Update(Division division);

        /// <summary>
        /// Deletes a <see cref="Division"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Division"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Division"/> entity.</returns>
        Task<Division?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Division"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Division"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DivisionExists(int id);
    }
}
