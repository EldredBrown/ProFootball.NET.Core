namespace EldredBrown.ProFootball.Net.Data.Models
{
    public class RankingsOffensiveTeamSeason : IRankingsTeamSeason
    {
        public string TeamName { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public decimal? OffensiveAverage { get; set; }
        public decimal? OffensiveFactor { get; set; }
        public decimal? OffensiveIndex { get; set; }
        public decimal? DefensiveAverage { get; private set; } = null;
        public decimal? DefensiveFactor { get; private set; } = null;
        public decimal? DefensiveIndex { get; private set; } = null;
        public decimal? FinalExpectedWinningPercentage { get; private set; } = null;
    }
}
