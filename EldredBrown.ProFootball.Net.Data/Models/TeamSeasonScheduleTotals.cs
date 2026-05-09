using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.Net.Data.Models
{
    /// <summary>
    /// Represents a team's season schedule totals.
    /// </summary>
    public class TeamSeasonScheduleTotals
    {
        /// <summary>
        /// Gets or sets the total games played by a team.
        /// </summary>
        public int? Games { get; set; }

        /// <summary>
        /// Gets or sets the total points scored by a team.
        /// </summary>
        public int? PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the total points scored against a team.
        /// </summary>
        public int? PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the total wins by all opponents on a team's season schedule.
        /// </summary>
        public int? ScheduleWins { get; set; }

        /// <summary>
        /// Gets or sets the total losses by all opponents on a team's season schedule.
        /// </summary>
        public int? ScheduleLosses { get; set; }

        /// <summary>
        /// Gets or sets the total ties by all opponents on a team's season schedule.
        /// </summary>
        public int? ScheduleTies { get; set; }

        /// <summary>
        /// Gets or sets the winning percentage by all opponents on a team's season schedule.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        public decimal? ScheduleWinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of games by all opponents on a team's season schedule.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? ScheduleGames { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of points scored by all opponents on a team's season schedule.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? SchedulePointsFor { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of points allowed by all opponents on a team's season schedule.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? SchedulePointsAgainst { get; set; }
    }
}
