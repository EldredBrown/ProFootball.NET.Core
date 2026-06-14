using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a pro football team season.
    /// </summary>
    public class TeamSeasonModel
    {
        /// <summary>
        /// Gets or sets the Id of the current <see cref="TeamSeasonModel"/> object.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Id of the current <see cref="TeamSeasonModel"/> object's team.
        /// </summary>
        [Required]
        public int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the current <see cref="TeamSeasonModel"/> object's season.
        /// </summary>
        [Required]
        public int SeasonId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the current <see cref="TeamSeasonModel"/> object's league.
        /// </summary>
        [Required]
        public int LeagueId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the current <see cref="TeamSeasonModel"/> object's conference.
        /// </summary>
        public int? ConferenceId { get; set; }

        /// <summary>
        /// Gets or sets the Id of the current <see cref="TeamSeasonModel"/> object's division.
        /// </summary>
        public int? DivisionId { get; set; }

        /// <summary>
        /// Gets or sets the number of games played by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public int Games { get; set; }

        /// <summary>
        /// Gets or sets the number of games won by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the number of games lost by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets the number of games tied by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public int Ties { get; set; }

        /// <summary>
        /// Gets or sets the winning percentage of the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public decimal? WinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the number of points scored by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public int PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the number of points allowed by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public int PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the number of games won by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public decimal ExpectedWins { get; set; }

        /// <summary>
        /// Gets or sets the number of games lost by the current <see cref="TeamSeasonModel"/>.
        /// </summary>
        public decimal ExpectedLosses { get; set; }

        /// <summary>
        /// Gets or sets the the current <see cref="TeamSeasonModel"/> object's offensive average.
        /// </summary>
        public decimal? OffensiveAverage { get; set; }

        /// <summary>
        /// Gets or sets the the current <see cref="TeamSeasonModel"/> object's offensive factor.
        /// </summary>
        public decimal? OffensiveFactor { get; set; }

        /// <summary>
        /// Gets or sets the the current <see cref="TeamSeasonModel"/> object's offensive index.
        /// </summary>
        public decimal? OffensiveIndex { get; set; }

        /// <summary>
        /// Gets or sets the the current <see cref="TeamSeasonModel"/> object's defensive average.
        /// </summary>
        public decimal? DefensiveAverage { get; set; }

        /// <summary>
        /// Gets or sets the the current <see cref="TeamSeasonModel"/> object's defensive factor.
        /// </summary>
        public decimal? DefensiveFactor { get; set; }

        /// <summary>
        /// Gets or sets the the current <see cref="TeamSeasonModel"/> object's defensive index.
        /// </summary>
        public decimal? DefensiveIndex { get; set; }

        /// <summary>
        /// Gets or sets the the current <see cref="TeamSeasonModel"/> object's final expected winning percentage.
        /// </summary>
        public decimal? FinalExpectedWinningPercentage { get; set; }
    }
}
