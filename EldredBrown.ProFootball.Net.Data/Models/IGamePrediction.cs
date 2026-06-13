namespace EldredBrown.ProFootball.Net.Data.Models
{
    /// <summary>
    /// Represents a game prediction.
    /// </summary>
    public interface IGamePrediction
    {
        /// <summary>
        /// Gets or sets the guest season year of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        int GuestSeasonId { get; set; }

        /// <summary>
        /// Gets or sets the guest name of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        string GuestName { get; set; }

        /// <summary>
        /// Gets or sets the guest score of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        int? GuestScore { get; set; }

        /// <summary>
        /// Gets or sets the host season year of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        int HostSeasonId { get; set; }

        /// <summary>
        /// Gets or sets the host name of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        string HostName { get; set; }

        /// <summary>
        /// Gets or sets the host score of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        int? HostScore { get; set; }
    }
}
