using System.Collections.Generic;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    public interface ITeamSeasonScheduleRepository
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
        IEnumerable<TeamSeasonOpponentProfile> GetTeamSeasonScheduleProfile(string teamName, int seasonYear);

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
        Task<IEnumerable<TeamSeasonOpponentProfile>> GetTeamSeasonScheduleProfileAsync(string teamName, int seasonYear);

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
        TeamSeasonScheduleTotals GetTeamSeasonScheduleTotals(string teamName, int seasonYear);

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
        Task<TeamSeasonScheduleTotals> GetTeamSeasonScheduleTotalsAsync(string teamName, int seasonYear);

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
        TeamSeasonScheduleAverages GetTeamSeasonScheduleAverages(string teamName, int seasonYear);

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
        Task<TeamSeasonScheduleAverages> GetTeamSeasonScheduleAveragesAsync(string teamName, int seasonYear);
    }
}
