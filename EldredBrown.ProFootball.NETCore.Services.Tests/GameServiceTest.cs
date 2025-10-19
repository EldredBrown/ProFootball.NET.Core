using System;
using System.Threading.Tasks;
using FakeItEasy;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.NETCore.Data.Decorators;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.Exceptions;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.NETCore.Services.Tests
{
    public class GameServiceTest
    {
        private readonly IGameRepository _gameRepository;
        private readonly ISharedRepository _sharedRepository;
        private readonly IProcessGameStrategyFactory _processGameStrategyFactory;
        private readonly GameService _testService;

        public GameServiceTest()
        {
            _gameRepository = A.Fake<IGameRepository>();
            _sharedRepository = A.Fake<ISharedRepository>();
            _processGameStrategyFactory = A.Fake<IProcessGameStrategyFactory>();
            _testService = new GameService(_gameRepository, _sharedRepository, _processGameStrategyFactory);
        }

        [Fact]
        public void AddGame_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Game? newGame = null;

            // Act
            var action = new Action(() => _testService.AddGame(newGame!));

            // Assert
            var ex = action.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{_testService.GetType()}.AddGame: newGame");
        }

        [Fact]
        public void AddGame_WhenNewGameArgIsNotNull_ShouldAddGameToRepository()
        {
            // Arrange
            var strategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).Returns(strategy);

            var newGame = new Game();

            // Act
            _testService.AddGame(newGame);

            // Assert
            A.CallTo(() => _gameRepository.Add(newGame)).MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChanges()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => strategy.ProcessGame(A<IGameDecorator>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddGameAsync_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Game? newGame = null;

            // Act
            var func = new Func<Task>(async () => await _testService.AddGameAsync(newGame!));

            // Assert
            var ex = await func.ShouldThrowAsync<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{_testService.GetType()}.AddGameAsync: newGame");
        }

        [Fact]
        public async Task AddGameAsync_WhenNewGameArgIsNotNull_ShouldAddGameToRepository()
        {
            // Arrange
            var strategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).Returns(strategy);

            var newGame = new Game();

            // Act
            await _testService.AddGameAsync(newGame);

            // Assert
            A.CallTo(() => _gameRepository.AddAsync(newGame)).MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => strategy.ProcessGameAsync(A<IGameDecorator>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EditGame_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Game? newGame = null;
            Game? oldGame = null;

            // Act
            var action = new Action(() => _testService.EditGame(newGame!, oldGame!));

            // Assert
            var ex = action.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{_testService.GetType()}.EditGame: newGame");
        }

        [Fact]
        public void EditGame_WhenOldGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var newGame = new Game();
            Game? oldGame = null;

            // Act
            var func = new Action(() => _testService.EditGame(newGame, oldGame!));

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{_testService.GetType()}.EditGame: oldGame");
        }

        [Fact]
        public void EditGame_WhenNewGameAndOldGameArgsAreNotNullAndSelectedGameIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            A.CallTo(() => _gameRepository.GetGame(A<int>.Ignored)).Returns<Game?>(null);

            // Act
            var newGame = new Game();
            var oldGame = new Game();
            var func = new Action(() => _testService.EditGame(newGame, oldGame));

            // Assert
            var ex = func.ShouldThrow<EntityNotFoundException>();
            ex.Message.ShouldBe<string>($"{_testService.GetType()}.EditGame: The selected Game entity could not be found.");
        }

        [Fact]
        public void EditGame_WhenNewGameAndOldGameArgsAreNotNullAndSelectedGameIsFound_ShouldEditGameInRepository()
        {
            // Arrange
            var selectedGame = new Game();
            A.CallTo(() => _gameRepository.GetGame(A<int>.Ignored)).Returns(selectedGame);

            var downStrategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).Returns(downStrategy);
            var upStrategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).Returns(upStrategy);

            // Act
            var newGame = new Game();
            var oldGame = new Game();
            _testService.EditGame(newGame, oldGame);

            // Assert
            A.CallTo(() => _gameRepository.GetGame(newGame.ID)).MustHaveHappened();
            A.CallTo(() => _gameRepository.Update(selectedGame)).MustHaveHappened();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => downStrategy.ProcessGame(A<IGameDecorator>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => upStrategy.ProcessGame(A<IGameDecorator>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _sharedRepository.SaveChanges()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditGameAsync_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Game? newGame = null;
            Game? oldGame = null;

            // Act
            var func = new Func<Task>(async () => await _testService.EditGameAsync(newGame!, oldGame!));

            // Assert
            var ex = await func.ShouldThrowAsync<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{_testService.GetType()}.EditGameAsync: newGame");
        }

        [Fact]
        public async Task EditGameAsync_WhenOldGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var newGame = new Game();
            Game? oldGame = null;

            // Act
            var func = new Func<Task>(async () => await _testService.EditGameAsync(newGame, oldGame!));

            // Assert
            var ex = await func.ShouldThrowAsync<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{_testService.GetType()}.EditGameAsync: oldGame");
        }

        [Fact]
        public async Task EditGameAsync_WhenNewGameAndOldGameArgsAreNotNullAndSelectedGameIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            A.CallTo(() => _gameRepository.GetGameAsync(A<int>.Ignored)).Returns<Game?>(null);

            var newGame = new Game();
            var oldGame = new Game();

            // Act
            var func = new Func<Task>(async () => await _testService.EditGameAsync(newGame, oldGame));

            // Assert
            var ex = await func.ShouldThrowAsync<EntityNotFoundException>();
            ex.Message.ShouldBe<string>($"{_testService.GetType()}.EditGameAsync: The selected Game entity could not be found.");
        }

        [Fact]
        public async Task EditGameAsync_WhenNewGameAndOldGameArgsAreNotNullAndSelectedGameIsFound_ShouldEditGameInRepository()
        {
            // Arrange
            var selectedGame = new Game();
            A.CallTo(() => _gameRepository.GetGameAsync(A<int>.Ignored)).Returns(selectedGame);

            var downStrategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).Returns(downStrategy);
            var upStrategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).Returns(upStrategy);

            var newGame = new Game();
            var oldGame = new Game();

            // Act
            await _testService.EditGameAsync(newGame, oldGame);

            // Assert
            A.CallTo(() => _gameRepository.GetGameAsync(newGame.ID)).MustHaveHappened();
            A.CallTo(() => _gameRepository.Update(selectedGame)).MustHaveHappened();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => downStrategy.ProcessGameAsync(A<IGameDecorator>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => upStrategy.ProcessGameAsync(A<IGameDecorator>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void DeleteGame_WhenGameWithIdIsNotFoundInRepository_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var id = 1;
            A.CallTo(() => _gameRepository.GetGame(id)).Returns<Game?>(null);

            // Act
            var action = new Action(() => _testService.DeleteGame(id));

            // Assert
            var ex = action.ShouldThrow<EntityNotFoundException>();
            ex.Message.ShouldBe<string>(
                $"{_testService.GetType()}.DeleteGame: A Game entity with ID={id} could not be found.");
        }

        [Fact]
        public void DeleteGame_WhenGameWithIdIsFoundInRepository_ShouldDeleteGameFromRepository()
        {
            // Arrange
            var id = 1;

            var strategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).Returns(strategy);

            // Act
            _testService.DeleteGame(id);

            // Assert
            A.CallTo(() => _gameRepository.GetGame(id)).MustHaveHappened();
            A.CallTo(() => _gameRepository.Delete(id)).MustHaveHappened();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChanges()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task DeleteGameAsync_WhenGameWithIdIsNotFoundInRepository_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var id = 1;

            A.CallTo(() => _gameRepository.GetGameAsync(id)).Returns<Game?>(null);

            // Act
            var func = new Func<Task>(async () => await _testService.DeleteGameAsync(id));

            // Assert
            var ex = await func.ShouldThrowAsync<EntityNotFoundException>();
            ex.Message.ShouldBe<string>(
                $"{_testService.GetType()}.DeleteGameAsync: A Game entity with ID={id} could not be found.");
        }

        [Fact]
        public async Task DeleteGameAsync_WhenGameWithIdIsFoundInRepository_ShouldDeleteGameFromRepository()
        {
            // Arrange
            var id = 1;

            var strategy = A.Fake<ProcessGameStrategyBase>();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).Returns(strategy);

            // Act
            await _testService.DeleteGameAsync(id);

            // Assert
            A.CallTo(() => _gameRepository.GetGameAsync(id)).MustHaveHappened();
            A.CallTo(() => _gameRepository.DeleteAsync(id)).MustHaveHappened();
            A.CallTo(() => _processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
            A.CallTo(() => _sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
