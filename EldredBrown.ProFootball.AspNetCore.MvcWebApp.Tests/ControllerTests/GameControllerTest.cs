using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class GameControllerTest
    {
        [Fact]
        public async Task Index_WhenSeasonsListIsNotEmptyAndSelectedWeekIsNotNullAndGamesListIsNotEmpty_ShouldReturnGamesIndexView()
        {
            // Arrange
            var selectedWeek = 1;
            GameController.SelectedWeek = selectedWeek;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = 1920, NumOfWeeksScheduled = 2 },
                new Season { Year = 1921, NumOfWeeksScheduled = 2 },
                new Season { Year = 1922, NumOfWeeksScheduled = 2 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game { SeasonYear = 1920, Week = 1 },
                new Game { SeasonYear = 1920, Week = 2 },
                new Game { SeasonYear = 1920, Week = 3 },
                new Game { SeasonYear = 1921, Week = 1 },
                new Game { SeasonYear = 1921, Week = 2 },
                new Game { SeasonYear = 1921, Week = 3 },
                new Game { SeasonYear = 1922, Week = 1 },
                new Game { SeasonYear = 1922, Week = 2 },
                new Game { SeasonYear = 1922, Week = 3 },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(GameController.SelectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(GameController.SelectedSeasonYear);

            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Weeks.Items.ShouldBeEquivalentTo(new List<int?> { null, 1, 2 });
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBe(GameController.SelectedWeek);

            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            var filteredGames = games.Where(
                g => g.SeasonYear == GameController.SelectedSeasonYear && g.Week == GameController.SelectedWeek);
            fakeGameIndexViewModel.Games.ShouldBe(filteredGames);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSeasonsListIsEmpty_ShouldReturnGamesIndexView()
        {
            // Arrange
            GameController.SelectedWeek = null;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(GameController.SelectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(GameController.SelectedSeasonYear);

            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Weeks.Items.ShouldBeOfType<List<int?>>();
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBe(GameController.SelectedWeek);

            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Games.ShouldBe(games);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedWeekIsNull_ShouldReturnGamesIndexView()
        {
            // Arrange
            GameController.SelectedWeek = null;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = 1920, NumOfWeeksScheduled = 2 },
                new Season { Year = 1921, NumOfWeeksScheduled = 2 },
                new Season { Year = 1922, NumOfWeeksScheduled = 2 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(GameController.SelectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(GameController.SelectedSeasonYear);

            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Weeks.Items.ShouldBeEquivalentTo(new List<int?> { null, 1, 2 });
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBe(GameController.SelectedWeek);

            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Games.ShouldBe(games);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenGamesListIsEmpty_ShouldReturnGamesIndexView()
        {
            // Arrange
            var selectedWeek = 1;
            GameController.SelectedWeek = selectedWeek;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = 1920, NumOfWeeksScheduled = 2 },
                new Season { Year = 1921, NumOfWeeksScheduled = 2 },
                new Season { Year = 1922, NumOfWeeksScheduled = 2 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Year");
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(GameController.SelectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(GameController.SelectedSeasonYear);

            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Weeks.Items.ShouldBeEquivalentTo(new List<int?> { null, 1, 2 });
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBe(GameController.SelectedWeek);

            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Games.ShouldBe(games);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndGameFound_ShouldSucceed()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Details(id.Value);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            fakeGameDetailsViewModel.Game.ShouldBe(game);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Details(id.Value);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSeasonsListIsNotEmptyAndSelectedWeekIsNotNull_ShouldShowGameCreateView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            GameController.SelectedSeasonYear = selectedSeasonYear;
            GameController.SelectedWeek = 2;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = selectedSeasonYear, NumOfWeeksScheduled = 3 }
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Create();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(GameController.SelectedSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            weeksSelectList.Items.ShouldBeEquivalentTo(new List<int?> { 1, 2, 3 });
            weeksSelectList.SelectedValue.ShouldBe(GameController.SelectedWeek);

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSeasonsListIsEmpty_ShouldShowGameCreateView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            GameController.SelectedSeasonYear = selectedSeasonYear;
            GameController.SelectedWeek = null;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Create();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(GameController.SelectedSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            weeksSelectList.Items.ShouldBeEquivalentTo(new List<int?>());
            weeksSelectList.SelectedValue.ShouldBe(1);

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSelectedWeekIsNull_ShouldShowGameCreateView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            GameController.SelectedSeasonYear = selectedSeasonYear;
            GameController.SelectedWeek = null;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Year = selectedSeasonYear, NumOfWeeksScheduled = 3 }
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Create();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            seasonsSelectList.DataValueField.ShouldBe<string>("Year");
            seasonsSelectList.DataTextField.ShouldBe<string>("Year");
            seasonsSelectList.SelectedValue.ShouldBe(GameController.SelectedSeasonYear);

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
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            var game = new Game();

            // Act
            var result = await testController.Create(game);

            // Assert
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Create));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldShowGameCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            var game = new Game();
            testController.ModelState.AddModelError("Season", "Please enter a season.");

            // Act
            var result = await testController.Create(game);

            // Assert
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndGameFound_ShouldShowGameEditView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            GameController.SelectedSeasonYear = selectedSeasonYear;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season
                {
                    Year = 1920,
                    NumOfWeeksScheduled = 3
                },
            };
            var selectedSeason = seasons.FirstOrDefault(s => s.Year == selectedSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            int? id = 1;
            Game? game = new Game
            {
                SeasonYear = 1920,
                Week = 1
            };
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).Returns(game);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

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

            GameController.OldGame.ShouldBe(game);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            int? id = 1;
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).Returns(game);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsValidAndDbConcurrencyExceptionIsNotCaught_ShouldUpdateGameInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var oldGame = new Game();
            GameController.OldGame = oldGame;

            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int id = 1;
            var game = new Game { Id = 1 };

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            A.CallTo(() => fakeGameService.EditGameAsync(game, oldGame)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualGameId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int id = 0;
            var game = new Game { Id = 1 };

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnEditGameView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int id = 1;
            var game = new Game { Id = 1 };
            testController.ModelState.AddModelError("Season", "Please enter a season.");

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task EditPost_WhenDbConcurrencyExceptionIsCaughtAndGameWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameService = A.Fake<IGameService>();
            A.CallTo(() => fakeGameService.EditGameAsync(A<Game>.Ignored, A<Game>.Ignored))
                .Throws<DbUpdateConcurrencyException>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GameExists(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int id = 1;
            var game = new Game { Id = 1 };

            // Act
            var result = await testController.Edit(id, game);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbConcurrencyExceptionIsCaughtAndGameWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameService = A.Fake<IGameService>();
            A.CallTo(() => fakeGameService.EditGameAsync(A<Game>.Ignored, A<Game>.Ignored))
                .Throws<DbUpdateConcurrencyException>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            _ = A.CallTo(() => fakeGameRepository.GameExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int id = 1;
            var game = new Game { Id = 1 };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, game));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndGameIsFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = new Game();
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(game);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenGameIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteGameFromDataStore()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeGameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? year = 1920;

            // Act
            var result = testController.SetSelectedSeasonYear(year);

            // Assert
            GameController.SelectedSeasonYear.ShouldBe(year.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? year = null;

            // Act
            var result = testController.SetSelectedSeasonYear(year);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedWeek_ShouldSetSelectedWeekAndRedirectToIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel, fakeGameService,
                fakeSeasonRepository, fakeTeamRepository, fakeGameRepository, fakeSharedRepository);

            int? week = 1;

            // Act
            var result = testController.SetSelectedWeek(week);

            // Assert
            GameController.SelectedWeek.ShouldBe(week);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
