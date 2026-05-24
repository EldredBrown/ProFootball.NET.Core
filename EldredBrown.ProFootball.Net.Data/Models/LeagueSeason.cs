namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class LeagueSeason
{
    public int Id { get; set; }

    public int LeagueId { get; set; }

    public int SeasonId { get; set; }

    public int TotalGames { get; set; }

    public int TotalPoints { get; set; }

    public decimal? AveragePoints { get; set; }

    public virtual League LeagueIdNavigation { get; set; } = null!;

    public virtual Season SeasonIdNavigation { get; set; } = null!;
}
