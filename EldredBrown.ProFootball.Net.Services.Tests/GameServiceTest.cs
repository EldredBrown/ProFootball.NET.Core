using System;
using System.Threading.Tasks;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.Net.Data.Exceptions;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.ProcessGameStrategy;

namespace EldredBrown.ProFootball.Net.Services.Tests
{
    public class GameServiceTest
    {
        [Fact]
        public void AddGame_WhenNewGameArgIsNotNull_ShouldAddGameToRepository()
        {
            // Arrange
            var strategy = A.Fake<ProcessGameStrategyBase>();
            GameService testService = SetUp(upStrategy: strategy);

            // Act
            var newGame = new Game();
            testService.AddGame(newGame);

            // Assert
            A.CallTo(() => testService._gameRepository.Add(newGame)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => strategy.ProcessGame(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void AddGame_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var testService = SetUp();

            // Act
            Game? newGame = null;
            var action = new Action(() => testService.AddGame(newGame!));

            // Assert
            var ex = action.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testService.GetType()}.AddGame: newGame");
        }

        [Fact]
        public async Task AddGameAsync_WhenNewGameArgIsNotNull_ShouldAddGameToRepository()
        {
            // Arrange
            var strategy = A.Fake<ProcessGameStrategyBase>();
            var testService = SetUp(upStrategy: strategy);

            // Act
            var newGame = new Game();
            await testService.AddGameAsync(newGame);

            // Assert
            A.CallTo(() => testService._gameRepository.AddAsync(newGame)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => strategy.ProcessGameAsync(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task AddGameAsync_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GameService testService = SetUp();

            // Act
            Game? newGame = null;
            var func = new Func<Task>(async () => await testService.AddGameAsync(newGame!));

            // Assert
            var ex = await func.ShouldThrowAsync<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testService.GetType()}.AddGameAsync: newGame");
        }

        [Fact]
        public void EditGame_WhenArgsAreNotNullAndSelectedGameIsFound_ShouldEditGameInRepository()
        {
            // Arrange
            var selectedGame = new Game();

            var downStrategy = A.Fake<ProcessGameStrategyBase>();
            var upStrategy = A.Fake<ProcessGameStrategyBase>();

            GameService testService = SetUp(selectedGame: selectedGame, downStrategy: downStrategy, upStrategy: upStrategy);

            // Act
            var newGame = new Game();
            var oldGame = new Game();
            testService.EditGame(newGame, oldGame);

            // Assert
            A.CallTo(() => testService._gameRepository.GetGame(newGame.Id)).MustHaveHappened();
            A.CallTo(() => testService._gameRepository.Update(selectedGame)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => downStrategy.ProcessGame(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => upStrategy.ProcessGame(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void EditGame_WhenOldGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var testService = SetUp();

            // Act
            var newGame = new Game();
            Game? oldGame = null;
            var func = new Action(() => testService.EditGame(newGame, oldGame!));

            // Assert
            var ex = func.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testService.GetType()}.EditGame: oldGame");
        }

        [Fact]
        public void EditGame_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var testService = SetUp();

            // Act
            Game? newGame = null;
            Game? oldGame = null;
            var action = new Action(() => testService.EditGame(newGame!, oldGame!));

            // Assert
            var ex = action.ShouldThrow<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testService.GetType()}.EditGame: newGame");
        }

        [Fact]
        public void EditGame_WhenSelectedGameIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var testService = SetUp();

            // Act
            var newGame = new Game();
            var oldGame = new Game();
            var func = new Action(() => testService.EditGame(newGame, oldGame));

            // Assert
            var ex = func.ShouldThrow<EntityNotFoundException>();
            ex.Message.ShouldBe<string>($"{testService.GetType()}.EditGame: The selected Game entity could not be found.");
        }

        [Fact]
        public async Task EditGameAsync_WhenArgsAreNotNullAndSelectedGameIsFound_ShouldEditGameInRepository()
        {
            // Arrange
            var selectedGame = new Game();
            var downStrategy = A.Fake<ProcessGameStrategyBase>();
            var upStrategy = A.Fake<ProcessGameStrategyBase>();

            GameService testService = SetUp(selectedGame: selectedGame, downStrategy: downStrategy, upStrategy: upStrategy);

            // Act
            var newGame = new Game();
            var oldGame = new Game();
            await testService.EditGameAsync(newGame, oldGame);

            // Assert
            A.CallTo(() => testService._gameRepository.GetGameAsync(newGame.Id)).MustHaveHappened();
            A.CallTo(() => testService._gameRepository.Update(selectedGame)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Up)).MustHaveHappened();
            A.CallTo(() => downStrategy.ProcessGameAsync(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => upStrategy.ProcessGameAsync(A<Game>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task EditGameAsync_WhenNewGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GameService testService = SetUp();

            // Act
            Game? newGame = null;
            Game? oldGame = null;
            var func = new Func<Task>(async () => await testService.EditGameAsync(newGame!, oldGame!));

            // Assert
            var ex = await func.ShouldThrowAsync<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testService.GetType()}.EditGameAsync: newGame");
        }

        [Fact]
        public async Task EditGameAsync_WhenOldGameArgIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            GameService testService = SetUp();

            // Act
            var newGame = new Game();
            Game? oldGame = null;
            var func = new Func<Task>(async () => await testService.EditGameAsync(newGame, oldGame!));

            // Assert
            var ex = await func.ShouldThrowAsync<ArgumentNullException>();
            ex.ParamName.ShouldBe<string>($"{testService.GetType()}.EditGameAsync: oldGame");
        }

        [Fact]
        public async Task EditGameAsync_WhenSelectedGameIsNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            GameService testService = SetUp();

            // Act
            var newGame = new Game();
            var oldGame = new Game();
            var func = new Func<Task>(async () => await testService.EditGameAsync(newGame, oldGame));

            // Assert
            var ex = await func.ShouldThrowAsync<EntityNotFoundException>();
            ex.Message.ShouldBe<string>($"{testService.GetType()}.EditGameAsync: The selected Game entity could not be found.");
        }

