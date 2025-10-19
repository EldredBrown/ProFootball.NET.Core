using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a team's season schedule averages.
    /// </summary>
    public class TeamSeasonScheduleAverages
    {
        /// <summary>
        /// Gets or sets the average points scored per game by a team.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the average points scored per game against a team.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points scored per game by all opponents on a team's season schedule.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? SchedulePointsFor { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points allowed per game by all opponents on a team's season schedule.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? SchedulePointsAgainst { get; set; }
    }
}
