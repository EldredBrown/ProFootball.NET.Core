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
        public string? WinnerName
        {
            get
            {
                var outcome = GetOutcome();
                return outcome != null
                    ? outcome.WinnerName
                    : null;
            }
        }

        /// <summary>
        /// Gets the points scored by the wrapped <see cref="Game"/> entity's winner.
        /// </summary>
        [DisplayName("Winner Score")]
        public int? WinnerScore
        {
            get
            {
                var outcome = GetOutcome();
                return outcome != null
                    ? outcome.WinnerScore
                    : null;
            }
        }

        /// <summary>
        /// Gets the name of the wrapped <see cref="Game"/> entity's loser.
        /// </summary>
        [DisplayName("Loser")]
        public string? LoserName
        {
            get
            {
                var outcome = GetOutcome();
                return outcome != null
                    ? outcome.LoserName
                    : null;
            }
        }

        /// <summary>
        /// Gets the points scored by the wrapped <see cref="Game"/> entity's loser.
        /// </summary>
        [DisplayName("Loser Score")]
        public int? LoserScore
        {
            get
            {
                var outcome = GetOutcome();
                return outcome != null
                    ? outcome.LoserScore
                    : null;
            }
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

        /// <summary>
        /// Checks to see if the wrapped <see cref="Game"/> entity is a tie.
        /// </summary>
        /// <returns>True if the <see cref="Game"/> is a tie, otherwise false.</returns>
        public bool IsTie
        {
            get
            {
                return _game.GuestScore == _game.HostScore;
            }
        }

        /// <summary>
        /// Edits the wrapped <see cref="Game"/> entity with data from another <see cref="Game"/> entity.
        /// </summary>
        /// <param name="srcGame">The <see cref="Game"/> entity from which data will be copied.</param>
        public void Edit(IGameDecorator srcGame)
        {
            _game.Week = srcGame.Week;
            _game.GuestName = srcGame.GuestName;
            _game.GuestScore = srcGame.GuestScore;
            _game.HostName = srcGame.HostName;
            _game.HostScore = srcGame.HostScore;
            _game.IsPlayoff = srcGame.IsPlayoff;
            _game.Notes = srcGame.Notes;
        }

        // Using a record for a clean tuple-like return type
        private record Outcome(string WinnerName, int WinnerScore, string LoserName, int LoserScore);

        private Outcome? GetOutcome()
        {
            if (IsTie)
            {
                return null;
            }

            return GuestScore > HostScore
                ? new Outcome(GuestName, GuestScore, HostName, HostScore)
                : new Outcome(HostName, HostScore, GuestName, GuestScore);
        }
    }
}
