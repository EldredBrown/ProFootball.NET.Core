using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy
{
    public class AddGameStrategy : ProcessGameStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddGameStrategy"/> class.
        /// </summary>
        /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
        public AddGameStrategy(ITeamSeasonRepository teamSeasonRepository)
            : base(teamSeasonRepository)
        {
        }

        protected override void UpdateGamesForTeamSeasons(ITeamSeason? guestSeason, ITeamSeason? hostSeason)
        {
            if (guestSeason is not null)
            {
                guestSeason.Games++;
            }

            if (hostSeason is not null)
            {
                hostSeason.Games++;
            }
        }

        protected override void UpdateWinsLossesAndTiesForTeamSeasons(
            ITeamSeason? guestSeason, ITeamSeason? hostSeason, Game game)
        {
            if (game.IsTie)
            {
                if (guestSeason is not null)
                {
                    guestSeason.Ties++;
                }

                if (hostSeason is not null)
                {
                    hostSeason.Ties++;
                }
            }
            else
            {
                var (winnerSeason, loserSeason) = game.WinnerName == game.GuestName
                    ? (guestSeason, hostSeason)
                    : (hostSeason, guestSeason);

                if (winnerSeason is not null)
                {
                    winnerSeason.Wins++;
                }
                if (loserSeason is not null)
                {
                    loserSeason.Losses++;
                }
            }
        }

        protected override void EditScoringDataForTeamSeason(ITeamSeason? teamSeason, int teamScore, int opponentScore)
        {
            if (teamSeason is null)
            {
                return;
            }

            teamSeason.PointsFor += teamScore;
            teamSeason.PointsAgainst += opponentScore;
            teamSeason.CalculateExpectedWinsAndLosses();
        }
    }
}
