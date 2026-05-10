namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    /// <summary>
    /// Represents the model for a game details view.
    /// </summary>
    public class GameDetailsViewModel : IGameDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the game of the current view model.
        /// </summary>
        public EldredBrown.ProFootball.Net.Data.Models.Game Game { get; set; }
    }
}
