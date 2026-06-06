using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Provides CRUD access to an external <see cref="Season"/> data store.
    /// </summary>
    public class SeasonRepository : ISeasonRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public SeasonRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Season"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Season}"/> of all fetched entities.</returns>
        public IEnumerable<Season>? GetSeasons()
        {
            return _dbContext.Seasons?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Season"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Season}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Season>?> GetSeasonsAsync()
        {
            var seasons = _dbContext.Seasons;
            return seasons is null ? null : await seasons.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        public Season? GetSeason(int id)
        {
            return GetSeasons()?.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        public async Task<Season?> GetSeasonAsync(int id)
        {
            return (await GetSeasonsAsync())?.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Adds a <see cref="Season"/> entity to the data store.
        /// </summary>
        /// <param name="season">The <see cref="Season"/> entity to add.</param>
        /// <returns>The added <see cref="Season"/> entity.</returns>
        public Season Add(Season season)
        {
            ArgumentNullException.ThrowIfNull(season);

            if (_dbContext.Seasons is null)
            {
                return season;
            }

            _dbContext.Add(season);
            return season;
        }

        /// <summary>
        /// Adds a <see cref="Season"/> entity to the data store.
        /// </summary>
        /// <param name="season">The <see cref="Season"/> entity to add.</param>
        /// <returns>The added <see cref="Season"/> entity.</returns>
        public async Task<Season> AddAsync(Season season)
        {
            ArgumentNullException.ThrowIfNull(season);

            if (_dbContext.Seasons is null)
            {
                return season;
            }

            await _dbContext.AddAsync(season);
            return season;
        }

        /// <summary>
        /// Updates a <see cref="Season"/> entity in the data store.
        /// </summary>
        /// <param name="season">The <see cref="Season"/> entity to update.</param>
        /// <returns>The updated <see cref="Season"/> entity.</returns>
        public Season Update(Season season)
        {
            ArgumentNullException.ThrowIfNull(season);

            if (_dbContext.Seasons is null)
            {
                return season;
            }

            _dbContext.Update(season);
            return season;
        }

        /// <summary>
        /// Deletes a <see cref="Season"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Season"/> entity.</returns>
        public Season? Delete(int id)
        {
            if (_dbContext.Seasons is null)
            {
                return null;
            }

            var season = GetSeason(id);
            if (season is null)
            {
                return null;
            }

            _dbContext.Remove(season);
            return season;
        }

        /// <summary>
        /// Deletes a <see cref="Season"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Season"/> entity.</returns>
        public async Task<Season?> DeleteAsync(int id)
        {
            if (_dbContext.Seasons is null)
            {
                return null;
            }

            var season = await GetSeasonAsync(id);
            if (season is null)
            {
                return null;
            }

            _dbContext.Remove(season);
            return season;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Season"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool SeasonExists(int id)
        {
            return GetSeasons()?.Any(s => s.Id == id) ?? false;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Season"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> SeasonExistsAsync(int id)
        {
            return (await GetSeasonsAsync())?.Any(s => s.Id == id) ?? false;
        }
    }
}
