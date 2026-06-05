using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    /// <summary>
    /// A class that maps game data to game view models.
    /// </summary>
    public class GameViewModelMapper : IGameViewModelMapper
    {
        private readonly ISeasonRepository _seasonRepository;

        public GameViewModelMapper(ISeasonRepository seasonRepository)
        {
            _seasonRepository = seasonRepository;
        }

        public GameViewModel MapGameToViewModel(EldredBrown.ProFootball.Net.Data.Models.Game game)
        {
            return new GameViewModel { Game = game };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.Game> MapViewModelToGame(GameViewModel gameViewModel)
        {
            var game = gameViewModel.Game;

            var season = await _seasonRepository.GetSeasonAsync(gameViewModel.SeasonYear);
            game.SeasonId = season is not null ? season.Id : -1;

            return game;
        }
    }
}
