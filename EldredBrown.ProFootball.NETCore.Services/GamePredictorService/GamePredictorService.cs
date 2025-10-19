using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Services
{
    public class GamePredictorService : IGamePredictorService
    {
        /// <summary>
        /// Calculates a predicted game score.
        /// </summary>
        /// <param name="guestSeason">A <see cref="TeamSeason"/> object representing the guest's season data.</param>
        /// <param name="hostSeason">A <see cref="TeamSeason"/> object representing the host's season data.</param>
        /// <returns></returns>
        public (double?, double?) PredictGameScore(TeamSeason guestSeason, TeamSeason hostSeason)
        {
            var guestScore = (guestSeason.OffensiveFactor * hostSeason.DefensiveAverage +
                hostSeason.DefensiveFactor * guestSeason.OffensiveAverage) / 2d;
            var hostScore = (hostSeason.OffensiveFactor * guestSeason.DefensiveAverage +
                guestSeason.DefensiveFactor * hostSeason.OffensiveAverage) / 2d;

            return (guestScore, hostScore);
        }
    }
}
