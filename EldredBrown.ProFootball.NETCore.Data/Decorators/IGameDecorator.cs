using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Decorators
{
    public interface IGameDecorator
    {
        /// <summary>
        /// Gets or sets the ID of the wrapped <see cref="Game"/> entity.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="Game"/> entity's season.
        /// </summary>
        int SeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the ID of the wrapped <see cref="Game"/> entity's week.
        /// </summary>
        int Week { get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="Game"/> entity's guest.
        /// </summary>
        string GuestName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="Game"/> entity's guest.
        /// </summary>
        int GuestScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="Game"/> entity's host.
        /// </summary>
        string HostName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="Game"/> entity's host.
        /// </summary>
        int HostScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="Game"/> entity's winner.
        /// </summary>
        string? WinnerName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="Game"/> entity's winner.
        /// </summary>
        int? WinnerScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="Game"/> entity's loser.
        /// </summary>
        string? LoserName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="Game"/> entity's loser.
        /// </summary>
        int? LoserScore { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the wrapped <see cref="Game"/> entity is a playoff game.
        /// </summary>
        bool IsPlayoff { get; set; }

        /// <summary>
        /// Gets or sets any notes for the wrapped <see cref="Game"/> entity.
        /// </summary>
        string? Notes { get; set; }

        /// <summary>
        /// Decides the winner and loser of the wrapped <see cref="Game"/> entity.
        /// </summary>
        void DecideWinnerAndLoser();

        /// <summary>
        /// Edits the wrapped <see cref="Game"/> entity with data from another <see cref="Game"/> entity.
        /// </summary>
        /// <param name="srcGame">The <see cref="Game"/> entity from which data will be copied.</param>
        void Edit(IGameDecorator srcGame);

        /// <summary>
        /// Checks to see if the wrapped <see cref="Game"/> entity is a tie.
        /// </summary>
        /// <returns>True if the <see cref="Game"/> is a tie, otherwise false.</returns>
        bool IsTie();
    }
}