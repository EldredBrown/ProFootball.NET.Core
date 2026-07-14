using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace EldredBrown.ProFootball.Net.Data.Models
{
    /// <summary>
    /// Represents a game prediction.
    /// </summary>
    public class GamePrediction
    {
        /// <summary>
        /// Gets or sets the guest season year of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        public int GuestSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the guest name of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        public string GuestName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the guest score of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        public int? GuestScore { get; set; }

        /// <summary>
        /// Gets or sets the host season year of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        public int HostSeasonYear { get; set; }

        /// <summary>
        /// Gets or sets the host name of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the host score of the current <see cref="GamePrediction"/> entity.
        /// </summary>
        public int? HostScore { get; set; }

        [ValidateNever]
        public virtual Season GuestSeasonYearNavigation { get; set; } = null!;

        [ValidateNever]
        public virtual Season HostSeasonYearNavigation { get; set; } = null!;
    }
}
