using System;
using System.Collections.Generic;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class League
{
    public int Id { get; set; }

    public string ShortName { get; set; } = null!;

    public string LongName { get; set; } = null!;

    public int FirstSeasonYear { get; set; }

    public int? LastSeasonYear { get; set; }

    public virtual ICollection<Conference> Conferences { get; set; } = new List<Conference>();

    public virtual ICollection<Division> Divisions { get; set; } = new List<Division>();

    public virtual Season FirstSeasonYearNavigation { get; set; } = null!;

    public virtual Season? LastSeasonYearNavigation { get; set; }

    public virtual ICollection<LeagueSeason> LeagueSeasons { get; set; } = new List<LeagueSeason>();

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}
