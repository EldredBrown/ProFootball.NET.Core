using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.Net.Data.Models
{
    /// <summary>
    /// Represents an opponent in a team's season schedule profile.
    /// </summary>
    public class TeamSeasonOpponentProfile
    {
        /// <summary>
        /// Gets or sets the name of the opponent.
        /// </summary>
        public string Opponent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the team's points scored against the opponent.
        /// </summary>
        [DisplayName("GPF")]
        public int? GamePointsFor { get; set; }

        /// <summary>
        /// Gets or sets the opponent's points scored against the team.
        /// </summary>
        [DisplayName("GPA")]
        public int? GamePointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the number of opponent wins against all other teams.
        /// </summary>
        [DisplayName("OppW")]
        public int? OpponentWins { get; set; }

        /// <summary>
        /// Gets or sets the number of opponent losses against all other teams.
        /// </summary>
        [DisplayName("OppL")]
        public int? OpponentLosses { get; set; }

        /// <summary>
        /// Gets or sets the number of opponent ties against all other teams.
        /// </summary>
        [DisplayName("OppT")]
        public int? OpponentTies { get; set; }

        /// <summary>
        /// Gets or sets the opponent's winning percentage against all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        [DisplayName("OppW%")]
        public decimal? OpponentWinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of opponent games against all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [DisplayName("OppWG")]
        public decimal? OpponentWeightedGames { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of opponent points scored against all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [DisplayName("OppWPF")]
        public decimal? OpponentWeightedPointsFor { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of opponent points allowed to all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        [DisplayName("OppWPA")]
        public decimal? OpponentWeightedPointsAgainst { get; set; }
    }
}
