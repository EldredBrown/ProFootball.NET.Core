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
    /// Provides CRUD access to an external <see cref="TeamSeason"/> data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TeamSeasonRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class TeamSeasonRepository(ProFootballDbContext dbContext) : ITeamSeasonRepository
    {
        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public IEnumerable<TeamSeason>? GetTeamSeasons()
        {
            return GetTeamSeasonsDbSetWithNavigationProperties()?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<TeamSeason>?> GetTeamSeasonsAsync()
        {
            var teamSeasons = GetTeamSeasonsDbSetWithNavigationProperties();
            return teamSeasons is null ? null : await teamSeasons.ToListAsync();
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities from the data store for the specified team.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public IEnumerable<TeamSeason>? GetTeamSeasonsByTeam(int teamId)
        {
            return GetTeamSeasons()?.Where(ts => ts.TeamId == teamId).ToList();
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities from the data store for the specified team.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entities to fetch.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<TeamSeason>?> GetTeamSeasonsByTeamAsync(int teamId)
        {
            var teamSeasons = await GetTeamSeasonsAsync();
            return teamSeasons is null ? null : teamSeasons.Where(ts => ts.TeamId == teamId).ToList();
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch teamSeasons.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public IEnumerable<TeamSeason>? GetTeamSeasonsBySeason(int seasonYear)
        {
            return GetTeamSeasons()?.Where(ts => ts.SeasonYear == seasonYear).ToList();
        }

        /// <summary>
        /// Gets all <see cref="TeamSeason"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch teamSeasons.</param>
        /// <returns>An <see cref="IEnumerable{TeamSeason}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<TeamSeason>?> GetTeamSeasonsBySeasonAsync(int seasonYear)
        {
            var teamSeasons = await GetTeamSeasonsAsync();
            return teamSeasons is null ? null : teamSeasons.Where(ts => ts.SeasonYear == seasonYear).ToList();
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason? GetTeamSeason(int id)
        {
            return GetTeamSeasons()?.FirstOrDefault(ts => ts.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason?> GetTeamSeasonAsync(int id)
        {
            return (await GetTeamSeasonsAsync())?.FirstOrDefault(ts => ts.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store by team ID and season year.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <param name="seasonYear">The season year of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason? GetTeamSeasonByTeamAndSeason(int teamId, int seasonYear)
        {
            return GetTeamSeasons()?.FirstOrDefault(ts => ts.TeamId == teamId && ts.SeasonYear == seasonYear);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeason"/> entity from the data store asynchronously by team ID and season
        /// year.
        /// </summary>
        /// <param name="teamId">The team ID of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <param name="seasonYear">The season year of the <see cref="TeamSeason"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason?> GetTeamSeasonByTeamAndSeasonAsync(int teamId, int seasonYear)
        {
            return (await GetTeamSeasonsAsync())?.FirstOrDefault(ts => ts.TeamId == teamId && ts.SeasonYear == seasonYear);
        }

        /// <summary>
        /// Adds a <see cref="TeamSeason"/> entity to the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity to add.</param>
        /// <returns>The added <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason Add(TeamSeason teamSeason)
        {
            ArgumentNullException.ThrowIfNull(teamSeason);

            if (dbContext.TeamSeasons is null)
            {
                return teamSeason;
            }

            dbContext.Add(teamSeason);
            return teamSeason;
        }

        /// <summary>
        /// Adds a <see cref="TeamSeason"/> entity to the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity to add.</param>
        /// <returns>The added <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason> AddAsync(TeamSeason teamSeason)
        {
            ArgumentNullException.ThrowIfNull(teamSeason);

            if (dbContext.TeamSeasons is null)
            {
                return teamSeason;
            }

            await dbContext.AddAsync(teamSeason);
            return teamSeason;
        }

        /// <summary>
        /// Updates a <see cref="TeamSeason"/> entity in the data store.
        /// </summary>
        /// <param name="teamSeason">The <see cref="TeamSeason"/> entity to update.</param>
        /// <returns>The updated <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason? Update(TeamSeason? teamSeason)
        {
            ArgumentNullException.ThrowIfNull(teamSeason);

            if (dbContext.TeamSeasons is null)
            {
                return teamSeason;
            }

            dbContext.Update(teamSeason);
            return teamSeason;
        }

        /// <summary>
        /// Deletes a <see cref="TeamSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="TeamSeason"/> entity.</returns>
        public TeamSeason? Delete(int id)
        {
            if (dbContext.TeamSeasons is null)
            {
                return null;
            }

            var teamSeason = GetTeamSeason(id);
            if (teamSeason is null)
            {
                return null;
            }

            dbContext.Remove(teamSeason);
            return teamSeason;
        }

        /// <summary>
        /// Deletes a <see cref="TeamSeason"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="TeamSeason"/> entity to delete.</param>
        /// <returns>The deleted <see cref="TeamSeason"/> entity.</returns>
        public async Task<TeamSeason?> DeleteAsync(int id)
        {
            if (dbContext.TeamSeasons is null)
            {
                return null;
            }

            var teamSeason = await GetTeamSeasonAsync(id);
            if (teamSeason is null)
            {
                return null;
            }

            dbContext.Remove(teamSeason);
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
            return GetTeamSeasons()?.Any(ts => ts.Id == id) ?? false;
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
            return (await GetTeamSeasonsAsync())?.Any(ts => ts.Id == id) ?? false;
        }

        private IIncludableQueryable<TeamSeason, Association?>? GetTeamSeasonsDbSetWithNavigationProperties()
        {
            return dbContext.TeamSeasons?
                .Include(ts => ts.TeamIdNavigation)
                .Include(ts => ts.SeasonYearNavigation)
                .Include(ts => ts.LeagueIdNavigation)
                .Include(ts => ts.ConferenceIdNavigation)
                .Include(ts => ts.DivisionIdNavigation);
        }
    }
}
