using System;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Decorators;
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
        public virtual void ProcessGame(IGameDecorator gameDecorator)
        {
            Guard.ThrowIfNull(gameDecorator, $"{GetType()}.{nameof(ProcessGame)}: {nameof(gameDecorator)}");

            var seasonYear = gameDecorator.SeasonYear;

            var guestSeason =
                _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.GuestName, seasonYear);
            ITeamSeasonDecorator? guestSeasonDecorator = null;
            if (!(guestSeason is null))
            {
                guestSeasonDecorator = new TeamSeasonDecorator(guestSeason);
            }

            var hostSeason =
                _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.HostName, seasonYear);
            ITeamSeasonDecorator? hostSeasonDecorator = null;
            if (!(hostSeason is null))
            {
                hostSeasonDecorator = new TeamSeasonDecorator(hostSeason);
            }

            EditWinLossData(guestSeasonDecorator, hostSeasonDecorator, gameDecorator);
            EditScoringData(guestSeasonDecorator, hostSeasonDecorator, gameDecorator.GuestScore,
                gameDecorator.HostScore);
        }

        /// <summary>
        /// Processes a <see cref="Game"/> entity into the Teams data store asynchronously.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public virtual async Task ProcessGameAsync(IGameDecorator gameDecorator)
        {
            Guard.ThrowIfNull(gameDecorator, $"{GetType()}.{nameof(ProcessGameAsync)}: {nameof(gameDecorator)}");

            var seasonYear = gameDecorator.SeasonYear;

            var guestSeason =
                await _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.GuestName, seasonYear);
            TeamSeasonDecorator? guestSeasonDecorator = null;
            if (!(guestSeason is null))
            {
                guestSeasonDecorator = new TeamSeasonDecorator(guestSeason);
            }

            var hostSeason =
                await _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(gameDecorator.HostName, seasonYear);
            TeamSeasonDecorator? hostSeasonDecorator = null;
            if (!(hostSeason is null))
            {
                hostSeasonDecorator = new TeamSeasonDecorator(hostSeason);
            }

            await EditWinLossDataAsync(guestSeasonDecorator, hostSeasonDecorator, gameDecorator);
            EditScoringData(guestSeasonDecorator, hostSeasonDecorator, gameDecorator.GuestScore,
                gameDecorator.HostScore);
        }

        protected void EditWinLossData(
            ITeamSeasonDecorator? guestSeasonDecorator, ITeamSeasonDecorator? hostSeasonDecorator,
            IGameDecorator gameDecorator)
        {
            UpdateGamesForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator);
            UpdateWinsLossesAndTiesForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator, gameDecorator);
        }

        protected async Task EditWinLossDataAsync(
            ITeamSeasonDecorator? guestSeasonDecorator, ITeamSeasonDecorator? hostSeasonDecorator,
            IGameDecorator gameDecorator)
        {
            UpdateGamesForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator);
            await UpdateWinsLossesAndTiesForTeamSeasonsAsync(guestSeasonDecorator, hostSeasonDecorator, gameDecorator);
        }

        protected virtual void UpdateGamesForTeamSeasons(
            ITeamSeasonDecorator? guestSeasonDecorator, ITeamSeasonDecorator? hostSeasonDecorator)
        {
            throw new NotImplementedException(
                nameof(UpdateGamesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected virtual void UpdateWinsLossesAndTiesForTeamSeasons(
            ITeamSeasonDecorator? guestSeasonDecorator, ITeamSeasonDecorator? hostSeasonDecorator,
            IGameDecorator gameDecorator)
        {
            throw new NotImplementedException(
                nameof(UpdateWinsLossesAndTiesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected virtual Task UpdateWinsLossesAndTiesForTeamSeasonsAsync(
            ITeamSeasonDecorator? guestSeasonDecorator, ITeamSeasonDecorator? hostSeasonDecorator,
            IGameDecorator gameDecorator)
        {
            throw new NotImplementedException(
                nameof(UpdateWinsLossesAndTiesForTeamSeasonsAsync) + " must be implemented in a subclass.");
        }

        protected void EditScoringData(
            ITeamSeasonDecorator? guestSeasonDecorator, ITeamSeasonDecorator? hostSeasonDecorator,
            int guestScore, int hostScore)
        {
            EditScoringDataForTeamSeason(guestSeasonDecorator, guestScore, hostScore);
            EditScoringDataForTeamSeason(hostSeasonDecorator, hostScore, guestScore);
        }

        protected virtual void EditScoringDataForTeamSeason(
            ITeamSeasonDecorator? teamSeasonDecorator, int teamScore, int opponentScore)
        {
            throw new NotImplementedException(
                nameof(EditScoringDataForTeamSeason) + " must be implemented in a subclass.");
        }
    }
}
