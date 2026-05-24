namespace EldredBrown.ProFootball.Net.Data.Models
{
    public partial class LeagueSeason : ILeagueSeason
    {
        /// <summary>
        /// Updates the games and points totals of the wrapped <see cref="LeagueSeason"/> entity."
        /// </summary>
        /// <param name="totalGames">The value to be updated to the specified <see cref="LeagueSeason"/> entity's total games.</param>
        /// <param name="totalPoints">The value to be updated to the specified <see cref="LeagueSeason"/> entity's total points.</param>
        public void UpdateGamesAndPoints(int totalGames, int totalPoints)
        {
            TotalGames = totalGames;
            TotalPoints = totalPoints;
            AveragePoints = totalGames != 0
                ? totalPoints / (decimal)totalGames
                : null;
        }
    }
}
