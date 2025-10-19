using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.Exceptions;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy;
using EldredBrown.ProFootball.NETCore.Services.Utilities;

namespace EldredBrown.ProFootball.NETCore.Services.GameServiceNS
{
    /// <summary>
    /// Service to handle the more complicated actions of adding, editing, or deleting games in the data store.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ISharedRepository _sharedRepository;
        private readonly IProcessGameStrategyFactory _processGameStrategyFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="gameRepository">The repository by which game data will be accessed.</param>
        /// <param name="sharedRepository">The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// <param name="processGameStrategyFactory">The factory that will initialize the needed <see cref="ProcessGameStrategyBase"/> subclass.</param>
        public GameService(IGameRepository gameRepository, ISharedRepository sharedRepository,
            IProcessGameStrategyFactory processGameStrategyFactory)
        {
            _gameRepository = gameRepository;
            _sharedRepository = sharedRepository;
            _processGameStrategyFactory = processGameStrategyFactory;
        }

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity to add to the data store.</param>
        public void AddGame(Game newGame)
        {
            Guard.ThrowIfNull(newGame, $"{GetType()}.{nameof(AddGame)}: {nameof(newGame)}");

            var newGameDecorator = new GameDecorator(newGame);
            newGameDecorator.DecideWinnerAndLoser();

            _gameRepository.Add(newGame);

            EditTeams(Direction.Up, newGameDecorator);

            _sharedRepository.SaveChanges();
        }

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store asynchronously.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity to add to the data store.</param>
        public async Task AddGameAsync(Game newGame)
        {
            Guard.ThrowIfNull(newGame, $"{GetType()}.{nameof(AddGameAsync)}: {nameof(newGame)}");

            var newGameDecorator = new GameDecorator(newGame);
            newGameDecorator.DecideWinnerAndLoser();

            await _gameRepository.AddAsync(newGame);

            await EditTeamsAsync(Direction.Up, newGameDecorator);

            await _sharedRepository.SaveChangesAsync();
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

            var selectedGame = _gameRepository.GetGame(oldGame.ID);
            if (selectedGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(EditGame)}: The selected Game entity could not be found.");
            }

            var newGameDecorator = new GameDecorator(newGame);
            newGameDecorator.DecideWinnerAndLoser();

            var selectedGameDecorator = new GameDecorator(selectedGame);
            selectedGameDecorator.Edit(newGameDecorator);

            _gameRepository.Update(selectedGame);

            var oldGameDecorator = new GameDecorator(oldGame);
            EditTeams(Direction.Down, oldGameDecorator);
            EditTeams(Direction.Up, newGameDecorator);

            _sharedRepository.SaveChanges();
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

            var selectedGame = await _gameRepository.GetGameAsync(newGame.ID);
            if (selectedGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(EditGameAsync)}: The selected Game entity could not be found.");
            }

            var newGameDecorator = new GameDecorator(newGame);
            newGameDecorator.DecideWinnerAndLoser();

            var selectedGameDecorator = new GameDecorator(selectedGame);
            selectedGameDecorator.Edit(newGameDecorator);

            _gameRepository.Update(selectedGame);

            var oldGameDecorator = new GameDecorator(oldGame);
            await EditTeamsAsync(Direction.Down, oldGameDecorator);
            await EditTeamsAsync(Direction.Up, newGameDecorator);

            await _sharedRepository.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
        public void DeleteGame(int id)
        {
            var oldGame = _gameRepository.GetGame(id);
            if (oldGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(DeleteGame)}: A Game entity with ID={id} could not be found.");
            }

            var oldGameDecorator = new GameDecorator(oldGame);
            EditTeams(Direction.Down, oldGameDecorator);
            _gameRepository.Delete(id);
            _sharedRepository.SaveChanges();
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store asynchronously.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
        public async Task DeleteGameAsync(int id)
        {
            var oldGame = await _gameRepository.GetGameAsync(id);
            if (oldGame is null)
            {
                throw new EntityNotFoundException(
                    $"{GetType()}.{nameof(DeleteGameAsync)}: A Game entity with ID={id} could not be found.");
            }

            var oldGameDecorator = new GameDecorator(oldGame);
            await EditTeamsAsync(Direction.Down, oldGameDecorator);
            await _gameRepository.DeleteAsync(id);
            await _sharedRepository.SaveChangesAsync();
        }

        private void EditTeams(Direction direction, IGameDecorator gameDecorator)
        {
            var processGameStrategy = _processGameStrategyFactory.CreateStrategy(direction);

            // TODO - 2020-09-25: Implement ObjectNotFoundException class so it can be caught and used here.
            //try
            //{
            processGameStrategy.ProcessGame(gameDecorator);
            //}
            //catch (ObjectNotFoundException ex)
            //{
            //    Log.Error("ObjectNotFoundException in GamesService.EditTeams: " + ex.Message);
            //    _sharedService.ShowExceptionMessage(ex, "ObjectNotFoundException");
            //}
        }

        private async Task EditTeamsAsync(Direction direction, GameDecorator gameDecorator)
        {
            var processGameStrategy = _processGameStrategyFactory.CreateStrategy(direction);

            // TODO - 2020-09-25: Implement ObjectNotFoundException class so it can be caught and used here.
            //try
            //{
            await processGameStrategy.ProcessGameAsync(gameDecorator);
            //}
            //catch (ObjectNotFoundException ex)
            //{
            //    Log.Error("ObjectNotFoundException in GamesService.EditTeams: " + ex.Message);
            //    _sharedService.ShowExceptionMessage(ex, "ObjectNotFoundException");
            //}
        }
    }
}
