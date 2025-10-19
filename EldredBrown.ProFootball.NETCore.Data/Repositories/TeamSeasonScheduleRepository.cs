using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
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
            return _dbContext.TeamSeasonScheduleProfile.FromSqlInterpolated(
                $"sp_GetTeamSeasonScheduleProfile {teamName}, {seasonYear}").ToList();
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
            return await _dbContext.TeamSeasonScheduleProfile.FromSqlInterpolated(
                $"sp_GetTeamSeasonScheduleProfile {teamName}, {seasonYear}").ToListAsync();
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
            return _dbContext.TeamSeasonScheduleTotals.FromSqlInterpolated(
                $"sp_GetTeamSeasonScheduleTotals {teamName}, {seasonYear}").ToList().FirstOrDefault();
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
            return (await _dbContext.TeamSeasonScheduleTotals.FromSqlInterpolated(
                $"sp_GetTeamSeasonScheduleTotals {teamName}, {seasonYear}").ToListAsync()).FirstOrDefault();
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
            return _dbContext.TeamSeasonScheduleAverages.FromSqlInterpolated(
                $"sp_GetTeamSeasonScheduleAverages {teamName}, {seasonYear}").ToList().FirstOrDefault();
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
            return (await _dbContext.TeamSeasonScheduleAverages.FromSqlInterpolated(
                $"sp_GetTeamSeasonScheduleAverages {teamName}, {seasonYear}").ToListAsync()).FirstOrDefault();
        }
    }
}
