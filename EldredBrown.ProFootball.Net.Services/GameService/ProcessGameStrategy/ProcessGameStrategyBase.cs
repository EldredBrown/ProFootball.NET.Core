using System;
using System.Linq;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.Utilities;

namespace EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy
{
    public class ProcessGameStrategyBase
    {
        protected readonly ITeamRepository _teamRepository;
        protected readonly ITeamSeasonRepository _teamSeasonRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessGameStrategyBase"/> class.
        /// </summary>
        /// <param name="teamRepository">The repository by which team data will be accessed.</param>
        /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
        public ProcessGameStrategyBase(ITeamRepository teamRepository, ITeamSeasonRepository teamSeasonRepository)
        {
            _teamRepository = teamRepository;
            _teamSeasonRepository = teamSeasonRepository;
        }

        /// <summary>
        /// Processes a <see cref="Game"/> entity into the Teams data store.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public virtual void ProcessGame(Game game)
        {
            Guard.ThrowIfNull(game, $"{GetType()}.{nameof(ProcessGame)}: {nameof(game)}");

            var seasonId = game.SeasonId;
            var teamSeasons = _teamSeasonRepository.GetTeamSeasonsBySeason(seasonId);

            // I am starting this app with the 1920 season, in which the inaugural APFA allowed member teams to play 
            // opponents from outside the association. Eventually, after the APFA became the NFL, the league would 
            // restrict games only to two member teams. I anticipate that, at that time, this app will need to verify 
            // that both teams in a game are members of the NFL, and I will need to uncomment the following code 
            // blocks. Until then, I need to keep the following blocks of code commented out so that non-member 
            // opponents are permitted.

            var guestSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeam(ts.TeamId)?.Name == game.GuestName);
            //if (guestSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.GuestName}' and season year {seasonId}.");
            //}

            var hostSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeam(ts.TeamId)?.Name == game.HostName);
            //if (hostSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.HostName}' and season year {seasonId}.");
            //}

            EditWinLossData(guestSeason, hostSeason, game);
            EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            _teamSeasonRepository.Update(guestSeason);
            _teamSeasonRepository.Update(hostSeason);
        }

        /// <summary>
        /// Processes a <see cref="Game"/> entity into the Teams data store asynchronously.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public virtual async Task ProcessGameAsync(Game game)
        {
            Guard.ThrowIfNull(game, $"{GetType()}.{nameof(ProcessGameAsync)}: {nameof(game)}");

            var seasonId = game.SeasonId;
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonId);

            // I am starting this app with the 1920 season, in which the inaugural APFA allowed member teams to play 
            // opponents from outside the association. Eventually, after the APFA became the NFL, the league would 
            // restrict games only to two member teams. I anticipate that, at that time, this app will need to verify 
            // that both teams in a game are members of the NFL, and I will need to uncomment the following code 
            // blocks. Until then, I need to keep the following blocks of code commented out so that non-member 
            // opponents are permitted.

            var guestSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeamAsync(ts.TeamId).Result?.Name == game.GuestName);
            //if (guestSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.GuestName}' and season year {seasonId}.");
            //}

            var hostSeason = teamSeasons.FirstOrDefault(
                ts => _teamRepository.GetTeamAsync(ts.TeamId).Result?.Name == game.HostName);
            //if (hostSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.HostName}' and season year {seasonId}.");
            //}

            EditWinLossData(guestSeason, hostSeason, game);
            EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            _teamSeasonRepository.Update(guestSeason);
            _teamSeasonRepository.Update(hostSeason);
        }

        /// <summary>
        /// Calculates and updates this <see cref="TeamSeason"/> model's expected wins and losses.
        /// <param name="teamSeason"/>
        /// </summary>
        protected void CalculateExpectedWinsAndLosses(TeamSeason teamSeason)
        {
            if (teamSeason.Games == 0)
            {
                teamSeason.ExpectedWins = 0m;
                teamSeason.ExpectedLosses = 0m;
                return;
            }

            var expPct = MathUtils.CalculateExpectedWinningPercentage(teamSeason.PointsFor, teamSeason.PointsAgainst);

            if (expPct.HasValue)
            {
                teamSeason.ExpectedWins = expPct.Value * teamSeason.Games;
                teamSeason.ExpectedLosses = (1m - expPct.Value) * teamSeason.Games;
            }
            else
            {
                teamSeason.ExpectedWins = 0;
                teamSeason.ExpectedLosses = 0;
            }
        }

        protected void EditWinLossData(TeamSeason? guestSeason, TeamSeason? hostSeason, Game game)
        {
            UpdateGamesForTeamSeasons(guestSeason, hostSeason);
            UpdateWinsLossesAndTiesForTeamSeasons(guestSeason, hostSeason, game);
        }

        protected virtual void UpdateGamesForTeamSeasons(TeamSeason? guestSeason, TeamSeason? hostSeason)
        {
            throw new NotImplementedException(
                nameof(UpdateGamesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected virtual void UpdateWinsLossesAndTiesForTeamSeasons(
            TeamSeason? guestSeason, TeamSeason? hostSeason, Game game)
        {
            throw new NotImplementedException(
                nameof(UpdateWinsLossesAndTiesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected void EditScoringData(TeamSeason? guestSeason, TeamSeason? hostSeason, int guestScore, int hostScore)
        {
            EditScoringDataForTeamSeason(guestSeason, guestScore, hostScore);
            EditScoringDataForTeamSeason(hostSeason, hostScore, guestScore);
        }

        protected virtual void EditScoringDataForTeamSeason(TeamSeason? teamSeason, int teamScore, int opponentScore)
        {
            throw new NotImplementedException(
                nameof(EditScoringDataForTeamSeason) + " must be implemented in a subclass.");
        }
    }
}
