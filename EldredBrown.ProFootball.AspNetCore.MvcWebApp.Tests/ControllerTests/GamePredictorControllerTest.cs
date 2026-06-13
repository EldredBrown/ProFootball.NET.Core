using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using FakeItEasy;
using Moq;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;
using System;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class GamePredictorControllerTest
    {
        [Theory]
        [InlineData(null, "", 1922, "Guest1")]
        [InlineData(null, null, 1922, "Guest1")]
        [InlineData(null, "Guest", 1922, "Guest")]
        [InlineData(1920, "Guest", 1920, "Guest")]
        public async Task PredictGameGet_GuestValues_ShouldReturnTemplateFormView(int? guestSeasonId, string? guestName,
            int expGuestSeasonId, string expGuestName)
        {
            // Arrange
            var prediction = new GamePrediction();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestTeamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamIdNavigation = new Team { Id = 1, Name = "Guest1" } },
                new() { Id = 2, TeamIdNavigation = new Team { Id = 2, Name = "Guest2" } },
                new() { Id = 3, TeamIdNavigation = new Team { Id = 3, Name = "Guest3" } },
            };
            var hostTeamSeasons = new List<TeamSeason>
            {
                new() { Id = 4, TeamIdNavigation = new Team { Id = 4, Name = "Host1" } },
                new() { Id = 5, TeamIdNavigation = new Team { Id = 5, Name = "Host2" } },
                new() { Id = 6, TeamIdNavigation = new Team { Id = 6, Name = "Host3" } },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .ReturnsNextFromSequence(guestTeamSeasons, hostTeamSeasons);

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();

            fakeSession.SetObject("GuestSeasonId", guestSeasonId);
            fakeSession.SetObject("GuestName", guestName);

            int? hostSeasonId = 1921;
            fakeSession.SetObject("HostSeasonId", hostSeasonId);

            string hostName = "Host";
            fakeSession.SetObject("HostName", hostName);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = await testController.PredictGame();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            var orderedSeasons = seasonsFromSession.OrderByDescending(s => s.Id).ToList();
            seasonsFromSession.ShouldBe(orderedSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe<string>("Id");
            viewBagGuestSeasons.DataTextField.ShouldBe<string>("Id");
            viewBagGuestSeasons.SelectedValue.ShouldBe(expGuestSeasonId);

            prediction.GuestSeasonId.ShouldBe(expGuestSeasonId);

            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(expGuestSeasonId))
                .MustHaveHappenedOnceExactly();
            var guestTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons");
            guestTeamSeasonsFromSession.ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = (SelectList)testController.ViewBag.Guests;
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(expGuestName);

            prediction.GuestName.ShouldBe(expGuestName);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe<string>("Id");
            viewBagHostSeasons.DataTextField.ShouldBe<string>("Id");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonId);

            prediction.HostSeasonId.ShouldBe(hostSeasonId.Value);

            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(hostSeasonId.Value))
                .MustHaveHappenedOnceExactly();
            var hostTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons");
            hostTeamSeasonsFromSession.ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = (SelectList)testController.ViewBag.Hosts;
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(hostName);

            prediction.HostName.ShouldBe(hostName);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGameGet_WhenHostSeasonIdIsNotNullAndHostNameIsNeitherNullNorEmpty_ShouldReturnTemplateFormView()
        {
            // Arrange
            var prediction = new GamePrediction();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestTeamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamIdNavigation = new Team { Id = 1, Name = "Guest1" } },
                new() { Id = 2, TeamIdNavigation = new Team { Id = 2, Name = "Guest2" } },
                new() { Id = 3, TeamIdNavigation = new Team { Id = 3, Name = "Guest3" } },
            };
            var hostTeamSeasons = new List<TeamSeason>
            {
                new() { Id = 4, TeamIdNavigation = new Team { Id = 4, Name = "Host1" } },
                new() { Id = 5, TeamIdNavigation = new Team { Id = 5, Name = "Host2" } },
                new() { Id = 6, TeamIdNavigation = new Team { Id = 6, Name = "Host3" } },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .ReturnsNextFromSequence(guestTeamSeasons, hostTeamSeasons);

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();

            int? guestSeasonId = null;
            fakeSession.SetObject("GuestSeasonId", guestSeasonId);

            string guestName = string.Empty;
            fakeSession.SetObject("GuestName", guestName);

            int? hostSeasonId = 1921;
            fakeSession.SetObject("HostSeasonId", hostSeasonId);

            string hostName = "Host";
            fakeSession.SetObject("HostName", hostName);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = await testController.PredictGame();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            var orderedSeasons = seasonsFromSession.OrderByDescending(s => s.Id).ToList();
            seasonsFromSession.ShouldBe(orderedSeasons);

            var defaultSeasonYear = 1922;

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe<string>("Id");
            viewBagGuestSeasons.DataTextField.ShouldBe<string>("Id");
            viewBagGuestSeasons.SelectedValue.ShouldBe(defaultSeasonYear);

            prediction.GuestSeasonId.ShouldBe(defaultSeasonYear);

            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(defaultSeasonYear))
                .MustHaveHappenedOnceExactly();
            var guestTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons");
            guestTeamSeasonsFromSession.ShouldBeEquivalentTo(guestTeamSeasons);

            var defaultGuestName = guestTeamSeasons.First().TeamIdNavigation.Name;

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = (SelectList)testController.ViewBag.Guests;
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(defaultGuestName);

            prediction.GuestName.ShouldBe(defaultGuestName);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe<string>("Id");
            viewBagHostSeasons.DataTextField.ShouldBe<string>("Id");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonId);

            prediction.HostSeasonId.ShouldBe(hostSeasonId.Value);

            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(hostSeasonId.Value))
                .MustHaveHappenedOnceExactly();
            var hostTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons");
            hostTeamSeasonsFromSession.ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = (SelectList)testController.ViewBag.Hosts;
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(hostName);

            prediction.HostName.ShouldBe(hostName);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Theory]
        [InlineData(null, "", 1922, "Host1")]
        [InlineData(null, null, 1922, "Host1")]
        [InlineData(null, "Host", 1922, "Host")]
        public async Task PredictGameGet_HostValues_ShouldReturnTemplateFormView(int? hostSeasonId, string? hostName,
            int expHostSeasonId, string expHostName)
        {
            // Arrange
            var prediction = new GamePrediction();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var guestTeamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamIdNavigation = new Team { Id = 1, Name = "Guest1" } },
                new() { Id = 2, TeamIdNavigation = new Team { Id = 2, Name = "Guest2" } },
                new() { Id = 3, TeamIdNavigation = new Team { Id = 3, Name = "Guest3" } },
            };
            var hostTeamSeasons = new List<TeamSeason>
            {
                new() { Id = 4, TeamIdNavigation = new Team { Id = 4, Name = "Host1" } },
                new() { Id = 5, TeamIdNavigation = new Team { Id = 5, Name = "Host2" } },
                new() { Id = 6, TeamIdNavigation = new Team { Id = 6, Name = "Host3" } },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored))
                .ReturnsNextFromSequence(guestTeamSeasons, hostTeamSeasons);

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();

            int? guestSeasonId = null;
            fakeSession.SetObject("GuestSeasonId", guestSeasonId);

            string guestName = string.Empty;
            fakeSession.SetObject("GuestName", guestName);

            fakeSession.SetObject("HostSeasonId", hostSeasonId);
            fakeSession.SetObject("HostName", hostName);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = await testController.PredictGame();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            var orderedSeasons = seasonsFromSession.OrderByDescending(s => s.Id).ToList();
            seasonsFromSession.ShouldBe(orderedSeasons);

            var defaultSeasonYear = 1922;

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe<string>("Id");
            viewBagGuestSeasons.DataTextField.ShouldBe<string>("Id");
            viewBagGuestSeasons.SelectedValue.ShouldBe(defaultSeasonYear);

            prediction.GuestSeasonId.ShouldBe(defaultSeasonYear);

            var guestTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons");
            guestTeamSeasonsFromSession.ShouldBeEquivalentTo(guestTeamSeasons);

            var defaultGuestName = guestTeamSeasons.First().TeamIdNavigation.Name;

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = (SelectList)testController.ViewBag.Guests;
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(defaultGuestName);

            prediction.GuestName.ShouldBe(defaultGuestName);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe<string>("Id");
            viewBagHostSeasons.DataTextField.ShouldBe<string>("Id");
            viewBagHostSeasons.SelectedValue.ShouldBe(expHostSeasonId);

            prediction.HostSeasonId.ShouldBe(expHostSeasonId);

            var hostTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons");
            hostTeamSeasonsFromSession.ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = (SelectList)testController.ViewBag.Hosts;
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(expHostName);

            prediction.HostName.ShouldBe(expHostName);

            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(defaultSeasonYear))
                .MustHaveHappenedTwiceExactly();

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestAndHostTeamSeasonsBothFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            TeamSeason? guestTeamSeason = new() { Id = 1, TeamIdNavigation = new Team { Name = "Guest" } };
            var guestSeasonId = 1920;
            TeamSeason? hostTeamSeason = new() { Id = 2, TeamIdNavigation = new Team { Name = "Host" } };
            var hostSeasonId = 1921;
            var prediction = new GamePrediction
            {
                GuestName = guestTeamSeason.TeamIdNavigation.Name,
                GuestSeasonId = guestSeasonId,
                HostName = hostTeamSeason.TeamIdNavigation.Name,
                HostSeasonId = hostSeasonId,
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .Returns(gameScorePrediction);

            var fakeSession = new MockHttpSession();

            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            fakeSession.SetObject("Seasons", seasons);

            var guestTeamSeasons = new List<TeamSeason> { guestTeamSeason };
            fakeSession.SetObject("GuestTeamSeasons", guestTeamSeasons);

            var hostTeamSeasons = new List<TeamSeason> { hostTeamSeason };
            fakeSession.SetObject("HostTeamSeasons", hostTeamSeasons);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            fakeSession.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);

            fakeSession.GetObject<int>("GuestSeasonId").ShouldBe(guestSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Id");
            viewBagGuestSeasons.DataTextField.ShouldBe("Id");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonId);    

            fakeSession.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons").ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(guestTeamSeason.TeamIdNavigation.Name);

            fakeSession.GetObject<int>("HostSeasonId").ShouldBe(hostSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Id");
            viewBagHostSeasons.DataTextField.ShouldBe("Id");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonId);

            fakeSession.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons").ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(hostTeamSeason.TeamIdNavigation.Name);

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            prediction.GuestScore.ShouldBe(gameScorePrediction.GuestScore.Value);
            prediction.HostScore.ShouldBe(gameScorePrediction.HostScore.Value);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestTeamSeasonNotFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            TeamSeason? guestTeamSeason = new() { Id = 1, TeamIdNavigation = new Team { Name = "Guest" } };
            var guestSeasonId = 1920;
            TeamSeason? hostTeamSeason = new() { Id = 2, TeamIdNavigation = new Team { Name = "Host" } };
            var hostSeasonId = 1921;
            var prediction = new GamePrediction
            {
                GuestName = guestTeamSeason.TeamIdNavigation.Name,
                GuestSeasonId = guestSeasonId,
                HostName = hostTeamSeason.TeamIdNavigation.Name,
                HostSeasonId = hostSeasonId,
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .Returns(gameScorePrediction);

            var fakeSession = new MockHttpSession();

            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            fakeSession.SetObject("Seasons", seasons);

            var guestTeamSeasons = new List<TeamSeason> { };
            fakeSession.SetObject("GuestTeamSeasons", guestTeamSeasons);

            var hostTeamSeasons = new List<TeamSeason> { hostTeamSeason };
            fakeSession.SetObject("HostTeamSeasons", hostTeamSeasons);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            fakeSession.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);

            fakeSession.GetObject<int>("GuestSeasonId").ShouldBe(guestSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Id");
            viewBagGuestSeasons.DataTextField.ShouldBe("Id");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonId);

            fakeSession.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons").ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(prediction.GuestName);

            fakeSession.GetObject<int>("HostSeasonId").ShouldBe(hostSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Id");
            viewBagHostSeasons.DataTextField.ShouldBe("Id");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonId);

            fakeSession.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons").ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(hostTeamSeason.TeamIdNavigation.Name);

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            prediction.GuestScore.ShouldBe(gameScorePrediction.GuestScore.Value);
            prediction.HostScore.ShouldBe(gameScorePrediction.HostScore.Value);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenHostTeamSeasonNotFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            TeamSeason? guestTeamSeason = new() { Id = 1, TeamIdNavigation = new Team { Name = "Guest" } };
            var guestSeasonId = 1920;
            TeamSeason? hostTeamSeason = new() { Id = 2, TeamIdNavigation = new Team { Name = "Host" } };
            var hostSeasonId = 1921;
            var prediction = new GamePrediction
            {
                GuestName = guestTeamSeason.TeamIdNavigation.Name,
                GuestSeasonId = guestSeasonId,
                HostName = hostTeamSeason.TeamIdNavigation.Name,
                HostSeasonId = hostSeasonId,
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .Returns(gameScorePrediction);

            var fakeSession = new MockHttpSession();

            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            fakeSession.SetObject("Seasons", seasons);

            var guestTeamSeasons = new List<TeamSeason> { };
            fakeSession.SetObject("GuestTeamSeasons", guestTeamSeasons);

            var hostTeamSeasons = new List<TeamSeason> { };
            fakeSession.SetObject("HostTeamSeasons", hostTeamSeasons);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            fakeSession.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);

            fakeSession.GetObject<int>("GuestSeasonId").ShouldBe(guestSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Id");
            viewBagGuestSeasons.DataTextField.ShouldBe("Id");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonId);

            fakeSession.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons").ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(prediction.GuestName);

            fakeSession.GetObject<int>("HostSeasonId").ShouldBe(hostSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Id");
            viewBagHostSeasons.DataTextField.ShouldBe("Id");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonId);

            fakeSession.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons").ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(prediction.HostName);

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            prediction.GuestScore.ShouldBe(gameScorePrediction.GuestScore.Value);
            prediction.HostScore.ShouldBe(gameScorePrediction.HostScore.Value);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenPredictGameScoreThrowsException_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            TeamSeason? guestTeamSeason = new() { Id = 1, TeamIdNavigation = new Team { Name = "Guest" } };
            var guestSeasonId = 1920;
            TeamSeason? hostTeamSeason = new() { Id = 2, TeamIdNavigation = new Team { Name = "Host" } };
            var hostSeasonId = 1921;
            var prediction = new GamePrediction
            {
                GuestName = guestTeamSeason.TeamIdNavigation.Name,
                GuestSeasonId = guestSeasonId,
                HostName = hostTeamSeason.TeamIdNavigation.Name,
                HostSeasonId = hostSeasonId,
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .Throws<Exception>();

            var fakeSession = new MockHttpSession();

            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            fakeSession.SetObject("Seasons", seasons);

            var guestTeamSeasons = new List<TeamSeason> { };
            fakeSession.SetObject("GuestTeamSeasons", guestTeamSeasons);

            var hostTeamSeasons = new List<TeamSeason> { };
            fakeSession.SetObject("HostTeamSeasons", hostTeamSeasons);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            fakeSession.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);

            fakeSession.GetObject<int>("GuestSeasonId").ShouldBe(guestSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Id");
            viewBagGuestSeasons.DataTextField.ShouldBe("Id");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonId);

            fakeSession.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons").ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(prediction.GuestName);

            fakeSession.GetObject<int>("HostSeasonId").ShouldBe(hostSeasonId);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Id");
            viewBagHostSeasons.DataTextField.ShouldBe("Id");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonId);

            fakeSession.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons").ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(prediction.HostName);

            A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();
            // Test code here.
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("A prediction could not be calculated for the selected teams.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        public readonly struct Filter(int? guestSeasonId, string? guestName, int? hostSeasonId, string? hostName)
        {
            public int? GuestSeasonId { get; init; } = guestSeasonId;
            public string? GuestName { get; init; } = guestName;
            public int? HostSeasonId { get; init; } = hostSeasonId;
            public string? HostName { get; init; } = hostName;
        }

        public static TheoryData<Filter, Filter, Filter> FilterCases => new()
        {
            {
                new Filter(null, null, null, null),
                new Filter(1920, "Guest", 1921, "Host"),
                new Filter(1920, "Guest", 1921, "Host")
            },
            {
                new Filter(null, null, null, "Host"),
                new Filter(1920, "Guest", 1921, string.Empty),
                new Filter(1920, "Guest", 1921, "Host")
            },
            {
                new Filter(null, null, null, "Host"),
                new Filter(1920, "Guest", 1921, null),
                new Filter(1920, "Guest", 1921, "Host")
            },
            {
                new Filter(null, null, 1921, "Host"),
                new Filter(1920, "Guest", null, null),
                new Filter(1920, "Guest", 1921, "Host")
            },
            {
                new Filter(null, "Guest", 1921, "Host"),
                new Filter(1920, string.Empty, null, null),
                new Filter(1920, "Guest", 1921, "Host")
            },
            {
                new Filter(null, "Guest", 1921, "Host"),
                new Filter(1920, null, null, null),
                new Filter(1920, "Guest", 1921, "Host")
            },
            {
                new Filter(1920, "Guest", 1921, "Host"),
                new Filter(null, null, null, null),
                new Filter(1920, "Guest", 1921, "Host")
            },
        };

        [Theory]
        [MemberData(nameof(FilterCases))]
        public void ApplyFilter_ShouldApplyCorrectFilterAndRedirectToGamePredictorView(
            Filter startingFilter, Filter newFilter, Filter expFilter)
        {
            // Arrange
            var prediction = new GamePrediction();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();

            fakeSession.SetObject("GuestSeasonId", startingFilter.GuestSeasonId);
            fakeSession.SetObject("GuestName", startingFilter.GuestName);
            fakeSession.SetObject("HostSeasonId", startingFilter.HostSeasonId);
            fakeSession.SetObject("HostName", startingFilter.HostName);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            // Act
            var result = testController.ApplyFilter(newFilter.GuestSeasonId, newFilter.GuestName,
                newFilter.HostSeasonId, newFilter.HostName);

            // Assert
            fakeSession.GetObject<int?>("GuestSeasonId").ShouldBe(expFilter.GuestSeasonId);
            fakeSession.GetObject<string?>("GuestName").ShouldBe(expFilter.GuestName);
            fakeSession.GetObject<int?>("HostSeasonId").ShouldBe(expFilter.HostSeasonId);
            fakeSession.GetObject<string?>("HostName").ShouldBe(expFilter.HostName);

            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.PredictGame));
        }

        [Fact]
        public void SetTeamSeasonName_WhenTeamNameIsNeitherNullNorEmpty_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            var prediction = new GamePrediction();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();
            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            var sessionKey = "Key";
            var teamName = "Team";

            // Act
            var result = testController.SetTeamSeasonName(sessionKey, teamName);

            // Assert
            fakeSession.GetObject<string?>(sessionKey).ShouldBe(teamName);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetTeamSeasonName_WhenTeamNameIsNullOrEmpty_ShouldReturnBadRequest(string? teamName)
        {
            // Arrange
            var prediction = new GamePrediction();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();
            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            var sessionKey = "Key";

            // Act
            var result = testController.SetTeamSeasonName(sessionKey, teamName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetTeamSeasonYear_WhenSeasonYearIsNeitherNullNorEmpty_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            var prediction = new GamePrediction();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();
            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            var sessionKey = "Key";
            int? seasonYear = 1920;

            // Act
            var result = testController.SetTeamSeasonYear(sessionKey, seasonYear);

            // Assert
            fakeSession.GetObject<int?>(sessionKey).ShouldBe(seasonYear);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(Index));
        }

        [Fact]
        public void SetTeamSeasonYear_WhenSeasonYearIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var prediction = new GamePrediction();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();

            var fakeSession = new MockHttpSession();
            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            var sessionKey = "Key";
            int? seasonYear = null;

            // Act
            var result = testController.SetTeamSeasonYear(sessionKey, seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
