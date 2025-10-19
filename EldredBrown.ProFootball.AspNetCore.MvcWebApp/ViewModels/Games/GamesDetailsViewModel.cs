using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Games
{
    /// <summary>
    /// Represents the model for a game details view.
    /// </summary>
    public class GamesDetailsViewModel : IGamesDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the game of the current view model.
        /// </summary>
        public Game Game { get; set; }
    }
}
