using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="StandingsTeamSeason"/> data store.
    /// </summary>
    public interface ISeasonStandingsRepository
    {
        /// <summary>
        /// Gets all <see cref="StandingsTeamSeason"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="StandingsTeamSeason"/> entity to fetch.</param>
        /// <returns>An <see cref="IEnumerable{SeasonStanding}"/> of all fetched entities.</returns>
        IEnumerable<StandingsTeamSeason>? GetSeasonStandings(int seasonYear);

        /// <summary>
        /// Gets all <see cref="StandingsTeamSeason"/> entities in the data store asynchronously.
        /// </summary>
        /// <param name="seasonYear">The season year of the <see cref="StandingsTeamSeason"/> entity to fetch.</param>
        /// <returns>An <see cref="IEnumerable{SeasonStanding}"/> of all fetched entities.</returns>
        Task<IEnumerable<StandingsTeamSeason>?> GetSeasonStandingsAsync(int seasonYear);
    }
}
