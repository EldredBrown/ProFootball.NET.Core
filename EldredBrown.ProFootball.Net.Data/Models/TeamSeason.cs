namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class TeamSeason
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public int SeasonId { get; set; }

    public int LeagueId { get; set; }

    public int? ConferenceId { get; set; }

    public int? DivisionId { get; set; }

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

    public virtual Team TeamIdNavigation { get; set; } = null!;

    public virtual Season SeasonIdNavigation { get; set; } = null!;

    public virtual League LeagueIdNavigation { get; set; } = null!;

    public virtual Conference? ConferenceIdNavigation { get; set; }

    public virtual Division? DivisionIdNavigation { get; set; }
}
