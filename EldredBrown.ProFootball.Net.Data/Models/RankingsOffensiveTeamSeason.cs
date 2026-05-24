namespace EldredBrown.ProFootball.Net.Data.Models
{
    public class RankingsOffensiveTeamSeason
    {
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public decimal OffensiveAverage { get; set; }
        public decimal OffensiveFactor { get; set; }
        public decimal OffensiveIndex { get; set; }
    }
}
