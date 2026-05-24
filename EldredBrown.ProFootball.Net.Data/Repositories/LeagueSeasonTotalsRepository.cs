using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    public class LeagueSeasonTotalsRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeagueSeasonTotalsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public LeagueSeasonTotalsRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets a single <see cref="LeagueSeasonTotals"/> entity from the data store by league name and season
        /// year.
        /// </summary>
        /// <param name="leagueName">
        /// The league name of the <see cref="LeagueSeasonTotals"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="LeagueSeasonTotals"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="LeagueSeasonTotals"/> entity.</returns>
        public LeagueSeasonTotals? GetLeagueSeasonTotals(string leagueName, int seasonYear)
        {
            return ExecuteGetLeagueSeasonTotals(leagueName, seasonYear);
        }

        /// <summary>
        /// Gets a single <see cref="LeagueSeasonTotals"/> entity asynchronously from the data store by league name
        /// and season year.
        /// </summary>
        /// <param name="leagueName">
        /// The league name of the <see cref="LeagueSeasonTotals"/> entity to fetch.
        /// </param>
        /// <param name="seasonYear">
        /// The season year of the <see cref="LeagueSeasonTotals"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="LeagueSeasonTotals"/> entity.</returns>
        public async Task<LeagueSeasonTotals?> GetLeagueSeasonTotalsAsync(string leagueName, int seasonYear)
        {
            return await ExecuteGetLeagueSeasonTotalsAsync(leagueName, seasonYear);
        }

        protected virtual LeagueSeasonTotals? ExecuteGetLeagueSeasonTotals(string leagueName, int seasonYear)
        {
            return _dbContext.LeagueSeasonTotals
                .FromSqlInterpolated(
                    $"EXEC sp_GetLeagueSeasonTotals @leagueName = {leagueName}, @seasonYear = {seasonYear}")
                .ToList()
                .FirstOrDefault();
        }

        protected virtual async Task<LeagueSeasonTotals?> ExecuteGetLeagueSeasonTotalsAsync(
            string leagueName, int seasonYear)
        {
            return (await _dbContext.LeagueSeasonTotals
                .FromSqlInterpolated(
                    $"EXEC sp_GetLeagueSeasonTotals @leagueName = {leagueName}, @seasonYear = {seasonYear}")
                .ToListAsync())
                .FirstOrDefault();
        }
    }
}
