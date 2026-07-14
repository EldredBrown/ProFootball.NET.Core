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
    /// Provides CRUD access to an external <see cref="Association"/> data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AssociationRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class AssociationRepository(ProFootballDbContext dbContext) : IAssociationRepository
    {
        /// <summary>
        /// Gets all <see cref="Association"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Association}"/> of all fetched entities.</returns>
        public IEnumerable<Association>? GetAssociations()
        {
            return GetAssociationsDbSetWithNavigationProperties()?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Association"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Association}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Association>?> GetAssociationsAsync()
        {
            var associations = GetAssociationsDbSetWithNavigationProperties();
            return associations is null ? null : await associations.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        public Association? GetAssociation(int id)
        {
            return GetAssociations()?.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        public async Task<Association?> GetAssociationAsync(int id)
        {
            return (await GetAssociationsAsync())?.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store by shortName.
        /// </summary>
        /// <param name="shortName">The ShortName of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        public Association? GetAssociationByShortName(string shortName)
        {
            return GetAssociations()?.FirstOrDefault(a => a.ShortName == shortName);
        }

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store by shortName.
        /// </summary>
        /// <param name="shortName">The ShortName of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        public async Task<Association?> GetAssociationByShortNameAsync(string shortName)
        {
            return (await GetAssociationsAsync())?.FirstOrDefault(a => a.ShortName == shortName);
        }

        /// <summary>
        /// Adds a <see cref="Association"/> entity to the data store.
        /// </summary>
        /// <param name="association">The <see cref="Association"/> entity to add.</param>
        /// <returns>The added <see cref="Association"/> entity.</returns>
        public Association Add(Association association)
        {
            ArgumentNullException.ThrowIfNull(association);

            if (dbContext.Associations is null)
            {
                return association;
            }

            dbContext.Add(association);
            return association;
        }

        /// <summary>
        /// Adds a <see cref="Association"/> entity to the data store.
        /// </summary>
        /// <param name="association">The <see cref="Association"/> entity to add.</param>
        /// <returns>The added <see cref="Association"/> entity.</returns>
        public async Task<Association> AddAsync(Association association)
        {
            ArgumentNullException.ThrowIfNull(association);

            if (dbContext.Associations is null)
            {
                return association;
            }

            await dbContext.AddAsync(association);
            return association;
        }

        /// <summary>
        /// Updates a <see cref="Association"/> entity in the data store.
        /// </summary>
        /// <param name="association">The <see cref="Association"/> entity to update.</param>
        /// <returns>The updated <see cref="Association"/> entity.</returns>
        public Association Update(Association association)
        {
            ArgumentNullException.ThrowIfNull(association);

            if (dbContext.Associations is null)
            {
                return association;
            }

            dbContext.Update(association);
            return association;
        }

        /// <summary>
        /// Deletes a <see cref="Association"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Association"/> entity.</returns>
        public Association? Delete(int id)
        {
            if (dbContext.Associations is null)
            {
                return null;
            }

            var association = GetAssociation(id);
            if (association is null)
            {
                return null;
            }

            dbContext.Remove(association);
            return association;
        }

        /// <summary>
        /// Deletes a <see cref="Association"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Association"/> entity.</returns>
        public async Task<Association?> DeleteAsync(int id)
        {
            if (dbContext.Associations is null)
            {
                return null;
            }

            var association = await GetAssociationAsync(id);
            if (association is null)
            {
                return null;
            }

            dbContext.Remove(association);
            return association;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Association"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool AssociationExists(int id)
        {
            return GetAssociations()?.Any(a => a.Id == id) ?? false;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Association"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> AssociationExistsAsync(int id)
        {
            return (await GetAssociationsAsync())?.Any(a => a.Id == id) ?? false;
        }

        private IIncludableQueryable<Association, Season?>? GetAssociationsDbSetWithNavigationProperties()
        {
            // The use of AsNoTracking() is needed here because the Association table is self-referencing.
            return dbContext.Associations?
                .AsNoTracking()
                .Include(a => a.ParentIdNavigation)
                .Include(a => a.FirstSeasonYearNavigation)
                .Include(a => a.LastSeasonYearNavigation);
        }
    }
}
