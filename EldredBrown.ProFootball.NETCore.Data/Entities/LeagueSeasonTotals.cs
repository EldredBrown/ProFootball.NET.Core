using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a pro football league's totals for a season.
    /// </summary>
    public class LeagueSeasonTotals
    {
        /// <summary>
        /// Gets or sets the league's total games played for a season.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int? TotalGames { get; set; }

        /// <summary>
        /// Gets or sets the league's total points scored for a season.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int? TotalPoints { get; set; }
    }
}
