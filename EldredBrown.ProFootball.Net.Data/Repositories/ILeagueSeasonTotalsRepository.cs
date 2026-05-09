using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="LeagueSeasonTotals"/> data store.
    /// </summary>
    public interface ILeagueSeasonTotalsRepository
    {
        /// <summary>
        /// Gets a single <see cref="LeagueSeasonTotals"/> entity from the data store by league name and season Id.
        /// </summary>
        /// <param name="leagueName">The league name of the <see cref="LeagueSeasonTotals"/> entity to fetch.</param>
        /// <param name="seasonYear">The season year of the <see cref="LeagueSeasonTotals"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="LeagueSeasonTotals"/> entity.</returns>
        LeagueSeasonTotals GetLeagueSeasonTotals(string leagueName, int seasonYear);
    }
}
