using System.Threading.Tasks;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    public interface IGameViewModelMapper
    {
        GameViewModel MapGameToViewModel(EldredBrown.ProFootball.Net.Data.Models.Game game);

        Task<EldredBrown.ProFootball.Net.Data.Models.Game> MapViewModelToGame(
            GameViewModel gameViewModel
        );
    }
}
