using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Provides CRUD access to an external <see cref="Conference"/> data store.
    /// </summary>
    public class ConferenceRepository : IConferenceRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the data store.</param>
        public ConferenceRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Conference"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Conference}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Conference>> GetConferencesAsync()
        {
            return await _dbContext.Conferences.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Conference"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Conference"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Conference"/> entity.</returns>
        public async Task<Conference?> GetConferenceAsync(int id)
        {
            if (_dbContext.Conferences is null)
            {
                return null;
            }

            return await _dbContext.Conferences.FindAsync(id);
        }

        /// <summary>
        /// Adds a <see cref="Conference"/> entity to the data store.
        /// </summary>
        /// <param name="conference">The <see cref="Conference"/> entity to add.</param>
        /// <returns>The added <see cref="Conference"/> entity.</returns>
        public async Task<Conference> AddAsync(Conference conference)
        {
            await _dbContext.AddAsync(conference);

            return conference;
        }

        /// <summary>
        /// Updates a <see cref="Conference"/> entity in the data store.
        /// </summary>
        /// <param name="conference">The <see cref="Conference"/> to update.</param>
        /// <returns>The updated <see cref="Conference"/> entity.</returns>
        public Conference Update(Conference conference)
        {
            if (_dbContext.Conferences is null)
            {
                return conference;
            }

            var entity = _dbContext.Conferences.Attach(conference);
            entity.State = EntityState.Modified;

            return conference;
        }

        /// <summary>
        /// Deletes a <see cref="Conference"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Conference"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Conference"/> entity.</returns>
        public async Task<Conference?> DeleteAsync(int id)
        {
            if (_dbContext.Conferences is null)
            {
                return null;
            }

            var conference = await GetConferenceAsync(id);
            if (conference is null)
            {
                return null;
            }

            _dbContext.Conferences.Remove(conference);

            return conference;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Conference"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Conference"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ConferenceExists(int id)
        {
            return await _dbContext.Conferences.AnyAsync(l => l.ID == id);
        }
    }
}
