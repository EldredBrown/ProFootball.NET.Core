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
    /// Provides CRUD access to an external <see cref="Conference"/> data store.
    /// </summary>
    public class ConferenceRepository : IConferenceRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConferenceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public ConferenceRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Conference"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Conference}"/> of all fetched entities.</returns>
        public IEnumerable<Conference>? GetConferences()
        {
            return GetConferencesDbSetWithNavigationProperties()?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Conference"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Conference}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Conference>?> GetConferencesAsync()
        {
            var conferences = GetConferencesDbSetWithNavigationProperties();
            return conferences is null ? null : await conferences.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Conference"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Conference"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Conference"/> entity.</returns>
        public Conference? GetConference(int id)
        {
            return GetConferences()?.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Conference"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Conference"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Conference"/> entity.</returns>
        public async Task<Conference?> GetConferenceAsync(int id)
        {
            return (await GetConferencesAsync())?.FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Conference"/> entity from the data store by shortName.
        /// </summary>
        /// <param name="shortName">The ShortName of the <see cref="Conference"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Conference"/> entity.</returns>
        public Conference? GetConferenceByShortName(string shortName)
        {
            return GetConferences()?.FirstOrDefault(c => c.ShortName == shortName);
        }

        /// <summary>
        /// Gets a single <see cref="Conference"/> entity from the data store by shortName.
        /// </summary>
        /// <param name="shortName">The ShortName of the <see cref="Conference"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Conference"/> entity.</returns>
        public async Task<Conference?> GetConferenceByShortNameAsync(string shortName)
        {
            return (await GetConferencesAsync())?.FirstOrDefault(c => c.ShortName == shortName);
        }

        /// <summary>
        /// Adds a <see cref="Conference"/> entity to the data store.
        /// </summary>
        /// <param name="conference">The <see cref="Conference"/> entity to add.</param>
        /// <returns>The added <see cref="Conference"/> entity.</returns>
        public Conference Add(Conference conference)
        {
            ArgumentNullException.ThrowIfNull(conference);

            if (_dbContext.Conferences is null)
            {
                return conference;
            }

            _dbContext.Add(conference);
            return conference;
        }

        /// <summary>
        /// Adds a <see cref="Conference"/> entity to the data store.
        /// </summary>
        /// <param name="conference">The <see cref="Conference"/> entity to add.</param>
        /// <returns>The added <see cref="Conference"/> entity.</returns>
        public async Task<Conference> AddAsync(Conference conference)
        {
            ArgumentNullException.ThrowIfNull(conference);

            if (_dbContext.Conferences is null)
            {
                return conference;
            }

            await _dbContext.AddAsync(conference);
            return conference;
        }

        /// <summary>
        /// Updates a <see cref="Conference"/> entity in the data store.
        /// </summary>
        /// <param name="conference">The <see cref="Conference"/> entity to update.</param>
        /// <returns>The updated <see cref="Conference"/> entity.</returns>
        public Conference Update(Conference conference)
        {
            ArgumentNullException.ThrowIfNull(conference);

            if (_dbContext.Conferences is null)
            {
                return conference;
            }

            _dbContext.Update(conference);
            return conference;
        }

        /// <summary>
        /// Deletes a <see cref="Conference"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Conference"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Conference"/> entity.</returns>
        public Conference? Delete(int id)
        {
            if (_dbContext.Conferences is null)
            {
                return null;
            }

            var conference = GetConference(id);
            if (conference is null)
            {
                return null;
            }

            _dbContext.Remove(conference);
            return conference;
        }

        /// <summary>
        /// Deletes a <see cref="Conference"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Conference"/> entity to delete.</param>
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

            _dbContext.Remove(conference);
            return conference;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Conference"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Conference"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool ConferenceExists(int id)
        {
            return GetConferences()?.Any(c => c.Id == id) ?? false;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Conference"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Conference"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ConferenceExistsAsync(int id)
        {
            return (await GetConferencesAsync())?.Any(c => c.Id == id) ?? false;
        }

        private IIncludableQueryable<Conference, Season?>? GetConferencesDbSetWithNavigationProperties()
        {
            return _dbContext.Conferences?
                .Include(ts => ts.LeagueIdNavigation)
                .Include(ts => ts.FirstSeasonIdNavigation)
                .Include(ts => ts.LastSeasonIdNavigation);
        }
    }
}
