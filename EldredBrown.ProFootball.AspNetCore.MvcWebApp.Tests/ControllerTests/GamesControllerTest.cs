using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Games;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class GamesControllerTest
    {
        [Fact]
        public async Task Index_WhenSelectedSeasonIsNullAndSelectedWeekIsNull_ShouldReturnGamesIndexView()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            A.CallTo(() => gameRepository.GetGamesAsync()).Returns(games);

            var teamRepository = A.Fake<ITeamRepository>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            gamesIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            gamesIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            gamesIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            gamesIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            gamesIndexViewModel.Seasons.SelectedValue.ShouldBe(GamesController.SelectedSeasonYear);
            gamesIndexViewModel.SelectedSeasonYear.ShouldBe(GamesController.SelectedSeasonYear);
            gamesIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            gamesIndexViewModel.Weeks.Items.ShouldBeOfType<List<int?>>();
            gamesIndexViewModel.Weeks.SelectedValue.ShouldBe(GamesController.SelectedWeek);
            gamesIndexViewModel.Games.ShouldBe(games);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gamesIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonIsNotNullAndSelectedWeekIsNull_ShouldReturnGamesIndexView()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            A.CallTo(() => gameRepository.GetGamesAsync()).Returns(games);

            var teamRepository = A.Fake<ITeamRepository>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = 1920, NumOfWeeksScheduled = 2 },
                new Season { Year = 1921, NumOfWeeksScheduled = 2 },
                new Season { Year = 1922, NumOfWeeksScheduled = 2 },
            };
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            gamesIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            gamesIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            gamesIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            gamesIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            gamesIndexViewModel.Seasons.SelectedValue.ShouldBe(GamesController.SelectedSeasonYear);
            gamesIndexViewModel.SelectedSeasonYear.ShouldBe(GamesController.SelectedSeasonYear);
            gamesIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            gamesIndexViewModel.Weeks.Items.ShouldBeEquivalentTo(new List<int?> { null, 1, 2 });
            gamesIndexViewModel.Weeks.SelectedValue.ShouldBe(GamesController.SelectedWeek);
            gamesIndexViewModel.Games.ShouldBe(games);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gamesIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonIsNullAndSelectedWeekIsNotNull_ShouldReturnGamesIndexView()
        {
            // Arrange
            var selectedWeek = 1;
            GamesController.SelectedWeek = selectedWeek;

            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game { SeasonYear = 1920, Week = 1 },
                new Game { SeasonYear = 1920, Week = 2 },
                new Game { SeasonYear = 1920, Week = 3 }
            };
            A.CallTo(() => gameRepository.GetGamesAsync()).Returns(games);

            var teamRepository = A.Fake<ITeamRepository>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = 1920, NumOfWeeksScheduled = 2 },
                new Season { Year = 1921, NumOfWeeksScheduled = 2 },
                new Season { Year = 1922, NumOfWeeksScheduled = 2 },
            };
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => gameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();

            gamesIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            gamesIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            gamesIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            gamesIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            gamesIndexViewModel.Seasons.SelectedValue.ShouldBe(GamesController.SelectedSeasonYear);
            gamesIndexViewModel.SelectedSeasonYear.ShouldBe(GamesController.SelectedSeasonYear);
            gamesIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            gamesIndexViewModel.Weeks.Items.ShouldBeEquivalentTo(new List<int?> { null, 1, 2 });
            gamesIndexViewModel.Weeks.SelectedValue.ShouldBe(GamesController.SelectedWeek);
            gamesIndexViewModel.Games.ShouldBe(games.Where(g => g.Week == selectedWeek).ToList());

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gamesIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Details(id.Value);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndGameFound_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Details(id.Value);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            gamesDetailsViewModel.Game.ShouldBe(game);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gamesDetailsViewModel);
        }

        [Fact]
        public async Task CreateGet_WhenSelectedWeekIsNotNull_ShouldShowGameCreateView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            GamesController.SelectedSeasonYear = selectedSeasonYear;
            GamesController.SelectedWeek = 2;

            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = selectedSeasonYear, NumOfWeeksScheduled = 3 }
            };
            var selectedSeason = seasons.FirstOrDefault(s => s.Year == selectedSeasonYear);
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            // Act
            var result = await testController.Create();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(GamesController.SelectedSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            weeksSelectList.Items.ShouldBeEquivalentTo(new List<int?> { 1, 2, 3 });
            weeksSelectList.SelectedValue.ShouldBe(GamesController.SelectedWeek);

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSelectedWeekIsNull_ShouldShowGameCreateView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            GamesController.SelectedSeasonYear = selectedSeasonYear;
            GamesController.SelectedWeek = null;

            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = selectedSeasonYear, NumOfWeeksScheduled = 3 }
            };
            var selectedSeason = seasons.FirstOrDefault(s => s.Year == selectedSeasonYear);
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            // Act
            var result = await testController.Create();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(GamesController.SelectedSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            weeksSelectList.Items.ShouldBeEquivalentTo(new List<int?> { 1, 2, 3 });
            weeksSelectList.SelectedValue.ShouldBe(1);

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddGameToDataStoreAndRedirectToCreateView()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            var game = new Game();

            // Act
            var result = await testController.Create(game);

            // Assert
            A.CallTo(() => gameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Create));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldShowGameCreateView()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            var game = new Game();
            testController.ModelState.AddModelError("Season", "Please enter a season.");

            // Act
            var result = await testController.Create(game);

            // Assert
            A.CallTo(() => gameService.AddGameAsync(game)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            int? id = 1;
            Game? game = null;
            A.CallTo(() => gameRepository.GetGameAsync(id.Value)).Returns(game);

            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndGameFound_ShouldShowGameEditView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            GamesController.SelectedSeasonYear = selectedSeasonYear;

            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            int? id = 1;
            Game? game = new Game
            {
                SeasonYear = 1920,
                Week = 1
            };
            A.CallTo(() => gameRepository.GetGameAsync(id.Value)).Returns(game);

            var teamRepository = A.Fake<ITeamRepository>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season
                {
                    Year = 1920,
                    NumOfWeeksScheduled = 3
                },
            };
            var selectedSeason = seasons.FirstOrDefault(s => s.Year == selectedSeasonYear);
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(game.SeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            weeksSelectList.Items.ShouldBeEquivalentTo(new List<int?> { 1, 2, 3 });
            weeksSelectList.SelectedValue.ShouldBe(game.Week);

            GamesController.OldGame.ShouldBe(game);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualGameId_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int id = 0;
            var game = new Game { ID = 1 };

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsNotValid_ShouldReturnEditGameView()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int id = 1;
            var game = new Game { ID = 1 };
            testController.ModelState.AddModelError("Season", "Please enter a season.");

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsValidAndDbConcurrencyExceptionIsNotCaught_ShouldUpdateGameInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var oldGame = new Game();
            GamesController.OldGame = oldGame;

            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int id = 1;
            var game = new Game { ID = 1 };

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            A.CallTo(() => gameService.EditGameAsync(game, oldGame)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsValidAndDbConcurrencyExceptionIsCaughtAndGameWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();

            var gameService = A.Fake<IGameService>();
            A.CallTo(() => gameService.EditGameAsync(A<Game>.Ignored, A<Game>.Ignored))
                .Throws<DbUpdateConcurrencyException>();

            var gameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => gameRepository.GameExists(An<int>.Ignored)).Returns(false);

            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int id = 1;
            var game = new Game { ID = 1 };

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsValidAndDbConcurrencyExceptionIsCaughtAndGameWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();

            var gameService = A.Fake<IGameService>();
            A.CallTo(() => gameService.EditGameAsync(A<Game>.Ignored, A<Game>.Ignored))
                .Throws<DbUpdateConcurrencyException>();

            var gameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => gameRepository.GameExists(An<int>.Ignored)).Returns(true);

            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int id = 1;
            var game = new Game { ID = 1 };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, game));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndGameIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndGameIsFound_ShouldReturnNotFound()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();

            var gameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => gameRepository.GetGameAsync(A<int>.Ignored)).Returns(game);

            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => gameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteGameFromDataStore()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => gameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? year = null;

            // Act
            var result = testController.SetSelectedSeasonYear(year);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? year = 1920;

            // Act
            var result = testController.SetSelectedSeasonYear(year);

            // Assert
            GamesController.SelectedSeasonYear.ShouldBe(year.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedWeek_ShouldSetSelectedWeekAndRedirectToIndexView()
        {
            // Arrange
            var gamesIndexViewModel = A.Fake<IGamesIndexViewModel>();
            var gamesDetailsViewModel = A.Fake<IGamesDetailsViewModel>();
            var gameService = A.Fake<IGameService>();
            var gameRepository = A.Fake<IGameRepository>();
            var teamRepository = A.Fake<ITeamRepository>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new GamesController(gamesIndexViewModel, gamesDetailsViewModel, gameService,
                gameRepository, teamRepository, seasonRepository, sharedRepository);

            int? week = 1;

            // Act
            var result = testController.SetSelectedWeek(week);

            // Assert
            GamesController.SelectedWeek.ShouldBe(week);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
