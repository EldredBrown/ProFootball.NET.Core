using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="League"/> data store.
    /// </summary>
    public interface ILeagueRepository
    {
        /// <summary>
        /// Gets all <see cref="League"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{League}"/> of all fetched entities.</returns>
        IEnumerable<League> GetLeagues();

        /// <summary>
        /// Gets all <see cref="League"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{League}"/> of all fetched entities.</returns>
        Task<IEnumerable<League>> GetLeaguesAsync();

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        League? GetLeague(int id);

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        Task<League?> GetLeagueAsync(int id);

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store by Id.
        /// </summary>
        /// <param name="shortName">The short name of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        League? GetLeagueByShortName(string shortName);

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store by Id.
        /// </summary>
        /// <param name="shortName">The short name of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        Task<League?> GetLeagueByShortNameAsync(string shortName);

        /// <summary>
        /// Adds a <see cref="League"/> entity to the data store.
        /// </summary>
        /// <param name="league">The <see cref="League"/> entity to add.</param>
        /// <returns>The added <see cref="League"/> entity.</returns>
        League Add(League league);

        /// <summary>
        /// Adds a <see cref="League"/> entity to the data store.
        /// </summary>
        /// <param name="league">The <see cref="League"/> entity to add.</param>
        /// <returns>The added <see cref="League"/> entity.</returns>
        Task<League> AddAsync(League league);

        /// <summary>
        /// Updates a <see cref="League"/> entity in the data store.
        /// </summary>
        /// <param name="league">The <see cref="League"/> to update.</param>
        /// <returns>The updated <see cref="League"/> entity.</returns>
        League Update(League league);

        /// <summary>
        /// Deletes a <see cref="League"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to delete.</param>
        /// <returns>The deleted <see cref="League"/> entity.</returns>
        League? Delete(int id);

        /// <summary>
        /// Deletes a <see cref="League"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to delete.</param>
        /// <returns>The deleted <see cref="League"/> entity.</returns>
        Task<League?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="League"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        bool LeagueExists(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="League"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> LeagueExistsAsync(int id);
    }
}