        [Fact]
        public void DeleteGame_WhenGameWithIdIsFoundInRepository_ShouldDeleteGameFromRepository()
        {
            // Arrange
            var selectedGame = new Game
            {
                Id = 1
            };
            var strategy = A.Fake<ProcessGameStrategyBase>();
            GameService testService = SetUp(selectedGame: selectedGame, downStrategy: strategy);

            // Act
            var id = 1;
            testService.DeleteGame(id);

            // Assert
            A.CallTo(() => testService._gameRepository.GetGame(id)).MustHaveHappened();
            A.CallTo(() => testService._gameRepository.Delete(id)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
        }

        [Fact]
        public void DeleteGame_WhenGameWithIdIsNotFoundInRepository_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var testService = SetUp();

            // Act
            var id = 1;
            var action = new Action(() => testService.DeleteGame(id));

            // Assert
            var ex = action.ShouldThrow<EntityNotFoundException>();
            ex.Message.ShouldBe<string>(
                $"{testService.GetType()}.DeleteGame: A Game entity with Id={id} could not be found.");
        }

        [Fact]
        public async Task DeleteGameAsync_WhenGameWithIdIsFoundInRepository_ShouldDeleteGameFromRepository()
        {
            // Arrange
            var selectedGame = new Game
            {
                Id = 1
            };
            var strategy = A.Fake<ProcessGameStrategyBase>();
            GameService testService = SetUp(selectedGame: selectedGame, downStrategy: strategy);

            // Act
            var id = 1;
            await testService.DeleteGameAsync(id);

            // Assert
            A.CallTo(() => testService._gameRepository.GetGameAsync(id)).MustHaveHappened();
            A.CallTo(() => testService._gameRepository.DeleteAsync(id)).MustHaveHappened();
            A.CallTo(() => testService._processGameStrategyFactory.CreateStrategy(Direction.Down)).MustHaveHappened();
        }

        [Fact]
        public async Task DeleteGameAsync_WhenGameWithIdIsNotFoundInRepository_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var testService = SetUp();

            // Act
            var id = 1;
            var func = new Func<Task>(async () => await testService.DeleteGameAsync(id));

            // Assert
            var ex = await func.ShouldThrowAsync<EntityNotFoundException>();
            ex.Message.ShouldBe<string>(
                $"{testService.GetType()}.DeleteGameAsync: A Game entity with Id={id} could not be found.");
        }

        private GameService SetUp(
            Game? selectedGame = null,
            ProcessGameStrategyBase? downStrategy = null, ProcessGameStrategyBase? upStrategy = null
        )
        {
            IGameRepository fakeGameRepository = SetUpFakeGameRepository(selectedGame);
            IProcessGameStrategyFactory fakeProcessGameStrategyFactory = 
                SetUpFakeProcessGameStrategyFactory(downStrategy, upStrategy);

            return new GameService(fakeGameRepository, fakeProcessGameStrategyFactory);
        }

        private static IGameRepository SetUpFakeGameRepository(Game? selectedGame)
        {
            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GetGame(An<int>.Ignored)).Returns(selectedGame);
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(selectedGame);

            return fakeGameRepository;
        }

        private static IProcessGameStrategyFactory SetUpFakeProcessGameStrategyFactory(ProcessGameStrategyBase? downStrategy, ProcessGameStrategyBase? upStrategy)
        {
            var fakeProcessGameStrategyFactory = A.Fake<IProcessGameStrategyFactory>();
            if (downStrategy is not null)
            {
                A.CallTo(() => fakeProcessGameStrategyFactory.CreateStrategy(Direction.Down)).Returns(downStrategy);
            }
            if (upStrategy is not null)
            {
                A.CallTo(() => fakeProcessGameStrategyFactory.CreateStrategy(Direction.Up)).Returns(upStrategy);
            }

            return fakeProcessGameStrategyFactory;
        }
    }
}
