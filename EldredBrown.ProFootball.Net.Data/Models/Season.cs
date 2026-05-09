using System;
using System.Collections.Generic;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Season
{
    public int Id { get; set; }

    public int Year { get; set; }

    public int NumOfWeeksScheduled { get; set; }

    public int NumOfWeeksCompleted { get; set; }

    public virtual ICollection<Conference> ConferenceFirstSeasonYearNavigations { get; set; } = new List<Conference>();

    public virtual ICollection<Conference> ConferenceLastSeasonYearNavigations { get; set; } = new List<Conference>();

    public virtual ICollection<Division> DivisionFirstSeasonYearNavigations { get; set; } = new List<Division>();

    public virtual ICollection<Division> DivisionLastSeasonYearNavigations { get; set; } = new List<Division>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<League> LeagueFirstSeasonYearNavigations { get; set; } = new List<League>();

    public virtual ICollection<League> LeagueLastSeasonYearNavigations { get; set; } = new List<League>();

    public virtual ICollection<LeagueSeason> LeagueSeasons { get; set; } = new List<LeagueSeason>();

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}
