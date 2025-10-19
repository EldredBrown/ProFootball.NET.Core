using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class GamesControllerTest
    {
        [Fact]
        public async Task GetGames_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => gameRepository.GetGamesAsync()).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            // Act
            var result = await testController.GetGames();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetGames_WhenNoExceptionIsCaught_ShouldGetGames()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            A.CallTo(() => gameRepository.GetGamesAsync()).Returns(games);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            // Act
            var result = await testController.GetGames();

            // Assert
            A.CallTo(() => gameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<GameModel[]>(games)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<GameModel[]>>();
            result.Value.ShouldBe(mapper.Map<GameModel[]>(games));
        }

        [Fact]
        public async Task GetGame_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            GameModel? gameModel = new GameModel();
            A.CallTo(() => mapper.Map<GameModel>(A<Game>.Ignored)).Returns(gameModel);

            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.GetGame(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetGame_WhenGameIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.GetGame(id);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetGame_WhenGameIsNotNull_ShouldReturnGameModelOfDesiredGame()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            GameModel? gameModel = new GameModel();
            A.CallTo(() => mapper.Map<GameModel>(A<Game>.Ignored)).Returns(gameModel);

            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.GetGame(id);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<GameModel>(game)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<GameModel>();
        }

        [Fact]
        public async Task PutGame_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;
            var models = new Dictionary<string, GameModel>();

            // Act
            var result = await testController.PutGame(id, models);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutGame_WhenCurrentGameIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? currentGame = null;
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(currentGame);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            Game? oldGame = new Game();
            A.CallTo(() => mapper.Map<Game>(A<Dictionary<string, GameModel>>.Ignored)).Returns(oldGame);

            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;
            var models = new Dictionary<string, GameModel>
            {
                ["oldGame"] = new GameModel()
            };

            // Act
            var result = await testController.PutGame(id, models);

            // Assert
            A.CallTo(() => mapper.Map<Game>(models["oldGame"])).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find game with ID of {id}");
        }

        [Fact]
        public async Task PutGame_WhenCurrentGameIsFoundAndSaved_ShouldReturnModelOfGame()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? currentGame = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(currentGame);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var models = new Dictionary<string, GameModel>
            {
                ["oldGame"] = new GameModel { ID = 1 },
                ["newGame"] = new GameModel { ID = 2 }
            };

            var mapper = A.Fake<IMapper>();
            Game? oldGame = new Game();
            A.CallTo(() => mapper.Map<Game>(models["oldGame"])).Returns(oldGame);
            var currentGameModel = new GameModel();
            A.CallTo(() => mapper.Map<GameModel>(A<Game>.Ignored)).Returns(currentGameModel);

            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.PutGame(id, models);

            // Assert
            A.CallTo(() => mapper.Map<Game>(models["oldGame"])).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(models["newGame"], currentGame)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameService.EditGameAsync(currentGame, oldGame)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<GameModel>(currentGame)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(currentGameModel);
        }

        [Fact]
        public async Task PutGame_WhenCurrentGameIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? currentGame = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(currentGame);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var models = new Dictionary<string, GameModel>
            {
                ["oldGame"] = new GameModel { ID = 1 },
                ["newGame"] = new GameModel { ID = 2 }
            };

            var mapper = A.Fake<IMapper>();
            Game? oldGame = new Game();
            A.CallTo(() => mapper.Map<Game>(models["oldGame"])).Returns(oldGame);
            var currentGameModel = new GameModel();
            A.CallTo(() => mapper.Map<GameModel>(A<Game>.Ignored)).Returns(currentGameModel);

            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.PutGame(id, models);

            // Assert
            A.CallTo(() => mapper.Map<Game>(models["oldGame"])).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(models["newGame"], currentGame)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameService.EditGameAsync(currentGame, oldGame)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<GameModel>(currentGame)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteGame_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.DeleteGame(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteGame_WhenCurrentGameIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.DeleteGame(id);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find game with ID of {id}");
        }

        [Fact]
        public async Task DeleteGame_WhenCurrentGameIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.DeleteGame(id);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteGame_WhenCurrentGameIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var gameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();
            var gameService = A.Fake<IGameService>();

            var testController =
                new GamesController(gameRepository, sharedRepository, mapper, linkGenerator, gameService);

            int id = 1;

            // Act
            var result = await testController.DeleteGame(id);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
