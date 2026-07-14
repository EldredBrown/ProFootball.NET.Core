using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Season
{
    [DisplayName("Year")]
    public int Year { get; set; }

    [ValidateNever]
    public virtual ICollection<Association> AssociationsFirstSeasonOf { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<Association> AssociationsLastSeasonOf { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<Game> Games { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<LeagueSeason> LeagueSeasons { get; set; } = [];

    [ValidateNever]
    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = [];
}
