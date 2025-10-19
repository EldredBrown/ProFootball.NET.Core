using System.Collections.Generic;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
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
        Task<IEnumerable<LeagueSeason>> GetLeagueSeasonsAsync();

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        Task<LeagueSeason?> GetLeagueSeasonAsync(int id);

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store by league name and season year.
        /// </summary>
        /// <param name="leagueName">The name of the league of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <param name="seasonYear">The year of the season of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        Task<LeagueSeason?> GetLeagueSeasonByLeagueAndSeasonAsync(string leagueName, int seasonYear);

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
        LeagueSeason Update(LeagueSeason leagueSeason);

        /// <summary>
        /// Deletes a <see cref="LeagueSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="LeagueSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="LeagueSeason"/> entity.</returns>
        Task<LeagueSeason?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="LeagueSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="LeagueSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> LeagueSeasonExists(int id);
    }
}
