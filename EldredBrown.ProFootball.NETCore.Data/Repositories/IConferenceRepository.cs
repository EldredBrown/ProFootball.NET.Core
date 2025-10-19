using System.Collections.Generic;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="Conference"/> data store.
    /// </summary>
    public interface IConferenceRepository
    {
        /// <summary>
        /// Gets all <see cref="Conference"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Conference}"/> of all fetched entities.</returns>
        Task<IEnumerable<Conference>> GetConferencesAsync();

        /// <summary>
        /// Gets a single <see cref="Conference"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Conference"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Conference"/> entity.</returns>
        Task<Conference?> GetConferenceAsync(int id);

        /// <summary>
        /// Adds a <see cref="Conference"/> entity to the data store.
        /// </summary>
        /// <param name="conference">The <see cref="Conference"/> entity to add.</param>
        /// <returns>The added <see cref="Conference"/> entity.</returns>
        Task<Conference> AddAsync(Conference conference);

        /// <summary>
        /// Updates a <see cref="Conference"/> entity in the data store.
        /// </summary>
        /// <param name="conference">The <see cref="Conference"/> to update.</param>
        /// <returns>The updated <see cref="Conference"/> entity.</returns>
        Conference Update(Conference conference);

        /// <summary>
        /// Deletes a <see cref="Conference"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Conference"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Conference"/> entity.</returns>
        Task<Conference?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Conference"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Conference"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ConferenceExists(int id);
    }
}
