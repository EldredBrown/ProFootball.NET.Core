using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Provides read access to the GetSeasonStandings stored procedure.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeasonStandingsRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class SeasonStandingsRepository(ProFootballDbContext dbContext) : ISeasonStandingsRepository
    {
        /// <summary>
        /// Gets all <see cref="StandingsTeamSeason"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="StandingsTeamSeason"/> entity to fetch.</param>
        /// <returns>An <see cref="IEnumerable{SeasonStanding}"/> of all fetched entities.</returns>
        public IEnumerable<StandingsTeamSeason>? GetSeasonStandings(int seasonYear)
        {
            return ExecuteGetSeasonStandings(seasonYear);
        }

        /// <summary>
        /// Gets all <see cref="StandingsTeamSeason"/> entities in the data store asynchronously.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="StandingsTeamSeason"/> entity to fetch.</param>
        /// <returns>An <see cref="IEnumerable{SeasonStanding}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<StandingsTeamSeason>?> GetSeasonStandingsAsync(int seasonYear)
        {
            return await ExecuteGetSeasonStandingsAsync(seasonYear);
        }

        protected virtual IEnumerable<StandingsTeamSeason>? ExecuteGetSeasonStandings(int seasonId)
        {
            if (dbContext.SeasonStandings is null)
            {
                return null;
            }
            return dbContext.SeasonStandings
                .FromSqlInterpolated($"EXEC sp_GetSeasonStandings @season_id = {seasonId}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<StandingsTeamSeason>?> ExecuteGetSeasonStandingsAsync(int seasonId)
        {
            if (dbContext.SeasonStandings is null)
            {
                return null;
            }
            return await dbContext.SeasonStandings
                .FromSqlInterpolated($"EXEC sp_GetSeasonStandings @season_id = {seasonId}")
                .ToListAsync();
        }
    }
}
