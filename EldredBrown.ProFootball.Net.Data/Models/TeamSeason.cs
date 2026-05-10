namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class TeamSeason
{
    public int Id { get; set; }

    public string TeamName { get; set; } = null!;

    public int SeasonYear { get; set; }

    public string LeagueName { get; set; } = null!;

    public string? ConferenceName { get; set; }

    public string? DivisionName { get; set; }

    public int Games { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int Ties { get; set; }

    public int PointsFor { get; set; }

    public int PointsAgainst { get; set; }

    public decimal ExpectedWins { get; set; }

    public decimal ExpectedLosses { get; set; }

    public decimal? OffensiveAverage { get; set; }

    public decimal? OffensiveFactor { get; set; }

    public decimal? OffensiveIndex { get; set; }

    public decimal? DefensiveAverage { get; set; }

    public decimal? DefensiveFactor { get; set; }

    public decimal? DefensiveIndex { get; set; }

    public decimal? FinalExpectedWinningPercentage { get; set; }

    public virtual Conference? ConferenceNameNavigation { get; set; }

    public virtual Division? DivisionNameNavigation { get; set; }

    public virtual League LeagueNameNavigation { get; set; } = null!;

    public virtual Season SeasonYearNavigation { get; set; } = null!;

    public virtual Team TeamNameNavigation { get; set; } = null!;
}
