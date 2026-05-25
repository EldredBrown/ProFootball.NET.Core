using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Division
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int LeagueId { get; set; }

    public int? ConferenceId { get; set; }

    public int FirstSeasonId { get; set; }

    public int? LastSeasonId { get; set; }

    [ValidateNever]
    public virtual League LeagueIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Conference? ConferenceIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season FirstSeasonIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season? LastSeasonIdNavigation { get; set; }

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}
