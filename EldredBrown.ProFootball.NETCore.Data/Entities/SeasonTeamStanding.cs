using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a team in the season standings.
    /// </summary>
    public class SeasonTeamStanding
    {
        /// <summary>
        /// Gets or sets the name the current <see cref="SeasonTeamStanding"/> entity's team.
        /// </summary>
        public string Team { get; set; } = "";

        /// <summary>
        /// Gets or sets the name of the current <see cref="SeasonTeamStanding"/> entity's conference.
        /// </summary>
        public string? Conference { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="SeasonTeamStanding"/> entity's division.
        /// </summary>
        public string? Division { get; set; }

        /// <summary>
        /// Gets or sets the number of wins of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the number of losses of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets the number of ties of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        public int Ties { get; set; }

        /// <summary>
        /// Gets or sets the winning percentage of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        public double? WinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the points for of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        public int PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the points against of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        public int PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the average points for of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? AvgPointsFor { get; set; }

        /// <summary>
        /// Gets or sets the average points against of the current <see cref="SeasonTeamStanding"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? AvgPointsAgainst { get; set; }
    }
}
