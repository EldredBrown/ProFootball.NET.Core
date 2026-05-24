using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    public interface ISeasonRankingsRepository
    {
        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsOffensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsOffensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsOffensiveTeamSeason}"/> collection.</returns>
        IEnumerable<RankingsOffensiveTeamSeason> GetOffensiveRankingsForSeason(int seasonYear);

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsOffensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsOffensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsOffensiveTeamSeason}"/> collection.</returns>
        Task<IEnumerable<RankingsOffensiveTeamSeason>> GetOffensiveRankingsForSeasonAsync(int seasonYear);

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsDefensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsDefensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsDefensiveTeamSeason}"/> collection.</returns>
        IEnumerable<RankingsDefensiveTeamSeason> GetDefensiveRankingsForSeason(int seasonYear);

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsDefensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsDefensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsDefensiveTeamSeason}"/> collection.</returns>
        Task<IEnumerable<RankingsDefensiveTeamSeason>> GetDefensiveRankingsForSeasonAsync(int seasonYear);

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsTotalTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsTotalTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsTotalTeamSeason}"/> collection.</returns>
        IEnumerable<RankingsTotalTeamSeason> GetTotalRankingsForSeason(int seasonYear);

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsTotalTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsTotalTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsTotalTeamSeason}"/> collection.</returns>
        Task<IEnumerable<RankingsTotalTeamSeason>> GetTotalRankingsForSeasonAsync(int seasonYear);

        /// <summary>
        /// Gets a dictionary of data for the weekly rankings update 
        /// (<see cref="Dictionary{string, Dictionary{string, object}}"/>) from the data store for the specified 
        /// <see cref="TeamSeason"/> entity.
        /// </summary>
        /// <param name="teamSeason">
        /// The <see cref="TeamSeason"/> entity to fetch data for.
        /// </param>
        /// <returns>The fetched <see cref="Dictionary{string, Dictionary{string, object}}"/> dictionary.</returns>
        Dictionary<string, Dictionary<string, object>> GetDataForRankingsUpdate(ITeamSeason teamSeason);

        /// <summary>
        /// Gets a dictionary of data for the weekly rankings update 
        /// (<see cref="Dictionary{string, Dictionary{string, object}}"/>) from the data store for the specified 
        /// <see cref="TeamSeason"/> entity.
        /// </summary>
        /// <param name="teamSeason">
        /// The <see cref="TeamSeason"/> entity to fetch data for.
        /// </param>
        /// <returns>The fetched <see cref="Dictionary{string, Dictionary{string, object}}"/> dictionary.</returns>
        Task<Dictionary<string, Dictionary<string, object>>> GetDataForRankingsUpdateAsync(ITeamSeason teamSeason);
    }
}
