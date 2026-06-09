using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TeamSeasonScheduleProfileRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class SeasonRankingsRepository(
        ProFootballDbContext dbContext,
        IConnectionStringProvider connectionStringProvider,
        IDbConnectionFactory connectionFactory
    ) : ISeasonRankingsRepository
    {

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsOffensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonId">
        /// The season year of the <see cref="RankingsOffensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsOffensiveTeamSeason}"/> collection.</returns>
        public IEnumerable<RankingsOffensiveTeamSeason> GetOffensiveRankingsForSeason(int seasonId)
        {
            return ExecuteGetOffensiveRankingsForSeason(seasonId);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsOffensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonId">
        /// The season year of the <see cref="RankingsOffensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsOffensiveTeamSeason}"/> collection.</returns>
        public async Task<IEnumerable<RankingsOffensiveTeamSeason>> GetOffensiveRankingsForSeasonAsync(int seasonId)
        {
            return await ExecuteGetOffensiveRankingsForSeasonAsync(seasonId);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsDefensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonId">
        /// The season year of the <see cref="RankingsDefensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsDefensiveTeamSeason}"/> collection.</returns>
        public IEnumerable<RankingsDefensiveTeamSeason> GetDefensiveRankingsForSeason(int seasonId)
        {
            return ExecuteGetDefensiveRankingsForSeason(seasonId);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsDefensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonId">
        /// The season year of the <see cref="RankingsDefensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsDefensiveTeamSeason}"/> collection.</returns>
        public async Task<IEnumerable<RankingsDefensiveTeamSeason>> GetDefensiveRankingsForSeasonAsync(int seasonId)
        {
            return await ExecuteGetDefensiveRankingsForSeasonAsync(seasonId);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsTotalTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonId">
        /// The season year of the <see cref="RankingsTotalTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsTotalTeamSeason}"/> collection.</returns>
        public IEnumerable<RankingsTotalTeamSeason> GetTotalRankingsForSeason(int seasonId)
        {
            return ExecuteGetTotalRankingsForSeason(seasonId);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsTotalTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonId">
        /// The season year of the <see cref="RankingsTotalTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsTotalTeamSeason}"/> collection.</returns>
        public async Task<IEnumerable<RankingsTotalTeamSeason>> GetTotalRankingsForSeasonAsync(int seasonId)
        {
            return await ExecuteGetTotalRankingsForSeasonAsync(seasonId);
        }

        /// <summary>
        /// Gets a dictionary of data for the weekly rankings update 
        /// (<see cref="Dictionary{string, Dictionary{string, object}}"/>) from the data store for the specified 
        /// <see cref="TeamSeason"/> entity.
        /// </summary>
        /// <param name="teamSeason">
        /// The <see cref="TeamSeason"/> entity to fetch data for.
        /// </param>
        /// <returns>The fetched <see cref="Dictionary{string, Dictionary{string, object}}"/> dictionary.</returns>
        public Dictionary<string, Dictionary<string, object>> GetDataForRankingsUpdate(TeamSeason teamSeason)
        {
            var results = new Dictionary<string, Dictionary<string, object>>();
            var resultKeys = new[] { "TeamSeasonScheduleTotals", "TeamSeasonScheduleAverages", "LeagueSeason" };

            var connectionString = connectionStringProvider.GetConnectionString();
            using var connection = connectionFactory.CreateConnection(connectionString);

            using var command = connection.CreateCommand();
            command.CommandText = "dbo.sp_GetDataForRankingsUpdate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@team_id", teamSeason.TeamId));
            command.Parameters.Add(new SqlParameter("@league_id", teamSeason.LeagueId));
            command.Parameters.Add(new SqlParameter("@season_id", teamSeason.SeasonId));

            connection.Open();

            using var reader = command.ExecuteReader();

            for (int i = 0; i < resultKeys.Length; i++)
            {
                if (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int col = 0; col < reader.FieldCount; col++)
                    {
                        row[reader.GetName(col)] = reader.IsDBNull(col) ? null : reader.GetValue(col);
                    }
                    results[resultKeys[i]] = row;
                }

                if (i < resultKeys.Length - 1)
                {
                    reader.NextResult();
                }
            }

            return results;
        }

        protected virtual IEnumerable<RankingsOffensiveTeamSeason> 
            ExecuteGetOffensiveRankingsForSeason(int seasonId)
        {
            return dbContext.OffensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetRankingsOffensive @season_id = {seasonId}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<RankingsOffensiveTeamSeason>>
            ExecuteGetOffensiveRankingsForSeasonAsync(int seasonId)
        {
            return await dbContext.OffensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetRankingsOffensive @season_id = {seasonId}")
                .ToListAsync();
        }

        protected virtual IEnumerable<RankingsDefensiveTeamSeason>
            ExecuteGetDefensiveRankingsForSeason(int seasonId)
        {
            return dbContext.DefensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetRankingsDefensive @season_id = {seasonId}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<RankingsDefensiveTeamSeason>>
            ExecuteGetDefensiveRankingsForSeasonAsync(int seasonId)
        {
            return await dbContext.DefensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetRankingsDefensive @season_id = {seasonId}")
                .ToListAsync();
        }

        protected virtual IEnumerable<RankingsTotalTeamSeason>
            ExecuteGetTotalRankingsForSeason(int seasonId)
        {
            return dbContext.TotalRankings
                .FromSqlInterpolated($"EXEC sp_GetRankingsTotal @season_id = {seasonId}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<RankingsTotalTeamSeason>>
            ExecuteGetTotalRankingsForSeasonAsync(int seasonId)
        {
            return await dbContext.TotalRankings
                .FromSqlInterpolated($"EXEC sp_GetRankingsTotal @season_id = {seasonId}")
                .ToListAsync();
        }
    }
}
