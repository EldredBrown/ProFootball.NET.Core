using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Services
{
    public interface IGamePredictorService
    {
        (decimal?, decimal?) PredictGameScore(TeamSeason guestSeason, TeamSeason hostSeason);
    }
}
