using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.ProcessGameStrategy;
using EldredBrown.ProFootball.Net.Services.Utilities;

namespace EldredBrown.ProFootball.Net.Services
{
    /// <summary>
    /// Service to handle the more complicated actions of adding, editing, or deleting games in the data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GameService"/> class.
    /// </remarks>
    /// <param name="gameRepository">The repository by which game data will be accessed.</param>
    /// <param name="processGameStrategyFactory">The factory that will initialize the needed <see cref="ProcessGameStrategyBase"/> subclass.</param>
    public class GameService(IGameRepository gameRepository, IProcessGameStrategyFactory processGameStrategyFactory)
        : IGameService
    {
        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity to add to the data store.</param>
        public void AddGame(Game newGame)
        {
            Guard.ThrowIfNull(newGame, $"{GetType()}.{nameof(AddGame)}: {nameof(newGame)}");

            //await ValidateTeamsInNewGameAsync(newGame);

            gameRepository.Add(newGame);

            EditTeamSeasons(Direction.Up, newGame);
        }

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store asynchronously.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity to add to the data store.</param>
        public async Task AddGameAsync(Game newGame)
        {
            Guard.ThrowIfNull(newGame, $"{GetType()}.{nameof(AddGameAsync)}: {nameof(newGame)}");

            //await ValidateTeamsInNewGameAsync(newGame);

            await gameRepository.AddAsync(newGame);
            await EditTeamSeasonsAsync(Direction.Up, newGame);
        }

        /// <summary>
        /// Edits a <see cref="Game"/> entity in the data store.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity containing data to add to the data store.</param>
        /// <param name="oldGame">The <see cref="Game"/> entity containing data to remove from the data store.</param>
        public void EditGame(Game newGame, Game oldGame)
        {
            Guard.ThrowIfNull(newGame, $"{GetType()}.{nameof(EditGame)}: {nameof(newGame)}");
            Guard.ThrowIfNull(oldGame, $"{GetType()}.{nameof(EditGame)}: {nameof(oldGame)}");

            //await ValidateTeamsInNewGameAsync(newGame);

            var selectedGame = gameRepository.GetGame(newGame.Id);
            if (selectedGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(EditGame)}: The selected Game entity could not be found.");
            }

            selectedGame.Edit(newGame);

            gameRepository.Update(selectedGame);

            EditTeamSeasons(Direction.Down, oldGame);
            EditTeamSeasons(Direction.Up, newGame);
        }

        /// <summary>
        /// Edits a <see cref="Game"/> entity in the data store asynchronously.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity containing data to add to the data store.</param>
        /// <param name="oldGame">The <see cref="Game"/> entity containing data to remove from the data store.</param>
        public async Task EditGameAsync(Game newGame, Game oldGame)
        {
            Guard.ThrowIfNull(newGame, $"{GetType()}.{nameof(EditGameAsync)}: {nameof(newGame)}");
            Guard.ThrowIfNull(oldGame, $"{GetType()}.{nameof(EditGameAsync)}: {nameof(oldGame)}");

            //await ValidateTeamsInNewGameAsync(newGame);

            var selectedGame = await gameRepository.GetGameAsync(newGame.Id);
            if (selectedGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(EditGameAsync)}: The selected Game entity could not be found.");
            }

            selectedGame.Edit(newGame);

            gameRepository.Update(selectedGame);

            await EditTeamSeasonsAsync(Direction.Down, oldGame);
            await EditTeamSeasonsAsync(Direction.Up, newGame);
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        public void DeleteGame(int id)
        {
            var oldGame = gameRepository.GetGame(id);
            if (oldGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(DeleteGame)}: A Game entity with Id={id} could not be found.");
            }

            EditTeamSeasons(Direction.Down, oldGame);

            gameRepository.Delete(id);
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store asynchronously.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        public async Task DeleteGameAsync(int id)
        {
            var oldGame = await gameRepository.GetGameAsync(id);
            if (oldGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(DeleteGameAsync)}: A Game entity with Id={id} could not be found.");
            }

            await EditTeamSeasonsAsync(Direction.Down, oldGame);
            await gameRepository.DeleteAsync(id);
        }

        private void EditTeamSeasons(Direction direction, Game game)
        {
            var processGameStrategy = processGameStrategyFactory.CreateStrategy(direction);
            processGameStrategy.ProcessGame(game);
        }

        private async Task EditTeamSeasonsAsync(Direction direction, Game game)
        {
            var processGameStrategy = processGameStrategyFactory.CreateStrategy(direction);
            await processGameStrategy.ProcessGameAsync(game);
        }

        //private void ValidateTeamsInNewGame(Game newGame)
        //{
        //    foreach (var name in new[] { newGame.GuestName, newGame.HostName })
        //    {
        //        if (_teamSeasonRepository.GetTeamSeasonByTeamAndSeason(newGame.GuestId, newGame.SeasonYear) is null)
        //        {
        //            throw new EntityNotFoundException($"No team season found for {name} in year {newGame.SeasonYear}");
        //        }
        //    }
        //}

        //private async Task ValidateTeamsInNewGameAsync(Game newGame)
        //{
        //    foreach (var name in new[] { newGame.GuestName, newGame.HostName })
        //    {
        //        if (await _teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(name, newGame.SeasonYear) is null)
        //        {
        //            throw new EntityNotFoundException($"No team season found for {name} in year {newGame.SeasonYear}");
        //        }
        //    }
        //}
    }
}
