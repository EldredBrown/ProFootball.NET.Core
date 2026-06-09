using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.Net.Data.Models
{
    /// <summary>
    /// Represents a team in the season standings.
    /// </summary>
    public class StandingsTeamSeason
    {
        /// <summary>
        /// Gets or sets the name the current <see cref="StandingsTeamSeason"/> entity's team.
        /// </summary>
        public string Team { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the current <see cref="StandingsTeamSeason"/> entity's conference.
        /// </summary>
        //public string? Conference { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="StandingsTeamSeason"/> entity's division.
        /// </summary>
        //public string? Division { get; set; }

        /// <summary>
        /// Gets or sets the number of wins of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the number of losses of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets the number of ties of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        public int Ties { get; set; }

        /// <summary>
        /// Gets or sets the winning percentage of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        public decimal? WinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the points for of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        public int PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the points against of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        public int PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the average points for of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? AvgPointsFor { get; set; }

        /// <summary>
        /// Gets or sets the average points against of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? AvgPointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the number of expected wins of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal? ExpectedWins { get; set; }

        /// <summary>
        /// Gets or sets the number of expected losses of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal? ExpectedLosses { get; set; }
    }
}
