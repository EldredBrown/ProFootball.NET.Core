using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Models
{
    public class GameModel
    {
        /// <summary>
        /// Gets or sets the ID of the current <see cref="GameModel"/> object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the year of the current <see cref="GameModel"/> object's season.
        /// </summary>
        [Required]
        public int SeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the ID of the current <see cref="GameModel"/> object's week.
        /// </summary>
        [Required]
        public int Week { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="GameModel"/> object's guest.
        /// </summary>
        [Required]
        public string GuestName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the current <see cref="GameModel"/> object's guest.
        /// </summary>
        [Required]
        public int GuestScore { get; set; }

        /// <summary>
        /// Gets or sets the name of the current <see cref="GameModel"/> object's host.
        /// </summary>
        [Required]
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the points scored by the current <see cref="GameModel"/> object's host.
        /// </summary>
        [Required]
        public int HostScore { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the current <see cref="GameModel"/> object represents a playoff game.
        /// </summary>
        public bool IsPlayoffGame { get; set; }

        /// <summary>
        /// Gets or sets any notes for the current <see cref="GameModel"/> object.
        /// </summary>
        public string Notes { get; set; }
    }
}
