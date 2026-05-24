using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Decorators
{
    public class GameDecorator : Game, IGameDecorator
    {
        private readonly Game _game;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameDecorator"/> class.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity that will be wrapped inside this object.</param>
        public GameDecorator(Game game)
        {
            _game = game;
        }

        /// <summary>
        /// Gets or sets the Id of the wrapped <see cref="Game"/> entity.
        /// </summary>
        public int Id
        {
            get { return _game.Id; }
            set { _game.Id = value; }
        }

        /// <summary>
        /// Gets or sets the year of the wrapped <see cref="Game"/> entity's season.
        /// </summary>
        [DisplayName("Season")]
        [Required(ErrorMessage = "Please enter a season.")]
        public new int SeasonYear
        {
            get { return _game.SeasonYear; }
            set { _game.SeasonYear = value; }
        }

        /// <summary>
        /// Gets or sets the Id of the wrapped <see cref="Game"/> entity's week.
        /// </summary>
        [DisplayName("Week")]
        [Required(ErrorMessage = "Please enter a week.")]
        public new int Week
        {
            get { return _game.Week; }
            set { _game.Week = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="Game"/> entity's guest.
        /// </summary>
        [DisplayName("Guest")]
        [Required(ErrorMessage = "Please enter a guest.")]
        public new string GuestName
        {
            get { return _game.GuestName; }
            set { _game.GuestName = value; }
        }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="Game"/> entity's guest.
        /// </summary>
        [DisplayName("Guest Score")]
        [Required(ErrorMessage = "Please enter the guest's score.")]
        public new int GuestScore
        {
            get { return _game.GuestScore; }
            set { _game.GuestScore = value; }
        }

        /// <summary>
        /// Gets or sets the name of the wrapped <see cref="Game"/> entity's host.
        /// </summary>
        [DisplayName("Host")]
        [Required(ErrorMessage = "Please enter a host.")]
        public new string HostName
        {
            get { return _game.HostName; }
            set { _game.HostName = value; }
        }

        /// <summary>
        /// Gets or sets the points scored by the wrapped <see cref="Game"/> entity's host.
        /// </summary>
        [DisplayName("Host Score")]
        [Required(ErrorMessage = "Please enter the host's score.")]
        public new int HostScore
        {
            get { return _game.HostScore; }
            set { _game.HostScore = value; }
        }

        /// <summary>
        /// Gets the name of the wrapped <see cref="Game"/> entity's winner.
        /// </summary>
        [DisplayName("Winner")]
        public new string? WinnerName
        {
            get { return _game.WinnerName; }
        }

        /// <summary>
        /// Gets the points scored by the wrapped <see cref="Game"/> entity's winner.
        /// </summary>
        [DisplayName("Winner Score")]
        public new int? WinnerScore
        {
            get { return _game.WinnerScore; }
        }

        /// <summary>
        /// Gets the name of the wrapped <see cref="Game"/> entity's loser.
        /// </summary>
        [DisplayName("Loser")]
        public new string? LoserName
        {
            get { return _game.LoserName; }
        }

        /// <summary>
        /// Gets the points scored by the wrapped <see cref="Game"/> entity's loser.
        /// </summary>
        [DisplayName("Loser Score")]
        public new int? LoserScore
        {
            get{ return _game.LoserScore; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the wrapped <see cref="Game"/> entity is a playoff game.
        /// </summary>
        [DisplayName("Playoff Game?")]
        [DefaultValue(false)]
        public new bool IsPlayoff
        {
            get { return _game.IsPlayoff; }
            set { _game.IsPlayoff = value; }
        }

        /// <summary>
        /// Gets or sets any notes for the wrapped <see cref="Game"/> entity.
        /// </summary>
        public new string? Notes
        {
            get { return _game.Notes; }
            set { _game.Notes = value; }
        }
    }
}
