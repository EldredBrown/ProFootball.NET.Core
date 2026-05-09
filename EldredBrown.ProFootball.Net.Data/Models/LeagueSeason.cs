using System;
using System.Collections.Generic;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class LeagueSeason
{
    public int Id { get; set; }

    public string LeagueName { get; set; } = null!;

    public int SeasonYear { get; set; }

    public int TotalGames { get; set; }

    public int TotalPoints { get; set; }

    public decimal? AveragePoints { get; set; }

    public virtual League LeagueNameNavigation { get; set; } = null!;

    public virtual Season SeasonYearNavigation { get; set; } = null!;
}
