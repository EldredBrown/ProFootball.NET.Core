using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents an opponent in a team's season schedule profile.
    /// </summary>
    public class TeamSeasonOpponentProfile
    {
        /// <summary>
        /// Gets or sets the name of the opponent.
        /// </summary>
        public string Opponent { get; set; } = "";

        /// <summary>
        /// Gets or sets the team's points scored against the opponent.
        /// </summary>
        public int? GamePointsFor { get; set; }

        /// <summary>
        /// Gets or sets the opponent's points scored against the team.
        /// </summary>
        public int? GamePointsAgainst { get; set; }

        /// <summary>
        /// Gets or sets the number of opponent wins against all other teams.
        /// </summary>
        public int? OpponentWins { get; set; }

        /// <summary>
        /// Gets or sets the number of opponent losses against all other teams.
        /// </summary>
        public int? OpponentLosses { get; set; }

        /// <summary>
        /// Gets or sets the number of opponent ties against all other teams.
        /// </summary>
        public int? OpponentTies { get; set; }

        /// <summary>
        /// Gets or sets the opponent's winning percentage against all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:#.000}")]
        public double? OpponentWinningPercentage { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of opponent games against all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? OpponentWeightedGames { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of opponent points scored against all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? OpponentWeightedPointsFor { get; set; }

        /// <summary>
        /// Gets or sets the weighted total of opponent points allowed to all other teams.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? OpponentWeightedPointsAgainst { get; set; }
    }
}
