using System;
using System.Collections.Generic;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Division
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LeagueName { get; set; } = null!;

    public string? ConferenceName { get; set; }

    public int FirstSeasonYear { get; set; }

    public int? LastSeasonYear { get; set; }

    public virtual Conference? ConferenceNameNavigation { get; set; }

    public virtual Season FirstSeasonYearNavigation { get; set; } = null!;

    public virtual Season? LastSeasonYearNavigation { get; set; }

    public virtual League LeagueNameNavigation { get; set; } = null!;

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}
