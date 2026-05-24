namespace EldredBrown.ProFootball.Net.Data.Models
{
    public interface ILeagueSeason
    {
        int Id { get; set; }

        int LeagueId { get; set; }

        int SeasonId { get; set; }

        int TotalGames { get; set; }

        int TotalPoints { get; set; }

        decimal? AveragePoints { get; set; }

        League LeagueIdNavigation { get; set; }

        Season SeasonIdNavigation { get; set; }

        /// <summary>
        /// Updates the games and points totals of the wrapped <see cref="LeagueSeason"/> entity."
        /// </summary>
        /// <param name="totalGames">The value to be updated to the specified <see cref="LeagueSeason"/> entity's total games.</param>
        /// <param name="totalPoints">The value to be updated to the specified <see cref="LeagueSeason"/> entity's total points.</param>
        void UpdateGamesAndPoints(int totalGames, int totalPoints);
    }
}
