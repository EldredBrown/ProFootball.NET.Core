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
    /// Provides CRUD access to an external <see cref="League"/> data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="LeagueRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class LeagueRepository(ProFootballDbContext dbContext) : ILeagueRepository
    {
        /// <summary>
        /// Gets all <see cref="League"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{League}"/> of all fetched entities.</returns>
        public IEnumerable<League>? GetLeagues()
        {
            return GetLeaguesDbSetWithNavigationProperties()?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="League"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{League}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<League>?> GetLeaguesAsync()
        {
            var leagues = GetLeaguesDbSetWithNavigationProperties();
            return leagues is null ? null : await leagues.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        public League? GetLeague(int id)
        {
            return GetLeagues()?.FirstOrDefault(l => l.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        public async Task<League?> GetLeagueAsync(int id)
        {
            return (await GetLeaguesAsync())?.FirstOrDefault(l => l.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store by shortName.
        /// </summary>
        /// <param name="shortName">The ShortName of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        public League? GetLeagueByShortName(string shortName)
        {
            return GetLeagues()?.FirstOrDefault(l => l.ShortName == shortName);
        }

        /// <summary>
        /// Gets a single <see cref="League"/> entity from the data store by shortName.
        /// </summary>
        /// <param name="shortName">The ShortName of the <see cref="League"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="League"/> entity.</returns>
        public async Task<League?> GetLeagueByShortNameAsync(string shortName)
        {
            return (await GetLeaguesAsync())?.FirstOrDefault(l => l.ShortName == shortName);
        }

        /// <summary>
        /// Adds a <see cref="League"/> entity to the data store.
        /// </summary>
        /// <param name="league">The <see cref="League"/> entity to add.</param>
        /// <returns>The added <see cref="League"/> entity.</returns>
        public League Add(League league)
        {
            ArgumentNullException.ThrowIfNull(league);

            if (dbContext.Leagues is null)
            {
                return league;
            }

            dbContext.Add(league);
            return league;
        }

        /// <summary>
        /// Adds a <see cref="League"/> entity to the data store.
        /// </summary>
        /// <param name="league">The <see cref="League"/> entity to add.</param>
        /// <returns>The added <see cref="League"/> entity.</returns>
        public async Task<League> AddAsync(League league)
        {
            ArgumentNullException.ThrowIfNull(league);

            if (dbContext.Leagues is null)
            {
                return league;
            }

            await dbContext.AddAsync(league);
            return league;
        }

        /// <summary>
        /// Updates a <see cref="League"/> entity in the data store.
        /// </summary>
        /// <param name="league">The <see cref="League"/> entity to update.</param>
        /// <returns>The updated <see cref="League"/> entity.</returns>
        public League Update(League league)
        {
            ArgumentNullException.ThrowIfNull(league);

            if (dbContext.Leagues is null)
            {
                return league;
            }

            dbContext.Update(league);
            return league;
        }

        /// <summary>
        /// Deletes a <see cref="League"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to delete.</param>
        /// <returns>The deleted <see cref="League"/> entity.</returns>
        public League? Delete(int id)
        {
            if (dbContext.Leagues is null)
            {
                return null;
            }

            var league = GetLeague(id);
            if (league is null)
            {
                return null;
            }

            dbContext.Remove(league);
            return league;
        }

        /// <summary>
        /// Deletes a <see cref="League"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to delete.</param>
        /// <returns>The deleted <see cref="League"/> entity.</returns>
        public async Task<League?> DeleteAsync(int id)
        {
            if (dbContext.Leagues is null)
            {
                return null;
            }

            var league = await GetLeagueAsync(id);
            if (league is null)
            {
                return null;
            }

            dbContext.Remove(league);
            return league;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="League"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool LeagueExists(int id)
        {
            return GetLeagues()?.Any(l => l.Id == id) ?? false;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="League"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="League"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> LeagueExistsAsync(int id)
        {
            return (await GetLeaguesAsync())?.Any(l => l.Id == id) ?? false;
        }

        private IIncludableQueryable<League, Season?>? GetLeaguesDbSetWithNavigationProperties()
        {
            return dbContext.Leagues?
                .Include(ts => ts.FirstSeasonIdNavigation)
                .Include(ts => ts.LastSeasonIdNavigation);
        }
    }
}
