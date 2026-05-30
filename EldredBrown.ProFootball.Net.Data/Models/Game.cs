using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Game
{
    public int Id { get; set; }

    public int SeasonId { get; set; }

    public int Week { get; set; }

    public string GuestName { get; set; } = null!;

    public int GuestScore { get; set; }

    public string HostName { get; set; } = null!;

    public int HostScore { get; set; }

    public bool IsPlayoff { get; set; }

    public string? Notes { get; set; }

    [ValidateNever]
    public virtual Season SeasonIdNavigation { get; set; } = null!;
}
