using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Provides CRUD access to an external <see cref="LeagueSeason"/> data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="LeagueSeasonRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class LeagueSeasonRepository(ProFootballDbContext dbContext) : ILeagueSeasonRepository
    {
        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        public IEnumerable<LeagueSeason>? GetLeagueSeasons()
        {
            return GetLeagueSeasonsDbSetWithNavigationProperties()?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<LeagueSeason>?> GetLeagueSeasonsAsync()
        {
            var leagueSeasons = GetLeagueSeasonsDbSetWithNavigationProperties();
            return leagueSeasons is null ? null : await leagueSeasons.ToListAsync();
        }

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities from the data store for the specified league.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        public IEnumerable<LeagueSeason>? GetLeagueSeasonsByLeague(int leagueId)
        {
            return GetLeagueSeasons()?.Where(g => g.LeagueId == leagueId).ToList();
        }

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities from the data store for the specified league.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<LeagueSeason>?> GetLeagueSeasonsByLeagueAsync(int leagueId)
        {
            var leagueSeasons = await GetLeagueSeasonsAsync();
            return leagueSeasons is null ? null : leagueSeasons.Where(g => g.LeagueId == leagueId).ToList();
        }

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities in the data store.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch leagueSeasons.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        public IEnumerable<LeagueSeason>? GetLeagueSeasonsBySeason(int seasonId)
        {
            return GetLeagueSeasons()?.Where(g => g.SeasonId == seasonId).ToList();
        }

        /// <summary>
        /// Gets all <see cref="LeagueSeason"/> entities in the data store.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch leagueSeasons.</param>
        /// <returns>An <see cref="IEnumerable{LeagueSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<LeagueSeason>?> GetLeagueSeasonsBySeasonAsync(int seasonId)
        {
            var leagueSeasons = await GetLeagueSeasonsAsync();
            return leagueSeasons is null ? null : leagueSeasons.Where(g => g.SeasonId == seasonId).ToList();
        }

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        public LeagueSeason? GetLeagueSeason(int id)
        {
            return GetLeagueSeasons()?.FirstOrDefault(g => g.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        public async Task<LeagueSeason?> GetLeagueSeasonAsync(int id)
        {
            return (await GetLeagueSeasonsAsync())?.FirstOrDefault(g => g.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store by league ID and season year.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <param name="seasonId">The season year of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        public LeagueSeason? GetLeagueSeasonByLeagueAndSeason(int leagueId, int seasonId)
        {
            return GetLeagueSeasons()?.FirstOrDefault(g => g.LeagueId == leagueId && g.SeasonId == seasonId);
        }

        /// <summary>
        /// Gets a single <see cref="LeagueSeason"/> entity from the data store asynchronously by league ID and season
        /// year.
        /// </summary>
        /// <param name="leagueId">The league ID of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <param name="seasonId">The season year of the <see cref="LeagueSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeason"/> entity.</returns>
        public async Task<LeagueSeason?> GetLeagueSeasonByLeagueAndSeasonAsync(int leagueId, int seasonId)
        {
            return (await GetLeagueSeasonsAsync())?.FirstOrDefault(g => g.LeagueId == leagueId && g.SeasonId == seasonId);
        }

        /// <summary>
        /// Adds a <see cref="LeagueSeason"/> entity to the data store.
        /// </summary>
        /// <param name="leagueSeason">The <see cref="LeagueSeason"/> entity to add.</param>
        /// <returns>The added <see cref="LeagueSeason"/> entity.</returns>
        public LeagueSeason Add(LeagueSeason leagueSeason)
        {
            ArgumentNullException.ThrowIfNull(leagueSeason);

            if (dbContext.LeagueSeasons is null)
            {
                return leagueSeason;
            }

            dbContext.Add(leagueSeason);
            return leagueSeason;
        }

        /// <summary>
        /// Adds a <see cref="LeagueSeason"/> entity to the data store.
        /// </summary>
        /// <param name="leagueSeason">The <see cref="LeagueSeason"/> entity to add.</param>
        /// <returns>The added <see cref="LeagueSeason"/> entity.</returns>
        public async Task<LeagueSeason> AddAsync(LeagueSeason leagueSeason)
        {
            ArgumentNullException.ThrowIfNull(leagueSeason);

            if (dbContext.LeagueSeasons is null)
            {
                return leagueSeason;
            }

            await dbContext.AddAsync(leagueSeason);
            return leagueSeason;
        }

        /// <summary>
        /// Updates a <see cref="LeagueSeason"/> entity in the data store.
        /// </summary>
        /// <param name="leagueSeason">The <see cref="LeagueSeason"/> entity to update.</param>
        /// <returns>The updated <see cref="LeagueSeason"/> entity.</returns>
        public LeagueSeason? Update(LeagueSeason? leagueSeason)
        {
            ArgumentNullException.ThrowIfNull(leagueSeason);

            if (dbContext.LeagueSeasons is null)
            {
                return leagueSeason;
            }

            dbContext.Update(leagueSeason);
            return leagueSeason;
        }

        /// <summary>
        /// Deletes a <see cref="LeagueSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="LeagueSeason"/> entity.</returns>
        public LeagueSeason? Delete(int id)
        {
            if (dbContext.LeagueSeasons is null)
            {
                return null;
            }

            var leagueSeason = GetLeagueSeason(id);
            if (leagueSeason is null)
            {
                return null;
            }

            dbContext.Remove(leagueSeason);
            return leagueSeason;
        }

        /// <summary>
        /// Deletes a <see cref="LeagueSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="LeagueSeason"/> entity.</returns>
        public async Task<LeagueSeason?> DeleteAsync(int id)
        {
            if (dbContext.LeagueSeasons is null)
            {
                return null;
            }

            var leagueSeason = await GetLeagueSeasonAsync(id);
            if (leagueSeason is null)
            {
                return null;
            }

            dbContext.Remove(leagueSeason);
            return leagueSeason;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="LeagueSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool LeagueSeasonExists(int id)
        {
            return GetLeagueSeasons()?.Any(g => g.Id == id) ?? false;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="LeagueSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="LeagueSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> LeagueSeasonExistsAsync(int id)
        {
            return (await GetLeagueSeasonsAsync())?.Any(g => g.Id == id) ?? false;
        }

        private IIncludableQueryable<LeagueSeason, Season?>? GetLeagueSeasonsDbSetWithNavigationProperties()
        {
            return dbContext.LeagueSeasons?
                .Include(ts => ts.LeagueIdNavigation)
                .Include(ts => ts.SeasonIdNavigation);
        }
    }
}
