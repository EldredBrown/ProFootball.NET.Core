using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    /// <summary>
    /// A class that maps game data to game view models.
    /// </summary>
    public class GameViewModelMapper(IAssociationRepository associationRepository) : IGameViewModelMapper
    {
        internal readonly IAssociationRepository _associationRepository = associationRepository;

        public GameViewModel MapGameToViewModel(EldredBrown.ProFootball.Net.Data.Models.Game game)
        {
            return new GameViewModel { Game = game };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.Game> MapViewModelToGame(GameViewModel gameViewModel)
        {
            var game = gameViewModel.Game;

            var league = await _associationRepository.GetAssociationByShortNameAsync(gameViewModel.LeagueName);
            game.LeagueId = !gameViewModel.LeagueName.IsNullOrEmpty() && league is null ? -1 : league?.Id;

            return game;
        }
    }
}
