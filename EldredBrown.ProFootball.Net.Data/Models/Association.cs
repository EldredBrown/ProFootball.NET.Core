using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Association
{
    public int Id { get; set; }

    public string LongName { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public int? ParentId { get; set; }

    public int FirstSeasonYear { get; set; }

    public int? LastSeasonYear { get; set; }

    [ValidateNever]
    public virtual Association? ParentIdNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season FirstSeasonYearNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Season? LastSeasonYearNavigation { get; set; }

    public virtual ICollection<Association> ChildAssociations { get; set; } = [];

    public virtual ICollection<Game> Games { get; set; } = [];

    public virtual ICollection<LeagueSeason> LeagueSeasons { get; set; } = [];

    public virtual ICollection<TeamSeason> TeamSeasonsLeagueOf { get; set; } = [];

    public virtual ICollection<TeamSeason> TeamSeasonsConferenceOf { get; set; } = [];

    public virtual ICollection<TeamSeason> TeamSeasonsDivisionOf { get; set; } = [];
}
