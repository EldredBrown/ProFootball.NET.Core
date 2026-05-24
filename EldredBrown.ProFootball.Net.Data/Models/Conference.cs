using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Conference
{
    public int Id { get; set; }

    public string ShortName { get; set; } = null!;

    public string LongName { get; set; } = null!;

    public int LeagueId { get; set; }

    public int FirstSeasonId { get; set; }

    public int? LastSeasonId { get; set; }

    [ValidateNever]
    public virtual League LeagueIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season FirstSeasonIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season? LastSeasonIdNavigation { get; set; }

    public virtual ICollection<Division> Divisions { get; set; } = new List<Division>();

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}
