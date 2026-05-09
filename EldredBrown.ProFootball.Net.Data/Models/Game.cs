using System;
using System.Collections.Generic;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Game
{
    public int Id { get; set; }

    public int SeasonYear { get; set; }

    public int Week { get; set; }

    public string GuestName { get; set; } = null!;

    public int GuestScore { get; set; }

    public string HostName { get; set; } = null!;

    public int HostScore { get; set; }

    public bool IsPlayoff { get; set; }

    public string? Notes { get; set; }

    public virtual Season SeasonYearNavigation { get; set; } = null!;
}
