using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="LeagueSeason"/> data store.
    /// </summary>
    public interface ILeagueSeasonRepository
    {
        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        IEnumerable<LeagueSeason>? GetLeagueSeasons();

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        Task<IEnumerable<LeagueSeason>?> GetLeagueSeasonsAsync();

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities from the data store for the specified league.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        IEnumerable<LeagueSeason>? GetLeagueSeasonsByLeague(int leagueId);

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities from the data store for the specified league.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        Task<IEnumerable<LeagueSeason>?> GetLeagueSeasonsByLeagueAsync(int leagueId);

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities from the data store for the specified season year.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="LeagueSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        IEnumerable<LeagueSeason>? GetLeagueSeasonsBySeason(int seasonYear);

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities from the data store asynchronously for the specified season year.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="LeagueSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        Task<IEnumerable<LeagueSeason>?> GetLeagueSeasonsBySeasonAsync(int seasonYear);

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        LeagueSeason? GetLeagueSeason(int id);

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        Task<LeagueSeason?> GetLeagueSeasonAsync(int id);

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store by league ID and season year.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <param name="seasonYear">The season year of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        LeagueSeason? GetLeagueSeasonByLeagueAndSeason(int leagueId, int seasonYear);

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store asynchronously by league ID and season
        /// year.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <param name="seasonYear">The season year of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        Task<LeagueSeason?> GetLeagueSeasonByLeagueAndSeasonAsync(int leagueId, int seasonYear);

        /// <summary>
        /// Adds a <see cref="LeagueSeason"/> entity to the data store.
        /// </summary>
        /// <param name="leagueSeason">The <see cref="LeagueSeason"/> entity to add.</param>
        /// <returns>The added <see cref="LeagueSeason"/> entity.</returns>
        LeagueSeason Add(LeagueSeason leagueSeason);

        /// <summary>
        /// Adds a <see cref="LeagueSeason"/> entity to the data store.
        /// </summary>
        /// <param name="leagueSeason">The <see cref="LeagueSeason"/> entity to add.</param>
        /// <returns>The added <see cref="LeagueSeason"/> entity.</returns>
        Task<LeagueSeason> AddAsync(LeagueSeason leagueSeason);

        /// <summary>
        /// Updates a <see cref="LeagueSeason"/> entity in the data store.
        /// </summary>
        /// <param name="leagueSeason">The <see cref="LeagueSeason"/> to update.</param>
        /// <returns>The updated <see cref="LeagueSeason"/> entity.</returns>
        LeagueSeason? Update(LeagueSeason? leagueSeason);

        /// <summary>
        /// Deletes a <see cref="LeagueSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="LeagueSeason"/> entity.</returns>
        LeagueSeason? Delete(int id);

        /// <summary>
        /// Deletes a <see cref="LeagueSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="LeagueSeason"/> entity.</returns>
        Task<LeagueSeason?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="LeagueSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        bool LeagueSeasonExists(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="LeagueSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> LeagueSeasonExistsAsync(int id);
    }
}
