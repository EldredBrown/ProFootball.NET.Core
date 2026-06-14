using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Services
{
    public interface IGamePredictorService
    {
        GameScorePrediction PredictGameScore(TeamSeason guestSeason, TeamSeason hostSeason);
    }
}
