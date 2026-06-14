using System;
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
    /// <remarks>
    /// Initializes a new instance of the <see cref="TeamRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class TeamRepository(ProFootballDbContext dbContext) : ITeamRepository
    {
        /// <summary>
        /// Gets all <see cref="Team"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Team}"/> of all fetched entities.</returns>
        public IEnumerable<Team>? GetTeams()
        {
            return dbContext.Teams?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Team"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Team}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Team>?> GetTeamsAsync()
        {
            var teams = dbContext.Teams;
            return teams is null ? null : await teams.ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Team"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Team"/> entity.</returns>
        public Team? GetTeam(int id)
        {
            return GetTeams()?.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Team"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Team"/> entity.</returns>
        public async Task<Team?> GetTeamAsync(int id)
        {
            return (await GetTeamsAsync())?.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Team"/> entity from the data store by name.
        /// </summary>
        /// <param name="name">The Name of the <see cref="Team"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Team"/> entity.</returns>
        public Team? GetTeamByName(string name)
        {
            return GetTeams()?.FirstOrDefault(t => t.Name == name);
        }

        /// <summary>
        /// Gets a single <see cref="Team"/> entity from the data store by name.
        /// </summary>
        /// <param name="name">The Name of the <see cref="Team"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Team"/> entity.</returns>
        public async Task<Team?> GetTeamByNameAsync(string name)
        {
            return (await GetTeamsAsync())?.FirstOrDefault(t => t.Name == name);
        }

        /// <summary>
        /// Adds a <see cref="Team"/> entity to the data store.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> entity to add.</param>
        /// <returns>The added <see cref="Team"/> entity.</returns>
        public Team Add(Team team)
        {
            ArgumentNullException.ThrowIfNull(team);

            if (dbContext.Teams is null)
            {
                return team;
            }

            dbContext.Add(team);
            return team;
        }

        /// <summary>
        /// Adds a <see cref="Team"/> entity to the data store.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> entity to add.</param>
        /// <returns>The added <see cref="Team"/> entity.</returns>
        public async Task<Team> AddAsync(Team team)
        {
            ArgumentNullException.ThrowIfNull(team);

            if (dbContext.Teams is null)
            {
                return team;
            }

            await dbContext.AddAsync(team);
            return team;
        }

        /// <summary>
        /// Updates a <see cref="Team"/> entity in the data store.
        /// </summary>
        /// <param name="team">The <see cref="Team"/> entity to update.</param>
        /// <returns>The updated <see cref="Team"/> entity.</returns>
        public Team Update(Team team)
        {
            ArgumentNullException.ThrowIfNull(team);

            if (dbContext.Teams is null)
            {
                return team;
            }

            dbContext.Update(team);
            return team;
        }

        /// <summary>
        /// Deletes a <see cref="Team"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Team"/> entity.</returns>
        public Team? Delete(int id)
        {
            if (dbContext.Teams is null)
            {
                return null;
            }

            var team = GetTeam(id);
            if (team is null)
            {
                return null;
            }

            dbContext.Remove(team);
            return team;
        }

        /// <summary>
        /// Deletes a <see cref="Team"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Team"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Team"/> entity.</returns>
        public async Task<Team?> DeleteAsync(int id)
        {
            if (dbContext.Teams is null)
            {
                return null;
            }

            var team = await GetTeamAsync(id);
            if (team is null)
            {
                return null;
            }

            dbContext.Remove(team);
            return team;
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
            return GetTeams()?.Any(t => t.Id == id) ?? false;
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
            return (await GetTeamsAsync())?.Any(t => t.Id == id) ?? false;
        }
    }
}
