using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class LeagueSeason
{
    public int Id { get; set; }

    public int LeagueId { get; set; }

    public int SeasonYear { get; set; }

    public int NumOfWeeksScheduled { get; set; }

    public int NumOfWeeksCompleted { get; set; }

    public int TotalGames { get; set; }

    public int TotalPoints { get; set; }

    public decimal? AveragePoints { get; set; }

    [ValidateNever]
    public virtual Association LeagueIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season SeasonYearNavigation { get; set; } = null!;
}
