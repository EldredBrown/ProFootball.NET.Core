using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Session;

using FakeItEasy;
using Moq;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;
using System;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class GamePredictorControllerTest
    {
        private readonly Mock<ISession> _mockSession;
        private readonly Mock<HttpContext> _mockHttpContext;

        public GamePredictorControllerTest()
        {
            _mockSession = new Mock<ISession>();
            _mockHttpContext = new Mock<HttpContext>();

            // Wire session to HttpContext
            _mockHttpContext.Setup(x => x.Session).Returns(_mockSession.Object);
        }

        [Fact]
        public async Task PredictGameGet_ShouldReturnTemplateFormView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 1920;
            GamePredictorController.HostSeasonYear = 1921;

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guests = new List<TeamSeason>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.GuestSeasonYear))
                .Returns(guests);

            var hosts = new List<TeamSeason>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.HostSeasonYear))
                .Returns(hosts);

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService);

            // In your test setup:
            var httpContext = new DefaultHttpContext();
            httpContext.Features.Set<ISessionFeature>(new SessionFeature { Session = new TestSession() });
            testController.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await testController.PredictGame();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBe(seasons);

            var orderedSeasons = seasonsFromSession.OrderByDescending(s => s.Year);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBe(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe<string>("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe<string>("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBe(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe<string>("Year");
            viewBagHostSeasons.DataTextField.ShouldBe<string>("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.GuestSeasonYear))
                .MustHaveHappenedOnceExactly();
            var guestTeamSeasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>(
                "GuestTeamSeasons");
            guestTeamSeasonsFromSession.ShouldBe(guests);
            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = (SelectList)testController.ViewBag.Guests;
            viewBagGuests.Items.ShouldBe(guests);
            viewBagGuests.DataValueField.ShouldBe("TeamName");
            viewBagGuests.DataTextField.ShouldBe("TeamName");

            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.HostSeasonYear))
                .MustHaveHappenedOnceExactly();
            var hostTeamSeasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>(
                "HostTeamSeasons");
            hostTeamSeasonsFromSession.ShouldBe(hosts);
            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHosts = (SelectList)testController.ViewBag.Hosts;
            viewBagHosts.Items.ShouldBe(hosts);
            viewBagHosts.DataValueField.ShouldBe("TeamName");
            viewBagHosts.DataTextField.ShouldBe("TeamName");

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestAndHostBothFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .Returns(gameScorePrediction);

            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };

            var guestSeasonYear = 1920;
            var hostSeasonYear = 1921;
            var seasons = new List<Season>
            {
                new Season() { Year = guestSeasonYear },
                new Season() { Year = hostSeasonYear },
            };
            SetupSessionObject("Seasons", seasons);

            var guest = new TeamSeason { TeamName = "Guest" };
            var guestTeamSeasons = new List<TeamSeason> { guest };
            SetupSessionObject("GuestTeamSeasons", guestTeamSeasons);

            var host = new TeamSeason { TeamName = "Host" };
            var hostTeamSeasons = new List<TeamSeason> { host };
            SetupSessionObject("HostTeamSeasons", hostTeamSeasons);

            var prediction = new GamePrediction
            {
                GuestName = guest.TeamName,
                GuestSeasonYear = guestSeasonYear,
                HostName = host.TeamName,
                HostSeasonYear = hostSeasonYear,
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            byte[] anyBytes;

            _mockSession.Verify(s => s.TryGetValue("Seasons", out anyBytes), Moq.Times.Once);

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GamePredictorController.GuestSeasonYear.ShouldBe(prediction.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("GuestTeamSeasons", out anyBytes), Moq.Times.Once);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons);
            viewBagGuests.DataValueField.ShouldBe<string>("TeamName");
            viewBagGuests.DataTextField.ShouldBe<string>("TeamName");
            viewBagGuests.SelectedValue.ShouldBe(guest.TeamName);

            GamePredictorController.HostSeasonYear.ShouldBe(prediction.HostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("HostTeamSeasons", out anyBytes), Moq.Times.Once);
            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons);
            viewBagHosts.DataValueField.ShouldBe<string>("TeamName");
            viewBagHosts.DataTextField.ShouldBe<string>("TeamName");
            viewBagHosts.SelectedValue.ShouldBe(host.TeamName);

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            prediction.GuestScore.ShouldBe(gameScorePrediction.GuestScore.Value);
            prediction.HostScore.ShouldBe(gameScorePrediction.HostScore.Value);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestNotFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .Returns(gameScorePrediction);

            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };

            var guestSeasonYear = 1920;
            var hostSeasonYear = 1921;
            var seasons = new List<Season>
            {
                new Season() { Year = guestSeasonYear },
                new Season() { Year = hostSeasonYear },
            };
            SetupSessionObject("Seasons", seasons);

            var guestTeamSeasons = new List<TeamSeason> { };
            SetupSessionObject("GuestTeamSeasons", guestTeamSeasons);

            var host = new TeamSeason { TeamName = "Host" };
            var hostTeamSeasons = new List<TeamSeason> { host };
            SetupSessionObject("HostTeamSeasons", hostTeamSeasons);

            var prediction = new GamePrediction
            {
                GuestName = "Guest",
                GuestSeasonYear = guestSeasonYear,
                HostName = host.TeamName,
                HostSeasonYear = hostSeasonYear,
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            byte[] anyBytes;

            _mockSession.Verify(s => s.TryGetValue("Seasons", out anyBytes), Moq.Times.Once);

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GamePredictorController.GuestSeasonYear.ShouldBe(prediction.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("GuestTeamSeasons", out anyBytes), Moq.Times.Once);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons);
            viewBagGuests.DataValueField.ShouldBe<string>("TeamName");
            viewBagGuests.DataTextField.ShouldBe<string>("TeamName");
            viewBagGuests.SelectedValue.ShouldBeNull();

            GamePredictorController.HostSeasonYear.ShouldBe(prediction.HostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("HostTeamSeasons", out anyBytes), Moq.Times.Once);
            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons);
            viewBagHosts.DataValueField.ShouldBe<string>("TeamName");
            viewBagHosts.DataTextField.ShouldBe<string>("TeamName");
            viewBagHosts.SelectedValue.ShouldBe(host.TeamName);

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            prediction.GuestScore.ShouldBe(gameScorePrediction.GuestScore.Value);
            prediction.HostScore.ShouldBe(gameScorePrediction.HostScore.Value);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenHostNotFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .Returns(gameScorePrediction);

            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };

            var guestSeasonYear = 1920;
            var hostSeasonYear = 1921;
            var seasons = new List<Season>
            {
                new Season() { Year = guestSeasonYear },
                new Season() { Year = hostSeasonYear },
            };
            SetupSessionObject("Seasons", seasons);

            var guestTeamSeasons = new List<TeamSeason> { };
            SetupSessionObject("GuestTeamSeasons", guestTeamSeasons);

            var hostTeamSeasons = new List<TeamSeason> { };
            SetupSessionObject("HostTeamSeasons", hostTeamSeasons);

            var prediction = new GamePrediction
            {
                GuestName = "Guest",
                GuestSeasonYear = guestSeasonYear,
                HostName = "Host",
                HostSeasonYear = hostSeasonYear,
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            byte[] anyBytes;

            _mockSession.Verify(s => s.TryGetValue("Seasons", out anyBytes), Moq.Times.Once);

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GamePredictorController.GuestSeasonYear.ShouldBe(prediction.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("GuestTeamSeasons", out anyBytes), Moq.Times.Once);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons);
            viewBagGuests.DataValueField.ShouldBe<string>("TeamName");
            viewBagGuests.DataTextField.ShouldBe<string>("TeamName");
            viewBagGuests.SelectedValue.ShouldBeNull();

            GamePredictorController.HostSeasonYear.ShouldBe(prediction.HostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("HostTeamSeasons", out anyBytes), Moq.Times.Once);
            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons);
            viewBagHosts.DataValueField.ShouldBe<string>("TeamName");
            viewBagHosts.DataTextField.ShouldBe<string>("TeamName");
            viewBagHosts.SelectedValue.ShouldBeNull();

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            prediction.GuestScore.ShouldBe(gameScorePrediction.GuestScore.Value);
            prediction.HostScore.ShouldBe(gameScorePrediction.HostScore.Value);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenPredictionCannotBeCalculated_ShouldShowCorrectErrorMessage()
        {
            // Arrange
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .Throws<Exception>();

            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _mockHttpContext.Object
                }
            };

            var guestSeasonYear = 1920;
            var hostSeasonYear = 1921;
            var seasons = new List<Season>
            {
                new Season() { Year = guestSeasonYear },
                new Season() { Year = hostSeasonYear },
            };
            SetupSessionObject("Seasons", seasons);

            var guest = new TeamSeason { TeamName = "Guest" };
            var guestTeamSeasons = new List<TeamSeason> { guest };
            SetupSessionObject("GuestTeamSeasons", guestTeamSeasons);

            var host = new TeamSeason { TeamName = "Host" };
            var hostTeamSeasons = new List<TeamSeason> { host };
            SetupSessionObject("HostTeamSeasons", hostTeamSeasons);

            var prediction = new GamePrediction
            {
                GuestName = guest.TeamName,
                GuestSeasonYear = guestSeasonYear,
                HostName = host.TeamName,
                HostSeasonYear = hostSeasonYear,
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            byte[] anyBytes;

            _mockSession.Verify(s => s.TryGetValue("Seasons", out anyBytes), Moq.Times.Once);

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GamePredictorController.GuestSeasonYear.ShouldBe(prediction.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("GuestTeamSeasons", out anyBytes), Moq.Times.Once);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons);
            viewBagGuests.DataValueField.ShouldBe<string>("TeamName");
            viewBagGuests.DataTextField.ShouldBe<string>("TeamName");
            viewBagGuests.SelectedValue.ShouldBe(guest.TeamName);

            GamePredictorController.HostSeasonYear.ShouldBe(prediction.HostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            _mockSession.Verify(s => s.TryGetValue("HostTeamSeasons", out anyBytes), Moq.Times.Once);
            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons);
            viewBagHosts.DataValueField.ShouldBe<string>("TeamName");
            viewBagHosts.DataTextField.ShouldBe<string>("TeamName");
            viewBagHosts.SelectedValue.ShouldBe(host.TeamName);

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(An<ITeamSeason>.Ignored, An<ITeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState[""].Errors[0].ErrorMessage.ShouldBe("A prediction could not be calculated for the selected teams.");
            prediction.GuestScore.ShouldBeNull();
            prediction.HostScore.ShouldBeNull();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public void ApplyFilter_WhenGuestSeasonYearAndHostSeasonYearAreNotNull_ShouldNotApplyFilterAndShouldRedirectToGamePredictorView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 0;
            GamePredictorController.HostSeasonYear = 0;

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService);

            int? guestSeasonYear = 1920;
            int? hostSeasonYear = 1921;

            // Act
            var result = testController.ApplyFilter(guestSeasonYear, hostSeasonYear);

            // Assert
            GamePredictorController.GuestSeasonYear.ShouldBe(guestSeasonYear.Value);
            GamePredictorController.HostSeasonYear.ShouldBe(hostSeasonYear.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.PredictGame));
        }

        [Fact]
        public void ApplyFilter_WhenGuestSeasonYearIsNull_ShouldNotApplyFilterAndShouldRedirectToGamePredictorView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 0;
            GamePredictorController.HostSeasonYear = 0;
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService);

            int? guestSeasonYear = null;
            int? hostSeasonYear = 1920;

            // Act
            var result = testController.ApplyFilter(guestSeasonYear, hostSeasonYear);

            // Assert
            GamePredictorController.GuestSeasonYear.ShouldBe(0);
            GamePredictorController.HostSeasonYear.ShouldBe(hostSeasonYear.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.PredictGame));
        }

        [Fact]
        public void ApplyFilter_WhenHostSeasonYearIsNull_ShouldNotApplyFilterAndShouldRedirectToGamePredictorView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 0;
            GamePredictorController.HostSeasonYear = 0;
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var testController = new GamePredictorController(fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeGamePredictorService);

            int? guestSeasonYear = null;
            int? hostSeasonYear = null;

            // Act
            var result = testController.ApplyFilter(guestSeasonYear, hostSeasonYear);

            // Assert
            GamePredictorController.GuestSeasonYear.ShouldBe(0);
            GamePredictorController.HostSeasonYear.ShouldBe(0);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.PredictGame));
        }

        private void SetupSessionObject<T>(string key, T value)
        {
            var serialized = JsonSerializer.Serialize(value);
            var bytes = System.Text.Encoding.UTF8.GetBytes(serialized);

            _mockSession
                .Setup(s => s.TryGetValue(key, out bytes))
                .Returns(true);
        }
    }
}
