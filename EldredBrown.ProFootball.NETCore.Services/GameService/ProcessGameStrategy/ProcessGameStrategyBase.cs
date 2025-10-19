using System;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.Utilities;

namespace EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy
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
            TeamSeasonDecorator? guestSeasonDecorator = null;
            if (!(guestSeason is null))
            {
                guestSeasonDecorator = new TeamSeasonDecorator(guestSeason);
            }

            var hostSeason =
                _teamSeasonRepository.GetTeamSeasonByTeamAndSeason(gameDecorator.HostName, seasonYear);
            TeamSeasonDecorator? hostSeasonDecorator = null;
            if (!(hostSeason is null))
            {
                hostSeasonDecorator = new TeamSeasonDecorator(hostSeason);
            }

            EditWinLossData(guestSeasonDecorator, hostSeasonDecorator, gameDecorator);
            EditScoringData(guestSeasonDecorator, hostSeasonDecorator, gameDecorator.GuestScore,
                gameDecorator.HostScore);
        }

        protected void EditWinLossData(TeamSeasonDecorator? guestSeasonDecorator,
            TeamSeasonDecorator? hostSeasonDecorator, IGameDecorator gameDecorator)
        {
            UpdateGamesForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator);
            UpdateWinsLossesAndTiesForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator, gameDecorator);
            UpdateWinningPercentageForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator);
        }

        protected virtual void UpdateWinsLossesAndTiesForTeamSeasons(TeamSeasonDecorator? guestSeasonDecorator,
            TeamSeasonDecorator? hostSeasonDecorator, IGameDecorator gameDecorator)
        {
            throw new NotImplementedException(
                nameof(UpdateWinsLossesAndTiesForTeamSeasons) + " must be implemented in a subclass.");
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

        protected async Task EditWinLossDataAsync(TeamSeasonDecorator? guestSeasonDecorator,
            TeamSeasonDecorator? hostSeasonDecorator, IGameDecorator gameDecorator)
        {
            UpdateGamesForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator);
            await UpdateWinsLossesAndTiesForTeamSeasonsAsync(guestSeasonDecorator, hostSeasonDecorator, gameDecorator);
            UpdateWinningPercentageForTeamSeasons(guestSeasonDecorator, hostSeasonDecorator);
        }

        protected virtual Task UpdateWinsLossesAndTiesForTeamSeasonsAsync(TeamSeasonDecorator? guestSeasonDecorator,
            TeamSeasonDecorator? hostSeasonDecorator, IGameDecorator gameDecorator)
        {
            throw new NotImplementedException(
                nameof(UpdateWinsLossesAndTiesForTeamSeasonsAsync) + " must be implemented in a subclass.");
        }

        protected virtual void UpdateGamesForTeamSeasons(TeamSeasonDecorator? guestSeasonDecorator,
            TeamSeasonDecorator? hostSeasonDecorator)
        {
            throw new NotImplementedException(
                nameof(UpdateGamesForTeamSeasons) + " must be implemented in a subclass.");
        }

        protected void UpdateWinningPercentageForTeamSeasons(TeamSeasonDecorator? guestSeasonDecorator,
            TeamSeasonDecorator? hostSeasonDecorator)
        {
            if (!(guestSeasonDecorator is null))
            {
                guestSeasonDecorator.CalculateWinningPercentage();
            }

            if (!(hostSeasonDecorator is null))
            {
                hostSeasonDecorator.CalculateWinningPercentage();
            }
        }

        protected void EditScoringData(TeamSeasonDecorator? guestSeasonDecorator,
            TeamSeasonDecorator? hostSeasonDecorator, int guestScore, int hostScore)
        {
            EditScoringDataForTeamSeason(guestSeasonDecorator, guestScore, hostScore);
            EditScoringDataForTeamSeason(hostSeasonDecorator, hostScore, guestScore);
        }

        protected virtual void EditScoringDataForTeamSeason(TeamSeasonDecorator? teamSeasonDecorator, int teamScore,
            int opponentScore)
        {
            throw new NotImplementedException(
                nameof(EditScoringDataForTeamSeason) + " must be implemented in a subclass.");
        }
    }
}
