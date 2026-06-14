using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Season
{
    [Display(Name = "Year")]
    public int Id { get; set; }

    public int NumOfWeeksScheduled { get; set; }

    public int NumOfWeeksCompleted { get; set; }

    [ValidateNever]
    public virtual ICollection<League> LeagueFirstSeasonIdNavigations { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<League> LeagueLastSeasonIdNavigations { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<Conference> ConferenceFirstSeasonIdNavigations { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<Conference> ConferenceLastSeasonIdNavigations { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<Division> DivisionFirstSeasonIdNavigations { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<Division> DivisionLastSeasonIdNavigations { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<Game> Games { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<LeagueSeason> LeagueSeasons { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = [];
}
