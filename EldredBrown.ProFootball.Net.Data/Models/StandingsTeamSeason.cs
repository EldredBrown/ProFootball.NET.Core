using System.ComponentModel;
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
        [DisplayName("Team")]
        public string Team { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of wins of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("W")]
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the number of losses of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("L")]
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets the number of ties of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("T")]
        public int Ties { get; set; }

        /// <summary>
        /// Gets or sets the winning percentage of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("Pct.")]
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        public decimal? WinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the points for of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("PF")]
        public int PointsFor { get; set; }

        /// <summary>
        /// Gets or sets the points against of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("PA")]
        public int PointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the average points for of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("AvgPF")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? AvgPointsFor { get; set; }

        /// <summary>
        /// Gets or sets the average points against of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("AvgPA")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? AvgPointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the number of expected wins of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("ExpW")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal? ExpectedWins { get; set; }

        /// <summary>
        /// Gets or sets the number of expected losses of the current <see cref="StandingsTeamSeason"/> entity.
        /// </summary>
        [DisplayName("ExpL")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public decimal? ExpectedLosses { get; set; }
    }
}
