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
    /// Provides CRUD access to an external <see cref="Division"/> data store.
    /// </summary>
    public class DivisionRepository : IDivisionRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public DivisionRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Division"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Division}"/> of all fetched entities.</returns>
        public IEnumerable<Division>? GetDivisions()
        {
            return GetDivisionsDbSetWithNavigationProperties()?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Division"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Division}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Division>?> GetDivisionsAsync()
        {
            var divisions = GetDivisionsDbSetWithNavigationProperties();
            return divisions is null ? null : await divisions.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Division"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Division"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Division"/> entity.</returns>
        public Division? GetDivision(int id)
        {
            return GetDivisions()?.FirstOrDefault(d => d.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Division"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Division"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Division"/> entity.</returns>
        public async Task<Division?> GetDivisionAsync(int id)
        {
            return (await GetDivisionsAsync())?.FirstOrDefault(d => d.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Division"/> entity from the data store by name.
        /// </summary>
        /// <param name="name">The Name of the <see cref="Division"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Division"/> entity.</returns>
        public Division? GetDivisionByName(string name)
        {
            return GetDivisions()?.FirstOrDefault(d => d.Name == name);
        }

        /// <summary>
        /// Gets a single <see cref="Division"/> entity from the data store by name.
        /// </summary>
        /// <param name="name">The Name of the <see cref="Division"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Division"/> entity.</returns>
        public async Task<Division?> GetDivisionByNameAsync(string name)
        {
            return (await GetDivisionsAsync())?.FirstOrDefault(d => d.Name == name);
        }

        /// <summary>
        /// Adds a <see cref="Division"/> entity to the data store.
        /// </summary>
        /// <param name="division">The <see cref="Division"/> entity to add.</param>
        /// <returns>The added <see cref="Division"/> entity.</returns>
        public Division Add(Division division)
        {
            ArgumentNullException.ThrowIfNull(division);

            if (_dbContext.Divisions is null)
            {
                return division;
            }

            _dbContext.Add(division);
            return division;
        }

        /// <summary>
        /// Adds a <see cref="Division"/> entity to the data store.
        /// </summary>
        /// <param name="division">The <see cref="Division"/> entity to add.</param>
        /// <returns>The added <see cref="Division"/> entity.</returns>
        public async Task<Division> AddAsync(Division division)
        {
            ArgumentNullException.ThrowIfNull(division);

            if (_dbContext.Divisions is null)
            {
                return division;
            }

            await _dbContext.AddAsync(division);
            return division;
        }

        /// <summary>
        /// Updates a <see cref="Division"/> entity in the data store.
        /// </summary>
        /// <param name="division">The <see cref="Division"/> entity to update.</param>
        /// <returns>The updated <see cref="Division"/> entity.</returns>
        public Division Update(Division division)
        {
            ArgumentNullException.ThrowIfNull(division);

            if (_dbContext.Divisions is null)
            {
                return division;
            }

            _dbContext.Update(division);
            return division;
        }

        /// <summary>
        /// Deletes a <see cref="Division"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Division"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Division"/> entity.</returns>
        public Division? Delete(int id)
        {
            if (_dbContext.Divisions is null)
            {
                return null;
            }

            var division = GetDivision(id);
            if (division is null)
            {
                return null;
            }

            _dbContext.Remove(division);
            return division;
        }

        /// <summary>
        /// Deletes a <see cref="Division"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Division"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Division"/> entity.</returns>
        public async Task<Division?> DeleteAsync(int id)
        {
            if (_dbContext.Divisions is null)
            {
                return null;
            }

            var division = await GetDivisionAsync(id);
            if (division is null)
            {
                return null;
            }

            _dbContext.Remove(division);
            return division;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Division"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Division"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool DivisionExists(int id)
        {
            return GetDivisions()?.Any(d => d.Id == id) ?? false;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Division"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Division"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> DivisionExistsAsync(int id)
        {
            return (await GetDivisionsAsync())?.Any(d => d.Id == id) ?? false;
        }

        private IIncludableQueryable<Division, Season?>? GetDivisionsDbSetWithNavigationProperties()
        {
            return _dbContext.Divisions?
                .Include(ts => ts.LeagueIdNavigation)
                .Include(ts => ts.ConferenceIdNavigation)
                .Include(ts => ts.FirstSeasonIdNavigation)
                .Include(ts => ts.LastSeasonIdNavigation);
        }
    }
}
