namespace EldredBrown.ProFootball.Net.Data.Models
{
    public partial class Game
    {
        /// <summary>
        /// Checks to see if this <see cref="Game"/> entity is a tie.
        /// </summary>
        /// <returns>True if the <see cref="Game"/> is a tie, otherwise false.</returns>
        public bool IsTie
        {
            get
            {
                return GuestScore == HostScore;
            }
        }

        /// <summary>
        /// Gets the name of this <see cref="Game"/> entity's winner.
        /// </summary>
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
        /// Gets the points scored by this <see cref="Game"/> entity's winner.
        /// </summary>
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
        /// Gets the name of this <see cref="Game"/> entity's loser.
        /// </summary>
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
        /// Gets the points scored by this <see cref="Game"/> entity's loser.
        /// </summary>
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
        /// Edits this <see cref="Game"/> entity with data from another <see cref="Game"/> entity.
        /// </summary>
        /// <param name="srcGame">The <see cref="Game"/> entity from which data will be copied.</param>
        public void Edit(Game srcGame)
        {
            Week = srcGame.Week;
            GuestName = srcGame.GuestName;
            GuestScore = srcGame.GuestScore;
            HostName = srcGame.HostName;
            HostScore = srcGame.HostScore;
            IsPlayoff = srcGame.IsPlayoff;
            Notes = srcGame.Notes;
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
