using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Moq;
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
        public async Task Index_WhenSelectedSeasonIsNullAndSelectedWeekIsNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModels = new List<GameViewModel>
            {
                new GameViewModel
                {
                    Id = 7,
                    SeasonYear = 1922,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new GameViewModel
                {
                    Id = 8,
                    SeasonYear = 1922,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new GameViewModel
                {
                    Id = 9,
                    SeasonYear = 1922,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .ReturnsNextFromSequence(gameViewModels.ToArray());

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 4,
                    SeasonId = 1921,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 5,
                    SeasonId = 1921,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 6,
                    SeasonId = 1921,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 7,
                    SeasonId = 1922,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 8,
                    SeasonId = 1922,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 9,
                    SeasonId = 1922,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Id = 1920, NumOfWeeksScheduled = 3 },
                new Season { Id = 1921, NumOfWeeksScheduled = 3 },
                new Season { Id = 1922, NumOfWeeksScheduled = 3 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", null);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            var orderedSeasons = seasons.OrderByDescending(s => s.Id);

            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Id");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Id");

            var firstSeasonId = orderedSeasons.First().Id;
            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(firstSeasonId);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(firstSeasonId);

            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            fakeGameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            fakeGameIndexViewModel.SelectedWeek.ShouldBeNull();

            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonIsNotNullAndSelectedWeekIsNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var selectedSeasonYear = 1920;
            var gameViewModels = new List<GameViewModel>
            {
                new GameViewModel
                {
                    Id = 1,
                    SeasonYear = selectedSeasonYear,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new GameViewModel
                {
                    Id = 2,
                    SeasonYear = selectedSeasonYear,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new GameViewModel
                {
                    Id = 3,
                    SeasonYear = selectedSeasonYear,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .ReturnsNextFromSequence(gameViewModels.ToArray());

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 4,
                    SeasonId = 1921,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 5,
                    SeasonId = 1921,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 6,
                    SeasonId = 1921,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 7,
                    SeasonId = 1922,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 8,
                    SeasonId = 1922,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 9,
                    SeasonId = 1922,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Id = 1920, NumOfWeeksScheduled = 3 },
                new Season { Id = 1921, NumOfWeeksScheduled = 3 },
                new Season { Id = 1922, NumOfWeeksScheduled = 3 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            var orderedSeasons = seasons.OrderByDescending(s => s.Id);

            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Id");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Id");

            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            fakeGameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBeNull();
            fakeGameIndexViewModel.SelectedWeek.ShouldBeNull();

            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonIsNotNullAndSelectedWeekIsNotNull_ShouldReturnGameIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var selectedSeasonYear = 1920;
            var gameViewModels = new List<GameViewModel>
            {
                new GameViewModel
                {
                    Id = 2,
                    SeasonYear = selectedSeasonYear,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .ReturnsNextFromSequence(gameViewModels.ToArray());

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 4,
                    SeasonId = 1921,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 5,
                    SeasonId = 1921,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 6,
                    SeasonId = 1921,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 7,
                    SeasonId = 1922,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 8,
                    SeasonId = 1922,
                    Week = 2,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 9,
                    SeasonId = 1922,
                    Week = 3,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Id = 1920, NumOfWeeksScheduled = 3 },
                new Season { Id = 1921, NumOfWeeksScheduled = 3 },
                new Season { Id = 1922, NumOfWeeksScheduled = 3 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);

            var selectedWeek = 2;
            fakeSession.SetObject<int?>("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            var orderedSeasons = seasons.OrderByDescending(s => s.Id);

            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeGameIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeGameIndexViewModel.Seasons.DataValueField.ShouldBe("Id");
            fakeGameIndexViewModel.Seasons.DataTextField.ShouldBe("Id");

            fakeGameIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeGameIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            fakeGameIndexViewModel.Weeks.ShouldBeOfType<SelectList>();
            var weeks = new List<int?> { null, 1, 2, 3 };
            fakeGameIndexViewModel.Weeks.Items.ShouldBe(weeks);
            fakeGameIndexViewModel.Weeks.SelectedValue.ShouldBe(selectedWeek);
            fakeGameIndexViewModel.SelectedWeek.ShouldBe(selectedWeek);

            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            fakeGameIndexViewModel.Games.ShouldBe(gameViewModels);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndGameFound_ShouldReturnGameDetailsView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModel = new GameViewModel { };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(An<Game>.Ignored)).Returns(gameViewModel);

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var game = new Game { };
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();
            fakeGameDetailsViewModel.Game.ShouldNotBeNull();
            fakeGameDetailsViewModel.Game.ShouldBeOfType<GameViewModel>();
            fakeGameDetailsViewModel.Game.ShouldBe(gameViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeGameDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSelectedWeekIsNotNull_ShouldShowGameCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var selectedSeasonYear = 1920;
            var seasons = new List<Season>
            {
                new Season { Id = selectedSeasonYear, NumOfWeeksScheduled = 3 }
            };
            var selectedSeason = seasons.FirstOrDefault(s => s.Id == selectedSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);

            var selectedWeek = 2;
            fakeSession.SetObject<int?>("SelectedWeek", selectedWeek);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Create();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            seasonsSelectList.DataValueField.ShouldBe<string>("Id");
            seasonsSelectList.DataTextField.ShouldBe<string>("Id");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { null, 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(selectedWeek);

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreateGet_WhenSelectedWeekIsNull_ShouldShowGameCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var selectedSeasonYear = 1920;
            var seasons = new List<Season>
            {
                new Season { Id = selectedSeasonYear, NumOfWeeksScheduled = 3 }
            };
            var selectedSeason = seasons.FirstOrDefault(s => s.Id == selectedSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Create();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            seasonsSelectList.DataValueField.ShouldBe<string>("Id");
            seasonsSelectList.DataTextField.ShouldBe<string>("Id");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { null, 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBeNull();
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddGameToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game { };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            fakeSession.GetObject<int>("SelectedWeek").ShouldBe(game.Week);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 2,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 4",
                GuestScore = 0,
                HostName = "Host 4",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A game with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game
            {
                Id = 4,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 2",
                GuestScore = 0,
                HostName = "Host 2",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A game with the same season, week, guest, and host already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeySeasonIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game { };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Game_Season_SeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on SeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var game = new Game { };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnGameCreateView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var selectedSeasonYear = 1920;
            var seasons = new List<Season>
            {
                new Season { Id = selectedSeasonYear, NumOfWeeksScheduled = 3 }
            };
            var selectedSeason = seasons.FirstOrDefault(s => s.Id == selectedSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeasonYear);
            fakeSession.SetObject<int?>("SelectedWeek", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var game = new Game { };
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Create(gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustNotHaveHappened();
            A.CallTo(() => fakeGameService.AddGameAsync(game)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            seasonsSelectList.DataValueField.ShouldBe<string>("Id");
            seasonsSelectList.DataTextField.ShouldBe<string>("Id");
            seasonsSelectList.SelectedValue.ShouldBe(selectedSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { null, 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBeNull();

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndGameFound_ShouldReturnGameEditView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModel = new GameViewModel
            {
                SeasonYear = 1920
            };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored)).Returns(gameViewModel);

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = new Game { };
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeTeamRepository = A.Fake<ITeamRepository>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season { Id = 1920, NumOfWeeksScheduled = 3 },
                new Season { Id = 1921, NumOfWeeksScheduled = 3 },
                new Season { Id = 1922, NumOfWeeksScheduled = 3 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Seasons);
            var seasonsSelectList = (SelectList)testController.ViewBag.Seasons;
            seasonsSelectList.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            seasonsSelectList.DataValueField.ShouldBe<string>("Id");
            seasonsSelectList.DataTextField.ShouldBe<string>("Id");
            seasonsSelectList.SelectedValue.ShouldBe(gameViewModel.SeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.Weeks);
            var weeksSelectList = (SelectList)testController.ViewBag.Weeks;
            var weeks = new List<int?> { 1, 2, 3 };
            weeksSelectList.Items.ShouldBe(weeks);
            weeksSelectList.SelectedValue.ShouldBe(game.Week);

            var oldGame = fakeSession.GetObject<Game>("OldGame");
            oldGame.ShouldBeEquivalentTo(game);
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<GameViewModel>();
            resultModel.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = await testController.Edit(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsGameIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateGameInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 1;
            var game = new Game
            {
                Id = id
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
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
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            int id = 0;
            var game = new Game
            {
                Id = 1
            };
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndGameWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 1;
            var game = new Game
            {
                Id = id
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GameExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndGameWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 1;
            var game = new Game
            {
                Id = id
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            A.CallTo(() => fakeGameRepository.GameExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, gameViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 2;
            var game = new Game
            {
                Id = id,
                SeasonId = 1920,
                Week = 1,
                GuestName = "Guest 3",
                GuestScore = 0,
                HostName = "Host 3",
                HostScore = 0,
                IsPlayoff = false,
                Notes = "Notes"
            };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A game with the same season, week, guest, and host already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeySeasonIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 2;
            var game = new Game { Id = id };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Game_Season_SeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on SeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            int id = 2;
            var game = new Game { Id = id };
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(A<GameViewModel>.Ignored))
                .Returns(Task.FromResult(game));

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            var games = new List<Game>
            {
                new Game
                {
                    Id = 1,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 1",
                    GuestScore = 0,
                    HostName = "Host 1",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 2,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 2",
                    GuestScore = 0,
                    HostName = "Host 2",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
                new Game
                {
                    Id = 3,
                    SeasonId = 1920,
                    Week = 1,
                    GuestName = "Guest 3",
                    GuestScore = 0,
                    HostName = "Host 3",
                    HostScore = 0,
                    IsPlayoff = false,
                    Notes = "Notes"
                },
            };
            A.CallTo(() => fakeGameRepository.GetGamesAsync()).Returns(games);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("OldGame", game);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameRepository.Update(game)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnGameEditView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            int id = 1;
            var game = new Game
            {
                Id = 1
            };
            var gameViewModel = new GameViewModel { Game = game };
            var result = await testController.Edit(id, gameViewModel);

            // Assert
            A.CallTo(() => fakeGameViewModelMapper.MapViewModelToGame(gameViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeGameRepository.Update(game)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndGameFound_ShouldReturnGameDeleteView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();

            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var gameViewModel = new GameViewModel { };
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(A<Game>.Ignored))
                .Returns(gameViewModel);

            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = new Game { };
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeGameViewModelMapper.MapGameToViewModel(game)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<GameViewModel>();
            resultModel.ShouldBe(gameViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = await testController.Delete(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenGameNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();

            var fakeGameRepository = A.Fake<IGameRepository>();
            Game? game = null;
            A.CallTo(() => fakeGameRepository.GetGameAsync(An<int>.Ignored)).Returns(game);

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeGameRepository.GetGameAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteGameFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeGameService.DeleteGameAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSessionVariablesAndRedirectToIndex()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var seasonYear = 1920;
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            fakeSession.GetObject<int?>("SelectedSeasonYear").ShouldBe(seasonYear);
            fakeSession.GetObject<int?>("SelectedWeek").ShouldBeNull();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = testController.SetSelectedSeasonYear(null);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task SetSelectedWeek_WhenSeasonYearArgIsNotNull_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var selectedWeek = 1;
            var result = testController.SetSelectedWeek(selectedWeek);

            // Assert
            fakeSession.GetObject<int?>("SelectedWeek").ShouldBe(selectedWeek);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task SetSelectedWeek_WhenSeasonYearArgIsNull_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            var fakeGameIndexViewModel = A.Fake<IGameIndexViewModel>();
            var fakeGameDetailsViewModel = A.Fake<IGameDetailsViewModel>();
            var fakeGameViewModelMapper = A.Fake<IGameViewModelMapper>();
            var fakeGameService = A.Fake<IGameService>();
            var fakeGameRepository = A.Fake<IGameRepository>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var fakeSession = new MockHttpSession();
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new GameController(fakeGameIndexViewModel, fakeGameDetailsViewModel,
                fakeGameViewModelMapper, fakeGameService, fakeGameRepository, fakeTeamRepository, fakeSeasonRepository,
                fakeSharedRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = testController.SetSelectedWeek(null);

            // Assert
            fakeSession.GetObject<int?>("SelectedWeek").ShouldBeNull();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
