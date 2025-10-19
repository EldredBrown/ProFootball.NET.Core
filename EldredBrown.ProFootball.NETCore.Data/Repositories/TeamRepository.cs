using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Provides CRUD access to an external <see cref="Team"/> data store.
    /// </summary>
    public class TeamRepository : ITeamRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the data store.</param>
        public TeamRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Team"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Team}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            return await _dbContext.Teams.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Team"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Team"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Team"/> entity.</returns>
        public async Task<Team?> GetTeamAsync(int id)
        {
            if (_dbContext.Teams is null)
            {
                return null;
            }

            return await _dbContext.Teams.FindAsync(id);
        }

        /// <summary>
        /// Adds a <see cref="Team"/> entity to the data store.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> entity to add.</param>
        /// <returns>The added <see cref="Team"/> entity.</returns>
        public async Task<Team> AddAsync(Team team)
        {
            await _dbContext.AddAsync(team);

            return team;
        }

        /// <summary>
        /// Updates a <see cref="Team"/> entity in the data store.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> to update.</param>
        /// <returns>The updated <see cref="Team"/> entity.</returns>
        public Team Update(Team team)
        {
            if (_dbContext.Teams is null)
            {
                return team;
            }

            var entity = _dbContext.Teams.Attach(team);
            entity.State = EntityState.Modified;

            return team;
        }

        /// <summary>
        /// Deletes a <see cref="Team"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Team"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Team"/> entity.</returns>
        public async Task<Team?> DeleteAsync(int id)
        {
            if (_dbContext.Teams is null)
            {
                return null;
            }

            var team = await GetTeamAsync(id);
            if (team is null)
            {
                return null;
            }

            _dbContext.Teams.Remove(team);

            return team;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Team"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Team"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> TeamExists(int id)
        {
            return await _dbContext.Teams.AnyAsync(t => t.ID == id);
        }
    }
}
