using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamSeasonScheduleProfileRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class TeamSeasonScheduleRepository(ProFootballDbContext dbContext) : ITeamSeasonScheduleRepository
    {
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
        public IEnumerable<TeamSeasonOpponentProfile> GetTeamSeasonScheduleProfile(int teamId, int seasonYear)
        {
            return ExecuteGetTeamSeasonScheduleProfile(teamId, seasonYear);
        }

        /// <summary>
        /// Gets a single team season schedule profile (<see cref="IEnumerable{OpponentProfile}"/>) asynchronously from
        /// the data store by team name and season year.
        /// </summary>
        /// <param name="teamId">
        /// The team name of the <see cref="TeamSeasonScheduleProfile"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="TeamSeasonScheduleProfile"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{OpponentProfile}"/> collection.</returns>
        public async Task<IEnumerable<TeamSeasonOpponentProfile>> GetTeamSeasonScheduleProfileAsync(
            int teamId, int seasonYear
        )
        {
            return await ExecuteGetTeamSeasonScheduleProfileAsync(teamId, seasonYear);
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
        public TeamSeasonScheduleTotals GetTeamSeasonScheduleTotals(int teamId, int seasonYear)
        {
            return ExecuteGetTeamSeasonScheduleTotals(teamId, seasonYear);
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
        public async Task<TeamSeasonScheduleTotals> GetTeamSeasonScheduleTotalsAsync(int teamId, int seasonYear)
        {
            return await ExecuteGetTeamSeasonScheduleTotalsAsync(teamId, seasonYear);
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
        public TeamSeasonScheduleAverages GetTeamSeasonScheduleAverages(int teamId, int seasonYear)
        {
            return ExecuteGetTeamSeasonScheduleAverages(teamId, seasonYear);
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
        public async Task<TeamSeasonScheduleAverages> GetTeamSeasonScheduleAveragesAsync(int teamId, int seasonYear)
        {
            return await ExecuteGetTeamSeasonScheduleAveragesAsync(teamId, seasonYear);
        }

        protected virtual IEnumerable<TeamSeasonOpponentProfile> ExecuteGetTeamSeasonScheduleProfile(
            int teamId, int seasonYear
        )
        {
            return dbContext.TeamSeasonScheduleProfile
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleProfile @team_id = {teamId}, @season_year = {seasonYear}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<TeamSeasonOpponentProfile>> ExecuteGetTeamSeasonScheduleProfileAsync(
            int teamId, int seasonYear
        )
        {
            return await dbContext.TeamSeasonScheduleProfile
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleProfile @team_id = {teamId}, @season_year = {seasonYear}")
                .ToListAsync();
        }

        protected virtual TeamSeasonScheduleTotals ExecuteGetTeamSeasonScheduleTotals(int teamId, int seasonYear)
        {
            return dbContext.TeamSeasonScheduleTotals
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleTotals @team_id = {teamId}, @season_year = {seasonYear}")
                .ToList()
                .FirstOrDefault();
        }

        protected virtual async Task<TeamSeasonScheduleTotals> ExecuteGetTeamSeasonScheduleTotalsAsync(
            int teamId, int seasonYear
        )
        {
            return (await dbContext.TeamSeasonScheduleTotals
                .FromSqlInterpolated(
                    $"sp_GetTeamSeasonScheduleTotals @team_id = {teamId}, @season_year = {seasonYear}")
                .ToListAsync())
                .FirstOrDefault();
        }

        protected virtual TeamSeasonScheduleAverages ExecuteGetTeamSeasonScheduleAverages(int teamId, int seasonYear)
        {
            return dbContext.TeamSeasonScheduleAverages
                .FromSqlInterpolated(
                    $"EXEC sp_GetTeamSeasonScheduleAverages @team_id = {teamId}, @season_year = {seasonYear}")
                .ToList()
                .FirstOrDefault();
        }

        protected virtual async Task<TeamSeasonScheduleAverages> ExecuteGetTeamSeasonScheduleAveragesAsync(
            int teamId, int seasonYear
        )
        {
            return (await dbContext.TeamSeasonScheduleAverages
                .FromSqlInterpolated(
                    $"sp_GetTeamSeasonScheduleAverages @team_id = {teamId}, @season_year = {seasonYear}")
                .ToListAsync())
                .FirstOrDefault();
        }
    }
}
