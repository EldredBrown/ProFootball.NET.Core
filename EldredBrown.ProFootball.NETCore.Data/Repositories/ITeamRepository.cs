using System.Collections.Generic;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="Team"/> data store.
    /// </summary>
    public interface ITeamRepository
    {
        /// <summary>
        /// Gets all <see cref="Team"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Team}"/> of all fetched entities.</returns>
        Task<IEnumerable<Team>> GetTeamsAsync();

        /// <summary>
        /// Gets a single <see cref="Team"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Team"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Team"/> entity.</returns>
        Task<Team?> GetTeamAsync(int id);

        /// <summary>
        /// Adds a <see cref="Team"/> entity to the data store.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> entity to add.</param>
        /// <returns>The added <see cref="Team"/> entity.</returns>
        Task<Team> AddAsync(Team team);

        /// <summary>
        /// Updates a <see cref="Team"/> entity in the data store.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> to update.</param>
        /// <returns>The updated <see cref="Team"/> entity.</returns>
        Team Update(Team team);

        /// <summary>
        /// Deletes a <see cref="Team"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Team"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Team"/> entity.</returns>
        Task<Team?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Team"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Team"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> TeamExists(int id);
    }
}
