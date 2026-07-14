using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.Net.Data.Models
{
    /// <summary>
    /// Represents a team's season schedule averages.
    /// </summary>
    public class TeamSeasonScheduleAverages
    {
        /// <summary>
        /// Gets or sets the average points scored per game by a team.
        /// </summary>
        [DisplayName("PF")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the average points scored per game against a team.
        /// </summary>
        [DisplayName("PA")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points scored per game by all opponents on a team's season schedule.
        /// </summary>
        [DisplayName("SchPF")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? SchedulePointsFor { get; set; }

        /// <summary>
        /// Gets or sets the weighted average points allowed per game by all opponents on a team's season schedule.
        /// </summary>
        [DisplayName("SchPA")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? SchedulePointsAgainst { get; set; }
    }
}
