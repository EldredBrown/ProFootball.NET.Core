using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy
{
    public class SubtractGameStrategy : ProcessGameStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractGameStrategy"/> class.
        /// </summary>
        /// <param name="teamRepository">The repository by which team data will be accessed.</param>
        /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
        public SubtractGameStrategy(
            ITeamRepository teamRepository,
            ITeamSeasonRepository teamSeasonRepository
        )
            : base(teamRepository, teamSeasonRepository)
        { }

        protected override void UpdateGamesForTeamSeasons(TeamSeason? guestSeason, TeamSeason? hostSeason)
        {
            if (guestSeason is not null)
            {
                guestSeason.Games--;
            }

            if (hostSeason is not null)
            {
                hostSeason.Games--;
            }
        }

        protected override void UpdateWinsLossesAndTiesForTeamSeasons(
            TeamSeason? guestSeason, TeamSeason? hostSeason, Game game)
        {
            if (game.IsTie)
            {
                if (guestSeason is not null)
                {
                    guestSeason.Ties--;
                }

                if (hostSeason is not null)
                {
                    hostSeason.Ties--;
                }
            }
            else
            {
                var (winnerSeason, loserSeason) = game.WinnerName == game.GuestName
                    ? (guestSeason, hostSeason)
                    : (hostSeason, guestSeason);

                if (winnerSeason is not null)
                {
                    winnerSeason.Wins--;
                }
                if (loserSeason is not null)
                {
                    loserSeason.Losses--;
                }
            }
        }

        protected override void EditScoringDataForTeamSeason(TeamSeason? teamSeason, int teamScore, int opponentScore)
        {
            if (teamSeason is null)
            {
                return;
            }

            teamSeason.PointsFor -= teamScore;
            teamSeason.PointsAgainst -= opponentScore;
            CalculateExpectedWinsAndLosses(teamSeason);
        }
    }
}
