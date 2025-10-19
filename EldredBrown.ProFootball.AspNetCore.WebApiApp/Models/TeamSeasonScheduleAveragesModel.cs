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
        public double? PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the average points scored per game against a team.
        /// </summary>
        public double? PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points scored per game by all opponents on a team's season schedule.
        /// </summary>
        public double? SchedulePointsFor { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points allowed per game by all opponents on a team's season schedule.
        /// </summary>
        public double? SchedulePointsAgainst { get; set; }
    }
}
