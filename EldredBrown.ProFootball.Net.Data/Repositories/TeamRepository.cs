using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
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
        public IEnumerable<Team> GetTeams()
        {
            return _dbContext.Teams;
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
        /// Gets a single <see cref="Team"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Team"/> entity.</returns>
        public Team? GetTeam(int id)
        {
            if (_dbContext.Teams is null)
            {
                return null;
            }

            return _dbContext.Teams.Find(id);
        }

        /// <summary>
        /// Gets a single <see cref="Team"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to fetch.</param>
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
        /// <param name="Team">The <see cref="Team"/> entity to add.</param>
        /// <returns>The added <see cref="Team"/> entity.</returns>
        public Team Add(Team Team)
        {
            _dbContext.Add(Team);

            return Team;
        }

        /// <summary>
        /// Adds a <see cref="Team"/> entity to the data store.
        /// </summary>
        /// <param name="Team">The <see cref="Team"/> entity to add.</param>
        /// <returns>The added <see cref="Team"/> entity.</returns>
        public async Task<Team> AddAsync(Team Team)
        {
            await _dbContext.AddAsync(Team);

            return Team;
        }

        /// <summary>
        /// Updates a <see cref="Team"/> entity in the data store.
        /// </summary>
        /// <param name="Team">The <see cref="Team"/> to update.</param>
        /// <returns>The updated <see cref="Team"/> entity.</returns>
        public Team Update(Team team)
        {
            if (_dbContext.Teams is null)
            {
                return team;
            }

            //var entity = _dbContext.Teams.Attach(team);
            //entity.State = EntityState.Modified;
            //_dbContext.Teams.Update(team);
            var existing = _dbContext.Teams.Find(team.Id);
            if (existing is null)
                return team;

            _dbContext.Entry(existing).CurrentValues.SetValues(team);

            return team;
        }

        /// <summary>
        /// Deletes a <see cref="Team"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Team"/> entity.</returns>
        public Team? Delete(int id)
        {
            if (_dbContext.Teams is null)
            {
                return null;
            }

            var Team = GetTeam(id);
            if (Team is null)
            {
                return null;
            }

            _dbContext.Teams.Remove(Team);

            return Team;
        }

        /// <summary>
        /// Deletes a <see cref="Team"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Team"/> entity.</returns>
        public async Task<Team?> DeleteAsync(int id)
        {
            if (_dbContext.Teams is null)
            {
                return null;
            }

            var Team = await GetTeamAsync(id);
            if (Team is null)
            {
                return null;
            }

            _dbContext.Teams.Remove(Team);

            return Team;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Team"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool TeamExists(int id)
        {
            return _dbContext.Teams.Any(t => t.Id == id);
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Team"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> TeamExistsAsync(int id)
        {
            return await _dbContext.Teams.AnyAsync(t => t.Id == id);
        }
    }
}
