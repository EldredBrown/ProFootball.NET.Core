using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="TeamSeason"/> data store.
    /// </summary>
    public interface ITeamSeasonRepository
    {
        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        IEnumerable<TeamSeason> GetTeamSeasons();

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        Task<IEnumerable<TeamSeason>> GetTeamSeasonsAsync();

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities from the data store for the specified season year.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="TeamSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        IEnumerable<TeamSeason> GetTeamSeasonsBySeason(int seasonYear);

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities from the data store asynchronously for the specified season year.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="TeamSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        Task<IEnumerable<TeamSeason>> GetTeamSeasonsBySeasonAsync(int seasonYear);

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        Task<TeamSeason?> GetTeamSeasonAsync(int id);

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store by team ID and season year.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <param name="seasonYear">The season year of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        TeamSeason? GetTeamSeasonByTeamAndSeason(int teamId, int seasonYear);

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store asynchronously by team ID and season
        /// year.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <param name="seasonYear">The season year of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        Task<TeamSeason?> GetTeamSeasonByTeamAndSeasonAsync(int teamId, int seasonYear);

        /// <summary>
        /// Adds a <see cref="TeamSeason"/> entity to the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity to add.</param>
        /// <returns>The added <see cref="TeamSeason"/> entity.</returns>
        Task<TeamSeason> AddAsync(TeamSeason teamSeason);

        /// <summary>
        /// Updates a <see cref="TeamSeason"/> entity in the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> to update.</param>
        /// <returns>The updated <see cref="TeamSeason"/> entity.</returns>
        TeamSeason Update(TeamSeason teamSeason);

        /// <summary>
        /// Deletes a <see cref="TeamSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="TeamSeason"/> entity.</returns>
        Task<TeamSeason?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="TeamSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> TeamSeasonExistsAsync(int id);
    }
}
