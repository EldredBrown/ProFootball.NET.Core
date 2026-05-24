using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    public class SeasonRankingsRepository : ISeasonRankingsRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonScheduleProfileRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public SeasonRankingsRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsOffensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsOffensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsOffensiveTeamSeason}"/> collection.</returns>
        public IEnumerable<RankingsOffensiveTeamSeason> GetOffensiveRankingsForSeason(int seasonYear)
        {
            return ExecuteGetOffensiveRankingsForSeason(seasonYear);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsOffensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsOffensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsOffensiveTeamSeason}"/> collection.</returns>
        public async Task<IEnumerable<RankingsOffensiveTeamSeason>> GetOffensiveRankingsForSeasonAsync(int seasonYear)
        {
            return await ExecuteGetOffensiveRankingsForSeasonAsync(seasonYear);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsDefensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsDefensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsDefensiveTeamSeason}"/> collection.</returns>
        public IEnumerable<RankingsDefensiveTeamSeason> GetDefensiveRankingsForSeason(int seasonYear)
        {
            return ExecuteGetDefensiveRankingsForSeason(seasonYear);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsDefensiveTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsDefensiveTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsDefensiveTeamSeason}"/> collection.</returns>
        public async Task<IEnumerable<RankingsDefensiveTeamSeason>> GetDefensiveRankingsForSeasonAsync(int seasonYear)
        {
            return await ExecuteGetDefensiveRankingsForSeasonAsync(seasonYear);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsTotalTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsTotalTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsTotalTeamSeason}"/> collection.</returns>
        public IEnumerable<RankingsTotalTeamSeason> GetTotalRankingsForSeason(int seasonYear)
        {
            return ExecuteGetTotalRankingsForSeason(seasonYear);
        }

        /// <summary>
        /// Gets an enumerable collection (<see cref="IEnumerable{RankingsTotalTeamSeason}"/>) from the data store
        /// by season year.
        /// </summary>
        /// <param name="seasonYear">
        /// The season year of the <see cref="RankingsTotalTeamSeason"/> entity to fetch.
        /// </param>
        /// <returns>The fetched <see cref="IEnumerable{RankingsTotalTeamSeason}"/> collection.</returns>
        public async Task<IEnumerable<RankingsTotalTeamSeason>> GetTotalRankingsForSeasonAsync(int seasonYear)
        {
            return await ExecuteGetTotalRankingsForSeasonAsync(seasonYear);
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
        public Dictionary<string, Dictionary<string, object>> GetDataForRankingsUpdate(ITeamSeason teamSeason)
        {
            var results = new Dictionary<string, Dictionary<string, object>>();
            var resultKeys = new[] { "TeamSeasonScheduleTotals", "TeamSeasonScheduleAverages", "LeagueSeason" };

            var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("dbo.sp_GetDataForRankingsUpdate", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@teamId", teamSeason.TeamId);
            command.Parameters.AddWithValue("@leagueId", teamSeason.LeagueId);
            command.Parameters.AddWithValue("@seasonYear", teamSeason.SeasonYear);

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

        /// <summary>
        /// Gets a dictionary of data for the weekly rankings update 
        /// (<see cref="Dictionary{string, Dictionary{string, object}}"/>) from the data store for the specified 
        /// <see cref="TeamSeason"/> entity.
        /// </summary>
        /// <param name="teamSeason">
        /// The <see cref="TeamSeason"/> entity to fetch data for.
        /// </param>
        /// <returns>The fetched <see cref="Dictionary{string, Dictionary{string, object}}"/> dictionary.</returns>
        public async Task<Dictionary<string, Dictionary<string, object>>> 
            GetDataForRankingsUpdateAsync(ITeamSeason teamSeason)
        {
            var results = new Dictionary<string, Dictionary<string, object>>();
            var resultKeys = new[] { "TeamSeasonScheduleTotals", "TeamSeasonScheduleAverages", "LeagueSeason" };

            var connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            using var connection = new SqlConnection(connectionString);
            using var command = new SqlCommand("dbo.sp_GetDataForRankingsUpdate", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@teamId", teamSeason.TeamId);
            command.Parameters.AddWithValue("@leagueId", teamSeason.LeagueId);
            command.Parameters.AddWithValue("@seasonYear", teamSeason.SeasonYear);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            for (int i = 0; i < resultKeys.Length; i++)
            {
                if (await reader.ReadAsync())
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
                    await reader.NextResultAsync();
                }
            }

            return results;
        }

        protected virtual IEnumerable<RankingsOffensiveTeamSeason> 
            ExecuteGetOffensiveRankingsForSeason(int seasonYear)
        {
            return _dbContext.OffensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetOffensiveRankingsForSeason @seasonYear = {seasonYear}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<RankingsOffensiveTeamSeason>>
            ExecuteGetOffensiveRankingsForSeasonAsync(int seasonYear)
        {
            return await _dbContext.OffensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetOffensiveRankingsForSeason @seasonYear = {seasonYear}")
                .ToListAsync();
        }

        protected virtual IEnumerable<RankingsDefensiveTeamSeason>
            ExecuteGetDefensiveRankingsForSeason(int seasonYear)
        {
            return _dbContext.DefensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetDefensiveRankingsForSeason @seasonYear = {seasonYear}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<RankingsDefensiveTeamSeason>>
            ExecuteGetDefensiveRankingsForSeasonAsync(int seasonYear)
        {
            return await _dbContext.DefensiveRankings
                .FromSqlInterpolated($"EXEC sp_GetDefensiveRankingsForSeason @seasonYear = {seasonYear}")
                .ToListAsync();
        }

        protected virtual IEnumerable<RankingsTotalTeamSeason>
            ExecuteGetTotalRankingsForSeason(int seasonYear)
        {
            return _dbContext.TotalRankings
                .FromSqlInterpolated($"EXEC sp_GetTotalRankingsForSeason @seasonYear = {seasonYear}")
                .ToList();
        }

        protected virtual async Task<IEnumerable<RankingsTotalTeamSeason>>
            ExecuteGetTotalRankingsForSeasonAsync(int seasonYear)
        {
            return await _dbContext.TotalRankings
                .FromSqlInterpolated($"EXEC sp_GetTotalRankingsForSeason @seasonYear = {seasonYear}")
                .ToListAsync();
        }
    }
}
