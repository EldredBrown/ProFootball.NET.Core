using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Services
{
    public interface IGamePredictorService
    {
        (double?, double?) PredictGameScore(TeamSeason guestSeason, TeamSeason hostSeason);
    }
}
