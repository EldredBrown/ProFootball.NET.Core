using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
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
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the data store.</param>
        public DivisionRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Division"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Division}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Division>> GetDivisionsAsync()
        {
            return await _dbContext.Divisions.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Division"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Division"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Division"/> entity.</returns>
        public async Task<Division?> GetDivisionAsync(int id)
        {
            if (_dbContext.Divisions is null)
            {
                return null;
            }

            return await _dbContext.Divisions.FindAsync(id);
        }

        /// <summary>
        /// Adds a <see cref="Division"/> entity to the data store.
        /// </summary>
        /// <param name="division">The <see cref="Division"/> entity to add.</param>
        /// <returns>The added <see cref="Division"/> entity.</returns>
        public async Task<Division> AddAsync(Division division)
        {
            await _dbContext.AddAsync(division);

            return division;
        }

        /// <summary>
        /// Updates a <see cref="Division"/> entity in the data store.
        /// </summary>
        /// <param name="division">The <see cref="Division"/> to update.</param>
        /// <returns>The updated <see cref="Division"/> entity.</returns>
        public Division Update(Division division)
        {
            if (_dbContext.Divisions is null)
            {
                return division;
            }

            var entity = _dbContext.Divisions.Attach(division);
            entity.State = EntityState.Modified;

            return division;
        }

        /// <summary>
        /// Deletes a <see cref="Division"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Division"/> entity to delete.</param>
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

            _dbContext.Divisions.Remove(division);

            return division;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Division"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Division"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> DivisionExists(int id)
        {
            return await _dbContext.Divisions.AnyAsync(l => l.ID == id);
        }
    }
}
