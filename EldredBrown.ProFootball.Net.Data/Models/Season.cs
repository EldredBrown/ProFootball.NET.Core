using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Season
{
    [Display(Name = "Year")]
    public int Id { get; set; }

    public int NumOfWeeksScheduled { get; set; }

    public int NumOfWeeksCompleted { get; set; }

    public virtual ICollection<League> LeagueFirstSeasonIdNavigations { get; set; } = new List<League>();

    public virtual ICollection<League> LeagueLastSeasonIdNavigations { get; set; } = new List<League>();

    public virtual ICollection<Conference> ConferenceFirstSeasonIdNavigations { get; set; } = new List<Conference>();

    public virtual ICollection<Conference> ConferenceLastSeasonIdNavigations { get; set; } = new List<Conference>();

    public virtual ICollection<Division> DivisionFirstSeasonIdNavigations { get; set; } = new List<Division>();

    public virtual ICollection<Division> DivisionLastSeasonIdNavigations { get; set; } = new List<Division>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<LeagueSeason> LeagueSeasons { get; set; } = new List<LeagueSeason>();

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}
