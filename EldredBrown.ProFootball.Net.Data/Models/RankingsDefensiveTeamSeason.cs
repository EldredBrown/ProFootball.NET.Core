namespace EldredBrown.ProFootball.Net.Data.Models
{
    public class RankingsDefensiveTeamSeason : IRankingsTeamSeason
    {
        public string TeamName { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public decimal? OffensiveAverage { get; private set; } = null;
        public decimal? OffensiveFactor { get; private set; } = null;
        public decimal? OffensiveIndex { get; private set; } = null;
        public decimal? DefensiveAverage { get; set; }
        public decimal? DefensiveFactor { get; set; }
        public decimal? DefensiveIndex { get; set; }
        public decimal? FinalExpectedWinningPercentage { get; private set; } = null;
    }
}
