using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using AutoMapper;
using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class GameControllerTest
    {
        [Fact]
        public async Task GetGames_WhenNoExceptionIsCaught_ShouldGetGames()
        {
            // Arrange
            List<Game> games = [];
            GameController testController = SetUp(games: games);

            // Act
            var result = await testController.GetGames();

            // Assert
            A.CallTo(() => testController._gameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<GameModel[]>(games)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<GameModel[]>>();
            result.Value.ShouldBe(testController._mapper.Map<GameModel[]>(games));
        }

        [Fact]
        public async Task GetGames_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            GameController testController = SetUp(ex: ex);

            // Act
            var result = await testController.GetGames();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetGame_WhenGameIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Game game = null!;
            GameController testController = SetUp(game: game);

            // Act
            int id = 1;
            var result = await testController.GetGame(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetGame_WhenGameIsNotNull_ShouldReturnGameModelOfDesiredGame()
        {
            // Arrange
            Game game = new();
            GameModel gameModel = new();
            GameController testController = SetUp(game: game, gameModel: gameModel);

            // Act
            int id = 1;
            var result = await testController.GetGame(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<GameModel>(game)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<GameModel>();
        }

        [Fact]
        public async Task GetGame_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            GameModel gameModel = new();
            var ex = new Exception();
            GameController testController = SetUp(gameModel: gameModel, ex: ex);

            // Act
            int id = 1;
            var result = await testController.GetGame(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutGame_WhenGameIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Game game = null!;
            Game oldGame = new();
            GameController testController = SetUp(game: game, oldGame: oldGame);

            // Act
            int id = 1;
            var models = new Dictionary<string, GameModel>
            {
                ["oldGame"] = new GameModel()
            };
            var result = await testController.PutGame(id, models);

            // Assert
            A.CallTo(() => testController._mapper.Map<Game>(models["oldGame"])).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find game with Id of {id}");
        }

        [Fact]
        public async Task PutGame_WhenGameIsFoundAndSaved_ShouldReturnModelOfGame()
        {
            // Arrange
            Game game = new();
            int numOfRecordsUpdated = 1;
            Game oldGame = new();
            GameModel returnModel = new();
            GameController testController = SetUp(
                game: game, numOfRecordsUpdated: numOfRecordsUpdated, oldGame: oldGame, gameModel: returnModel
            );

            // Act
            int id = 1;
            var models = new Dictionary<string, GameModel>
            {
                ["oldGame"] = new GameModel { Id = 1 },
                ["newGame"] = new GameModel { Id = 2 }
            };
            var result = await testController.PutGame(id, models);

            // Assert
            A.CallTo(() => testController._mapper.Map<Game>(models["oldGame"])).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(models["newGame"], game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, oldGame)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<GameModel>(game)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutGame_WhenGameIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            Game game = new();
            int numOfRecordsUpdated = 0;
            Game oldGame = new();
            GameModel returnModel = new();
            GameController testController = SetUp(
                game: game, numOfRecordsUpdated: numOfRecordsUpdated, oldGame: oldGame, gameModel: returnModel
            );

            // Act
            int id = 1;
            var models = new Dictionary<string, GameModel>
            {
                ["oldGame"] = new GameModel { Id = 1 },
                ["newGame"] = new GameModel { Id = 2 }
            };
            var result = await testController.PutGame(id, models);

            // Assert
            A.CallTo(() => testController._mapper.Map<Game>(models["oldGame"]))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameRepository.GetGameAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(models["newGame"], game))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.EditGameAsync(game, oldGame))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<GameModel>(game))
                .MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PutGame_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            GameController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var models = new Dictionary<string, GameModel>();
            var result = await testController.PutGame(id, models);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteGame_WhenCurrentGameIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Game game = null!;
            GameController testController = SetUp(game: game);

            // Act
            int id = 1;
            var result = await testController.DeleteGame(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find game with Id of {id}");
        }

        [Fact]
        public async Task DeleteGame_WhenCurrentGameIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            Game game = new();
            int numOfRecordsUpdated = 1;
            GameController testController = SetUp(game: game, numOfRecordsUpdated: numOfRecordsUpdated);

            // Act
            int id = 1;
            var result = await testController.DeleteGame(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteGame_WhenCurrentGameIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            Game game = new();
            int numOfRecordsUpdated = 0;
            GameController testController = SetUp(game: game, numOfRecordsUpdated: numOfRecordsUpdated);

            // Act
            int id = 1;
            var result = await testController.DeleteGame(id);

            // Assert
            A.CallTo(() => testController._gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._gameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteGame_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            GameController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var result = await testController.DeleteGame(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static GameController SetUp(
            List<Game>? games = null, Game? game = null, int? numOfRecordsUpdated = null,
            GameModel? gameModel = null, Game? oldGame = null, Exception? ex = null
        )
        {
            IGameRepository fakeGameRepository = SetUpFakeGameRepository(games, game, ex);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(numOfRecordsUpdated);
            IMapper fakeMapper = SetUpFakeMapper(gameModel, oldGame);
            LinkGenerator fakeLinkGenerator = SetUpFakeLinkGenerator();
            IGameService fakeGameService = SetUpFakeGameService();

            return new GameController(
                fakeGameRepository, fakeSharedRepository, fakeMapper, fakeLinkGenerator, fakeGameService
            );
        }

        private static IGameRepository SetUpFakeGameRepository(List<Game>? games, Game? game, Exception? ex)
        {
            var fakeGameRepository = A.Fake<IGameRepository>();
            if (ex is null)
            {
                A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);
                A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);
            }
            else
            {
                A.CallTo(() => fakeGameRepository.GetGamesAsync()).Throws(ex);
                A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Throws(ex);
            }

            return fakeGameRepository;
        }

        private static ISharedRepository SetUpFakeSharedRepository(int? numOfRecordsUpdated)
        {
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            if (numOfRecordsUpdated.HasValue)
            {
                A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Returns(numOfRecordsUpdated.Value);
            }

            return fakeSharedRepository;
        }

        private static IMapper SetUpFakeMapper(GameModel? gameModel, Game? oldGame)
        {
            var fakeMapper = A.Fake<IMapper>();
            if (gameModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<GameModel>(A<Game>.Ignored)).Returns(gameModel);
            }
            if (oldGame is not null)
            {
                A.CallTo(() => fakeMapper.Map<Game>(A<GameModel>.Ignored)).Returns(oldGame);
            }

            return fakeMapper;
        }

        private static LinkGenerator SetUpFakeLinkGenerator()
        {
            return A.Fake<LinkGenerator>();
        }

        private static IGameService SetUpFakeGameService()
        {
            return A.Fake<IGameService>();
        }
    }
}
