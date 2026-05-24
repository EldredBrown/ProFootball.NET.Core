using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    public class TeamSeasonScheduleRepository : ITeamSeasonScheduleRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonScheduleProfileRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public TeamSeasonScheduleRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets a single team season schedule profile (<see cref="IEnumerable{OpponentProfile}"/>) from the data store
        /// by team name and season year.
        /// </summary>
        /// <param name="teamName">
        /// The team name of the <see cref="TeamSeasonScheduleProfile"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="TeamSeasonScheduleProfile"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{OpponentProfile}"/> collection.</returns>
        public IEnumerable<TeamSeasonOpponentProfile> GetTeamSeasonScheduleProfile(string teamName, int seasonYear)
        {
            return ExecuteGetTeamSeasonScheduleProfile(teamName, seasonYear);
        }

        /// <summary>
        /// Gets a single team season schedule profile (<see cref="IEnumerable{OpponentProfile}"/>) asynchronously from
        /// the data store by team name and season year.
        /// </summary>
        /// <param name="teamName">
        /// The team name of the <see cref="TeamSeasonScheduleProfile"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="TeamSeasonScheduleProfile"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{OpponentProfile}"/> collection.</returns>
        public async Task<IEnumerable<TeamSeasonOpponentProfile>> GetTeamSeasonScheduleProfileAsync(string teamName,
            int seasonYear)
        {
            return await ExecuteGetTeamSeasonScheduleProfileAsync(teamName, seasonYear);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeasonScheduleTotals"/> entity from the data store by team name and season
        /// year.
        /// </summary>
        /// <param name="teamName">
        /// The team name of the <see cref="TeamSeasonScheduleTotals"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="TeamSeasonScheduleTotals"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="TeamSeasonScheduleTotals"/> entity.</returns>
        public TeamSeasonScheduleTotals GetTeamSeasonScheduleTotals(string teamName, int seasonYear)
        {
            return ExecuteGetTeamSeasonScheduleTotals(teamName, seasonYear);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeasonScheduleTotals"/> entity asynchronously from the data store by team name
        /// and season year.
        /// </summary>
        /// <param name="teamName">
        /// The team name of the <see cref="TeamSeasonScheduleTotals"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="TeamSeasonScheduleTotals"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="TeamSeasonScheduleTotals"/> entity.</returns>
        public async Task<TeamSeasonScheduleTotals> GetTeamSeasonScheduleTotalsAsync(string teamName,
            int seasonYear)
        {
            return await ExecuteGetTeamSeasonScheduleTotalsAsync(teamName, seasonYear);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeasonScheduleAverages"/> entity from the data store by team name and season
        /// year.
        /// </summary>
        /// <param name="teamName">
        /// The team name of the <see cref="TeamSeasonScheduleAverages"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="TeamSeasonScheduleAverages"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="TeamSeasonScheduleAverages"/> entity.</returns>
        public TeamSeasonScheduleAverages GetTeamSeasonScheduleAverages(string teamName, int seasonYear)
        {
            return ExecuteGetTeamSeasonScheduleAverages(teamName, seasonYear);
        }

        /// <summary>
        /// Gets a single <see cref="TeamSeasonScheduleAverages"/> entity asynchronously from the data store by team
        /// name and season year.
        /// </summary>
        /// <param name="teamName">
        /// The team name of the <see cref="TeamSeasonScheduleAverages"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="TeamSeasonScheduleAverages"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="TeamSeasonScheduleAverages"/> entity.</returns>
        public async Task<TeamSeasonScheduleAverages> GetTeamSeasonScheduleAveragesAsync(string teamName,
            int seasonYear)
        {
            return await ExecuteGetTeamSeasonScheduleAveragesAsync(teamName, seasonYear);
        }

        protected virtual IEnumerable<TeamSeasonOpponentProfile> ExecuteGetTeamSeasonScheduleProfile(
            string teamName, int seasonYear)
        {
            return _dbContext.TeamSeasonScheduleProfile
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleProfile @teamName = {teamName}, @seasonYear = {seasonYear}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<TeamSeasonOpponentProfile>> ExecuteGetTeamSeasonScheduleProfileAsync(
            string teamName, int seasonYear)
        {
            return await _dbContext.TeamSeasonScheduleProfile
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleProfile @teamName = {teamName}, @seasonYear = {seasonYear}")
                .ToListAsync();
        }

        protected virtual TeamSeasonScheduleTotals ExecuteGetTeamSeasonScheduleTotals(
            string teamName, int seasonYear)
        {
            return _dbContext.TeamSeasonScheduleTotals
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleTotals @teamName = {teamName}, @seasonYear = {seasonYear}")
                .ToList()
                .FirstOrDefault();
        }

        protected virtual async Task<TeamSeasonScheduleTotals> ExecuteGetTeamSeasonScheduleTotalsAsync(
            string teamName, int seasonYear)
        {
            return (await _dbContext.TeamSeasonScheduleTotals
                .FromSqlInterpolated(
                    $"sp_GetTeamSeasonScheduleTotals {teamName}, {seasonYear}")
                .ToListAsync())
                .FirstOrDefault();
        }

        protected virtual TeamSeasonScheduleAverages ExecuteGetTeamSeasonScheduleAverages(
            string teamName, int seasonYear)
        {
            return _dbContext.TeamSeasonScheduleAverages
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleAverages @teamName = {teamName}, @seasonYear = {seasonYear}")
                .ToList()
                .FirstOrDefault();
        }

        protected virtual async Task<TeamSeasonScheduleAverages> ExecuteGetTeamSeasonScheduleAveragesAsync(
            string teamName, int seasonYear)
        {
            return (await _dbContext.TeamSeasonScheduleAverages
                .FromSqlInterpolated(
                    $"sp_GetTeamSeasonScheduleAverages {teamName}, {seasonYear}")
                .ToListAsync())
                .FirstOrDefault();
        }
    }
}
