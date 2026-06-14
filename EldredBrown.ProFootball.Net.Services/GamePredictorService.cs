using System;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Services
{
    public struct GameScorePrediction
    {
        public int? GuestScore { get; set; }
        public int? HostScore { get; set; }
    }

    public class GamePredictorService : IGamePredictorService
    {
        /// <summary>
        /// Calculates a predicted game score.
        /// </summary>
        /// <param name="guestSeason">A <see cref="ITeamSeason"/> object representing the guest's season data.</param>
        /// <param name="hostSeason">A <see cref="ITeamSeason"/> object representing the host's season data.</param>
        /// <returns></returns>
        public GameScorePrediction PredictGameScore(TeamSeason guestSeason, TeamSeason hostSeason)
        {
            var guestScore = PredictScore(guestSeason, hostSeason);
            var hostScore = PredictScore(hostSeason, guestSeason);

            return new GameScorePrediction {
                GuestScore = guestScore,
                HostScore = hostScore
            };
        }

        private int? PredictScore(TeamSeason offensiveTeam, TeamSeason defensiveTeam)
        {
            return (int?)Math.Round((offensiveTeam.OffensiveFactor.Value * defensiveTeam.DefensiveAverage.Value +
                defensiveTeam.DefensiveFactor.Value * offensiveTeam.OffensiveAverage.Value) / 2m, 0);
        }
    }
}
