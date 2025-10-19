using System.ComponentModel;

namespace EldredBrown.ProFootball.NETCore.Data.Entities
{
    /// <summary>
    /// Represents a game prediction.
    /// </summary>
    public class GamePrediction
    {
        /// <summary>
        /// Gets or sets the guest season year of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        [DisplayName("Guest Season")]
        public int GuestSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the guest name of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        [DisplayName("Guest Name")]
        public string GuestName { get; set; } = "";

        /// <summary>
        /// Gets or sets the guest score of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        [DisplayName("Guest Score")]
        public int GuestScore { get; set; }

        /// <summary>
        /// Gets or sets the host season year of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        [DisplayName("Host Season")]
        public int HostSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the host name of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        [DisplayName("Host Name")]
        public string HostName { get; set; } = "";

        /// <summary>
        /// Gets or sets the host score of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        [DisplayName("Host Score")]
        public int HostScore { get; set; }
    }
}
