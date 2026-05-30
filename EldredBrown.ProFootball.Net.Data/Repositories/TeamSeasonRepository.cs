using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Provides CRUD access to an external <see cref="TeamSeason"/> data store.
    /// </summary>
    public class TeamSeasonRepository : ITeamSeasonRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public TeamSeasonRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public IEnumerable<TeamSeason> GetTeamSeasons()
        {
            return _dbContext.TeamSeasons.ToList();
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason "/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<TeamSeason>> GetTeamSeasonsAsync()
        {
            return await _dbContext.TeamSeasons.ToListAsync();
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities from the data store for the specified season year.
        /// </summary>
        /// <param name="seasonId">The season year of the <see cref="TeamSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public IEnumerable<TeamSeason> GetTeamSeasonsBySeason(int seasonId)
        {
            return GetTeamSeasons().Where(ts => ts.SeasonId == seasonId);
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities from the data store asynchronously for the specified season year.
        /// </summary>
        /// <param name="seasonId">The season year of the <see cref="TeamSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<TeamSeason>> GetTeamSeasonsBySeasonAsync(int seasonId)
        {
            return (await GetTeamSeasonsAsync()).Where(ts => ts.SeasonId == seasonId);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason? GetTeamSeason(int id)
        {
            if (_dbContext.TeamSeasons is null)
            {
                return null;
            }

            return _dbContext.TeamSeasons.Find(id);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason?> GetTeamSeasonAsync(int id)
        {
            if (_dbContext.TeamSeasons is null)
            {
                return null;
            }

            return await _dbContext.TeamSeasons.FindAsync(id);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store by team ID and season year.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <param name="seasonId">The season year of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason? GetTeamSeasonByTeamAndSeason(int teamId, int seasonId)
        {
            return _dbContext.TeamSeasons
                .FirstOrDefault(ts => ts.TeamId == teamId && ts.SeasonId == seasonId);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store asynchronously by team ID and season year.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <param name="seasonId">The season year of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason?> GetTeamSeasonByTeamAndSeasonAsync(int teamId, int seasonId)
        {
            return await _dbContext.TeamSeasons
                .FirstOrDefaultAsync(ts => ts.TeamId == teamId && ts.SeasonId == seasonId);
        }

        /// <summary>
        /// Adds a <see cref="TeamSeason"/> entity to the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity to add.</param>
        /// <returns>The added <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason Add(TeamSeason teamSeason)
        {
            _dbContext.Add(teamSeason);

            return teamSeason;
        }

        /// <summary>
        /// Adds a <see cref="TeamSeason"/> entity to the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity to add.</param>
        /// <returns>The added <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason> AddAsync(TeamSeason teamSeason)
        {
            await _dbContext.AddAsync(teamSeason);

            return teamSeason;
        }

        /// <summary>
        /// Updates a <see cref="TeamSeason"/> entity in the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> to update.</param>
        /// <returns>The updated <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason Update(TeamSeason? teamSeason)
        {
            if (teamSeason is null)
            {
                return teamSeason;
            }

            if (_dbContext.TeamSeasons is null)
            {
                return teamSeason;
            }

            _dbContext.Update(teamSeason);

            return teamSeason;
        }

        /// <summary>
        /// Deletes a <see cref="TeamSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason? Delete(int id)
        {
            if (_dbContext.TeamSeasons is null)
            {
                return null;
            }

            var teamSeason = GetTeamSeason(id);
            if (teamSeason is null)
            {
                return null;
            }

            _dbContext.TeamSeasons.Remove(teamSeason as TeamSeason);

            return teamSeason;
        }

        /// <summary>
        /// Deletes a <see cref="TeamSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason?> DeleteAsync(int id)
        {
            if (_dbContext.TeamSeasons is null)
            {
                return null;
            }

            var teamSeason = await GetTeamSeasonAsync(id);
            if (teamSeason is null)
            {
                return null;
            }

            _dbContext.TeamSeasons.Remove(teamSeason as TeamSeason);

            return teamSeason;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="TeamSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool TeamSeasonExists(int id)
        {
            return _dbContext.TeamSeasons.Any(ts => ts.Id == id);
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="TeamSeason"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> TeamSeasonExistsAsync(int id)
        {
            return await _dbContext.TeamSeasons.AnyAsync(ts => ts.Id == id);
        }
    }
}
