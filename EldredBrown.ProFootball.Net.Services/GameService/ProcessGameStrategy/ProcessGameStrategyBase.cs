using System;
using System.Linq;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Decorators;
using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.Utilities;

namespace EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy
{
    public class ProcessGameStrategyBase
    {
        protected readonly ITeamSeasonRepository _teamSeasonRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessGameStrategyBase"/> class.
        /// </summary>
        /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
        public ProcessGameStrategyBase(ITeamSeasonRepository teamSeasonRepository)
        {
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

            var seasonYear = game.SeasonYear;
            var teamSeasons = _teamSeasonRepository.GetTeamSeasonsBySeason(seasonYear);

            //var guestSeason = teamSeasons.FirstOrDefault(ts => ts.TeamName == game.GuestName);
            //if (guestSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.GuestName}' and season year {seasonYear}.");
            //}

            //var hostSeason = teamSeasons.FirstOrDefault(ts => ts.TeamName == game.HostName);
            //if (hostSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.HostName}' and season year {seasonYear}.");
            //}

            //EditWinLossData(guestSeason, hostSeason, game);
            //EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            //_teamSeasonRepository.Update(guestSeason);
            //_teamSeasonRepository.Update(hostSeason);
        }

        /// <summary>
        /// Processes a <see cref="Game"/> entity into the Teams data store asynchronously.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public virtual async Task ProcessGameAsync(Game game)
        {
            Guard.ThrowIfNull(game, $"{GetType()}.{nameof(ProcessGameAsync)}: {nameof(game)}");

            var seasonYear = game.SeasonYear;
            var teamSeasons = await _teamSeasonRepository.GetTeamSeasonsBySeasonAsync(seasonYear);

            //var guestSeason = teamSeasons.FirstOrDefault(ts => ts.TeamName == game.GuestName);
            //if (guestSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.GuestName}' and season year {seasonYear}.");
            //}

            //var hostSeason = teamSeasons.FirstOrDefault(ts => ts.TeamName == game.HostName);
            //if (hostSeason is null)
            //{
            //    throw new EntityNotFoundException(
            //        $"No TeamSeason entity found for team '{game.HostName}' and season year {seasonYear}.");
            //}

            //EditWinLossData(guestSeason, hostSeason, game);
            //EditScoringData(guestSeason, hostSeason, game.GuestScore, game.HostScore);

            //_teamSeasonRepository.Update(guestSeason);
            //_teamSeasonRepository.Update(hostSeason);
        }

        protected void EditWinLossData(ITeamSeason? guestSeason, ITeamSeason? hostSeason, Game game)
        {
            UpdateGamesForTeamSeasons(guestSeason, hostSeason);
            UpdateWinsLossesAndTiesForTeamSeasons(guestSeason, hostSeason, game);
        }

        protected virtual void UpdateGamesForTeamSeasons(ITeamSeason? guestSeason, ITeamSeason? hostSeason)
        {
            throw new NotImplementedException(
                nameof(UpdateGamesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected virtual void UpdateWinsLossesAndTiesForTeamSeasons(
            ITeamSeason? guestSeason, ITeamSeason? hostSeason, Game game)
        {
            throw new NotImplementedException(
                nameof(UpdateWinsLossesAndTiesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected void EditScoringData(ITeamSeason? guestSeason, ITeamSeason? hostSeason, int guestScore, int hostScore)
        {
            EditScoringDataForTeamSeason(guestSeason, guestScore, hostScore);
            EditScoringDataForTeamSeason(hostSeason, hostScore, guestScore);
        }

        protected virtual void EditScoringDataForTeamSeason(ITeamSeason? teamSeason, int teamScore, int opponentScore)
        {
            throw new NotImplementedException(
                nameof(EditScoringDataForTeamSeason) + " must be implemented in a subclass.");
        }
    }
}
