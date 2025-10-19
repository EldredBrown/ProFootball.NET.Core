using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Provides read access to the GetSeasonStandings stored procedure.
    /// </summary>
    public class SeasonStandingsRepository : ISeasonStandingsRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonStandingsRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public SeasonStandingsRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="SeasonTeamStanding"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="SeasonTeamStanding"/> entity to fetch.</param>
        /// <returns>An <see cref="IEnumerable{SeasonStanding}"/> of all fetched entities.</returns>
        public IEnumerable<SeasonTeamStanding> GetSeasonStandings(int seasonYear)
        {
            return _dbContext.SeasonStandings.FromSqlInterpolated($"sp_GetSeasonStandings {seasonYear}").ToList();
        }

        /// <summary>
        /// Gets all <see cref="SeasonTeamStanding"/> entities in the data store asynchronously.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="SeasonTeamStanding"/> entity to fetch.</param>
        /// <returns>An <see cref="IEnumerable{SeasonStanding}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<SeasonTeamStanding>> GetSeasonStandingsAsync(int seasonYear)
        {
            return await _dbContext.SeasonStandings.FromSqlInterpolated(
                $"sp_GetSeasonStandings {seasonYear}").ToListAsync();
        }
    }
}
