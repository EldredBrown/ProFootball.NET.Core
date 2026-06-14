namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    /// <summary>
    /// Represents a model of a team's season schedule averages.
    /// </summary>
    public class TeamSeasonScheduleAveragesModel
    {
        /// <summary>
        /// Gets or sets the average points scored per game by a team.
        /// </summary>
        public decimal? PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the average points scored per game against a team.
        /// </summary>
        public decimal? PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points scored per game by all opponents on a team's season schedule.
        /// </summary>
        public decimal? SchedulePointsFor { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points allowed per game by all opponents on a team's season schedule.
        /// </summary>
        public decimal? SchedulePointsAgainst { get; set; }
    }
}
