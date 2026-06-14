using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LeagueSeasonTotalsRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class LeagueSeasonTotalsRepository(ProFootballDbContext dbContext) : ILeagueSeasonTotalsRepository
    {
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
        public LeagueSeasonTotals? GetLeagueSeasonTotals(int leagueId, int seasonId)
        {
            return ExecuteGetLeagueSeasonTotals(leagueId, seasonId);
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
        public async Task<LeagueSeasonTotals?> GetLeagueSeasonTotalsAsync(int leagueId, int seasonId)
        {
            return await ExecuteGetLeagueSeasonTotalsAsync(leagueId, seasonId);
        }

        protected virtual LeagueSeasonTotals? ExecuteGetLeagueSeasonTotals(int leagueId, int seasonId)
        {
            return dbContext.LeagueSeasonTotals
                .FromSqlInterpolated(
                    $"EXEC sp_GetLeagueSeasonTotals @leagueId = {leagueId}, @seasonId = {seasonId}")
                .ToList()
                .FirstOrDefault();
        }

        protected virtual async Task<LeagueSeasonTotals?> ExecuteGetLeagueSeasonTotalsAsync(int leagueId, int seasonId)
        {
            return (await dbContext.LeagueSeasonTotals
                .FromSqlInterpolated(
                    $"EXEC sp_GetLeagueSeasonTotals @league_id = {leagueId}, @season_id = {seasonId}")
                .ToListAsync())
                .FirstOrDefault();
        }
    }
}
