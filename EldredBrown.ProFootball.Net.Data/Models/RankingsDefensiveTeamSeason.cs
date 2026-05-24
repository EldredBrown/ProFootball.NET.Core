namespace EldredBrown.ProFootball.Net.Data.Models
{
    public class RankingsDefensiveTeamSeason
    {
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public decimal DefensiveAverage { get; set; }
        public decimal DefensiveFactor { get; set; }
        public decimal DefensiveIndex { get; set; }
    }
}
