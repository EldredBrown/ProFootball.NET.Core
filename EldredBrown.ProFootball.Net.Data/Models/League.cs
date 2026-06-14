using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class League
{
    public int Id { get; set; }

    public string ShortName { get; set; } = null!;

    public string LongName { get; set; } = null!;

    [Display(Name = "First Season")]
    public int FirstSeasonId { get; set; }

    [Display(Name = "Last Season")]
    public int? LastSeasonId { get; set; }

    [ValidateNever]
    public virtual Season FirstSeasonIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season? LastSeasonIdNavigation { get; set; }

    public virtual ICollection<Conference> Conferences { get; set; } = [];

    public virtual ICollection<Division> Divisions { get; set; } = [];

    public virtual ICollection<LeagueSeason> LeagueSeasons { get; set; } = [];

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = [];
}
