using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
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
        public IEnumerable<Season> GetSeasons()
        {
            return _dbContext.Seasons.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Season"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Season}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Season>> GetSeasonsAsync()
        {
            return await _dbContext.Seasons.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        public Season? GetSeason(int id)
        {
            if (_dbContext.Seasons is null)
            {
                return null;
            }

            return _dbContext.Seasons.Find(id);
        }

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store asynchronously by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        public async Task<Season?> GetSeasonAsync(int id)
        {
            if (_dbContext.Seasons is null)
            {
                return null;
            }

            return await _dbContext.Seasons.FindAsync(id);
        }

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store by ID.
        /// </summary>
        /// <param name="year">The year of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        public Season? GetSeasonByYear(int year)
        {
            if (_dbContext.Seasons is null)
            {
                return null;
            }

            return _dbContext.Seasons.FirstOrDefault(s => s.Year == year);
        }

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store asynchronously by ID.
        /// </summary>
        /// <param name="year">The year of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        public async Task<Season?> GetSeasonByYearAsync(int year)
        {
            if (_dbContext.Seasons is null)
            {
                return null;
            }

            return await _dbContext.Seasons.FirstOrDefaultAsync(s => s.Year == year);
        }

        /// <summary>
        /// Adds a <see cref="Season"/> entity to the data store.
        /// </summary>
        /// <param name="season">The <see cref="Season"/> entity to add.</param>
        /// <returns>The added <see cref="Season"/> entity.</returns>
        public async Task<Season> AddAsync(Season season)
        {
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
            if (_dbContext.Seasons is null)
            {
                return season;
            }

            var entity = _dbContext.Seasons.Attach(season);
            entity.State = EntityState.Modified;

            return season;
        }

        /// <summary>
        /// Deletes a <see cref="Season"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Season"/> entity to delete.</param>
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

            _dbContext.Seasons.Remove(season);

            return season;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Season"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Season"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> SeasonExists(int id)
        {
            return await _dbContext.Seasons.AnyAsync(s => s.ID == id);
        }
    }
}
