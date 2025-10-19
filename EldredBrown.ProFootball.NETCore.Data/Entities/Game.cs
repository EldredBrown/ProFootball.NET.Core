using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a pro football game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="Game"/> entity.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="Game"/> entity's season.
        /// </summary>
        [DisplayName("Season")]
        [Required(ErrorMessage = "Please enter a season.")]
        [Range(0, int.MaxValue)]
        public int SeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the ID of the current <see cref="Game"/> entity's week.
        /// </summary>
        [DisplayName("Week")]
        [Required(ErrorMessage = "Please enter a week.")]
        [Range(0, int.MaxValue)]
        public int Week { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="Game"/> entity's guest.
        /// </summary>
        [DisplayName("Guest")]
        [Required(ErrorMessage = "Please enter a guest.")]
        public string GuestName { get; set; } = "";

        /// <summary>
        /// Gets or sets the points scored by the current <see cref="Game"/> entity's guest.
        /// </summary>
        [DisplayName("Guest Score")]
        [Required(ErrorMessage = "Please enter the guest's score.")]
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int GuestScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="Game"/> entity's host.
        /// </summary>
        [DisplayName("Host")]
        [Required(ErrorMessage = "Please enter a host.")]
        public string HostName { get; set; } = "";

        /// <summary>
        /// Gets or sets the points scored by the current <see cref="Game"/> entity's host.
        /// </summary>
        [DisplayName("Host Score")]
        [Required(ErrorMessage = "Please enter the host's score.")]
        [Range(0, int.MaxValue)]
        [DefaultValue(0)]
        public int HostScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="Game"/> entity's winner.
        /// </summary>
        [DisplayName("Winner")]
        public string? WinnerName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the current <see cref="Game"/> entity's winner.
        /// </summary>
        [DisplayName("Winner Score")]
        [Range(0, int.MaxValue)]
        public int? WinnerScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="Game"/> entity's loser.
        /// </summary>
        [DisplayName("Loser")]
        public string? LoserName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the current <see cref="Game"/> entity's loser.
        /// </summary>
        [DisplayName("Loser Score")]
        [Range(0, int.MaxValue)]
        public int? LoserScore { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the current <see cref="Game"/> entity is a playoff game.
        /// </summary>
        [DisplayName("Playoff Game?")]
        [DefaultValue(false)]
        public bool IsPlayoff { get; set; }

        /// <summary>
        /// Gets or sets any notes for the current <see cref="Game"/> entity.
        /// </summary>
        public string? Notes { get; set; }
    }
}
