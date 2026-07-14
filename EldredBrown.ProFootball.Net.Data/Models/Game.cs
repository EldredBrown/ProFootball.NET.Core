using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Game
{
    /// <summary>
    /// Creates a new instance of the <see cref="Game"/> class.
    /// </summary>
    /// <summary>
    public Game() { }

    /// <summary>
    /// Creates a copy of another instance of the <see cref="Game"/> class.
    /// </summary>
    /// <param name="game">The <see cref="Game"/> entity to copy.<</param>
    /// <summary>
    public Game(Game game)
    {
        Id = game.Id;
        SeasonYear = game.SeasonYear;
        LeagueId = game.LeagueId;
        Week = game.Week;
        GuestName = game.GuestName;
        GuestScore = game.GuestScore;
        HostName = game.HostName;
        HostScore = game.HostScore;
        IsPlayoff = game.IsPlayoff;
        Notes = game.Notes;
    }

    public int Id { get; set; }

    public int SeasonYear { get; set; }

    public int? LeagueId { get; set; }

    public int Week { get; set; }

    public string GuestName { get; set; } = null!;

    public int GuestScore { get; set; }

    public string HostName { get; set; } = null!;

    public int HostScore { get; set; }

    public bool IsPlayoff { get; set; }

    public string? Notes { get; set; }

    [ValidateNever]
    public virtual Season SeasonYearNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Association LeagueIdNavigation { get; set; } = null!;
}
