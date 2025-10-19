namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football league season.
    /// </summary>
    public class LeagueSeasonModel
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="LeagueSeasonModel"/> object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="LeagueSeasonModel"/> object's league.
        /// </summary>
        public string LeagueName { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="LeagueSeasonModel"/> object's season.
        /// </summary>
        public int SeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the total games of the current <see cref="LeagueSeasonModel"/> object.
        /// </summary>
        public int TotalGames { get; set; }

        /// <summary>
        /// Gets or sets the total points of the current <see cref="LeagueSeasonModel"/> object.
        /// </summary>
        public int TotalPoints { get; set; }

        /// <summary>
        /// Gets or sets the average points of the current <see cref="LeagueSeasonModel"/> object.
        /// </summary>
        public double? AveragePoints { get; set; }
    }
}
