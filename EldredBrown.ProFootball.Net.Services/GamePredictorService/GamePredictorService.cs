using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Services
{
    public class GamePredictorService : IGamePredictorService
    {
        /// <summary>
        /// Calculates a predicted game score.
        /// </summary>
        /// <param name="guestSeason">A <see cref="TeamSeason"/> object representing the guest's season data.</param>
        /// <param name="hostSeason">A <see cref="TeamSeason"/> object representing the host's season data.</param>
        /// <returns></returns>
        public (decimal?, decimal?) PredictGameScore(TeamSeason guestSeason, TeamSeason hostSeason)
        {
            var guestScore = (guestSeason.OffensiveFactor * hostSeason.DefensiveAverage +
                hostSeason.DefensiveFactor * guestSeason.OffensiveAverage) / 2m;
            var hostScore = (hostSeason.OffensiveFactor * guestSeason.DefensiveAverage +
                guestSeason.DefensiveFactor * hostSeason.OffensiveAverage) / 2m;

            return (guestScore, hostScore);
        }
    }
}
