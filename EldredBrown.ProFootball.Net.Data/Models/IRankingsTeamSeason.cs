namespace EldredBrown.ProFootball.Net.Data.Models
{
    public interface IRankingsTeamSeason
    {
        string TeamName { get; set; }
        int Wins { get; set; }
        int Losses { get; set; }
        int Ties { get; set; }
        decimal? OffensiveAverage { get; }
        decimal? OffensiveFactor { get; }
        decimal? OffensiveIndex { get; }
        decimal? DefensiveAverage { get; }
        decimal? DefensiveFactor { get; }
        decimal? DefensiveIndex { get; }
        decimal? FinalExpectedWinningPercentage { get; }
    }
}
