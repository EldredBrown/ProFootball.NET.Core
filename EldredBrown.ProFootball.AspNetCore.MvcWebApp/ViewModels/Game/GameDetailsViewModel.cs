namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    /// <summary>
    /// Represents the model for a game details view.
    /// </summary>
    public class GameDetailsViewModel : IGameDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the game of the current <see cref="GameDetailsViewModel"/> object.
        /// </summary>
        public GameViewModel Game { get; set; }
    }
}
