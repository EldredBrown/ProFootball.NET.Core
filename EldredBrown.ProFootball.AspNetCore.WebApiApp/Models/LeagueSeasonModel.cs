using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football league season.
    /// </summary>
    public class LeagueSeasonModel
    {
        /// <summary>
        /// Gets or sets the Id of the current <see cref="LeagueSeasonModel"/> object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="LeagueSeasonModel"/> object's league.
        /// </summary>
        [Required]
        public int LeagueId { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="LeagueSeasonModel"/> object's season.
        /// </summary>
        [Required]
        public int SeasonId { get; set; }

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
        public decimal? AveragePoints { get; set; }
    }
}
