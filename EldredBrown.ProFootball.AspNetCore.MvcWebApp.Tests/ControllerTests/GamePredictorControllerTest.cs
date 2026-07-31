using System;
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

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class GamePredictorControllerTest
    {
        [Theory]
        [InlineData(null, "", 1922, "Guest1")]
        [InlineData(null, null, 1922, "Guest1")]
        [InlineData(null, "Guest", 1922, "Guest")]
        [InlineData(1920, "Guest", 1920, "Guest")]
        public async Task PredictGameGet_GuestValues_ShouldReturnTemplateFormView(
            int? guestSeasonYear, string? guestName, int expGuestSeasonYear, string expGuestName
        )
        {
            int hostSeasonYear = 1921;
            string hostName = "Host";

            (
                GamePredictorController testController, GamePrediction prediction,
                List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons
            ) = SetUp(
                guestSeasonYear: guestSeasonYear, guestName: guestName,
                hostSeasonYear: hostSeasonYear, hostName: hostName
            );

            // Act
            var result = await testController.PredictGame();

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            var orderedSeasons = seasonsFromSession.OrderByDescending(s => s.Year).ToList();
            seasonsFromSession.ShouldBe(orderedSeasons);

            var guestSeasonYearFromSession = testController.HttpContext.Session.GetObject<int>("GuestSeasonYear");
            guestSeasonYearFromSession.ShouldBe(expGuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe<string>("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe<string>("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(expGuestSeasonYear);

            prediction.GuestSeasonYear.ShouldBe(expGuestSeasonYear);

            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(expGuestSeasonYear))
                .MustHaveHappenedOnceExactly();
            var guestTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons");
            guestTeamSeasonsFromSession.ShouldBeEquivalentTo(guestTeamSeasons);

            var guestNameFromSession = testController.HttpContext.Session.GetObject<string>("GuestName");
            guestNameFromSession.ShouldBe(expGuestName);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = (SelectList)testController.ViewBag.Guests;
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(expGuestName);

            prediction.GuestName.ShouldBe(expGuestName);

            var hostSeasonYearFromSession = testController.HttpContext.Session.GetObject<int>("HostSeasonYear");
            hostSeasonYearFromSession.ShouldBe(hostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe<string>("Year");
            viewBagHostSeasons.DataTextField.ShouldBe<string>("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonYear);

            prediction.HostSeasonYear.ShouldBe(hostSeasonYear);

            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(hostSeasonYear))
                .MustHaveHappenedOnceExactly();
            var hostTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons");
            hostTeamSeasonsFromSession.ShouldBeEquivalentTo(hostTeamSeasons);

            var hostNameFromSession = testController.HttpContext.Session.GetObject<string>("HostName");
            hostNameFromSession.ShouldBe(hostName);

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
        [InlineData(1920, "Host", 1920, "Host")]
        public async Task PredictGameGet_HostValues_ShouldReturnTemplateFormView(
            int? hostSeasonYear, string? hostName, int expHostSeasonYear, string expHostName
        )
        {
            int guestSeasonYear = 1921;
            string guestName = "Guest";

            (
                GamePredictorController testController, GamePrediction prediction,
                List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons
            ) = SetUp(
                guestSeasonYear: guestSeasonYear, guestName: guestName,
                hostSeasonYear: hostSeasonYear, hostName: hostName
            );

            // Act
            var result = await testController.PredictGame();

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            var orderedSeasons = seasonsFromSession.OrderByDescending(s => s.Year).ToList();
            seasonsFromSession.ShouldBe(orderedSeasons);

            var guestSeasonYearFromSession = testController.HttpContext.Session.GetObject<int>("GuestSeasonYear");
            guestSeasonYearFromSession.ShouldBe(guestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe<string>("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe<string>("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonYear);

            prediction.GuestSeasonYear.ShouldBe(guestSeasonYear);

            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(guestSeasonYear))
                .MustHaveHappenedOnceExactly();
            var guestTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons");
            guestTeamSeasonsFromSession.ShouldBeEquivalentTo(guestTeamSeasons);

            var guestNameFromSession = testController.HttpContext.Session.GetObject<string>("GuestName");
            guestNameFromSession.ShouldBe(guestName);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = (SelectList)testController.ViewBag.Guests;
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(guestName);

            prediction.GuestName.ShouldBe(guestName);

            var hostSeasonYearFromSession = testController.HttpContext.Session.GetObject<int>("HostSeasonYear");
            hostSeasonYearFromSession.ShouldBe(expHostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe<string>("Year");
            viewBagHostSeasons.DataTextField.ShouldBe<string>("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(expHostSeasonYear);

            prediction.HostSeasonYear.ShouldBe(expHostSeasonYear);

            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(expHostSeasonYear))
                .MustHaveHappenedOnceExactly();
            var hostTeamSeasonsFromSession = testController.HttpContext.Session
                .GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons");
            hostTeamSeasonsFromSession.ShouldBeEquivalentTo(hostTeamSeasons);

            var hostNameFromSession = testController.HttpContext.Session.GetObject<string>("HostName");
            hostNameFromSession.ShouldBe(expHostName);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = (SelectList)testController.ViewBag.Hosts;
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(expHostName);

            prediction.HostName.ShouldBe(expHostName);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestAndHostTeamSeasonsBothFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            var guestTeamSeason = new TeamSeason
            {
                Id = 1,
                TeamIdNavigation = new Team
                {
                    Name = "Guest"
                }
            };
            var guestSeasonYear = 1920;

            var hostTeamSeason = new TeamSeason
            {
                Id = 2,
                TeamIdNavigation = new Team
                {
                    Name = "Host"
                }
            };
            var hostSeasonYear = 1920;

            (
                GamePredictorController testController, GamePrediction prediction, List<Season> seasons,
                List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons,
                GameScorePrediction gameScorePrediction
            ) = SetUpPredictGamePost(
                guestTeamSeason: guestTeamSeason, guestSeasonYear: guestSeasonYear,
                hostTeamSeason: hostTeamSeason, hostSeasonYear: hostSeasonYear
            );

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);
            testController.HttpContext.Session.GetObject<int>("GuestSeasonYear").ShouldBe(guestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons")
                .ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(guestTeamSeason.TeamIdNavigation.Name);

            testController.HttpContext.Session.GetObject<int>("HostSeasonYear").ShouldBe(hostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons")
                .ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(hostTeamSeason.TeamIdNavigation.Name);

            A.CallTo(() => testController._gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
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
            var hostTeamSeason = new TeamSeason
            {
                Id = 2,
                TeamIdNavigation = new Team
                {
                    Name = "Host"
                }
            };
            var hostSeasonYear = 1920;

            (
                GamePredictorController testController, GamePrediction prediction, List<Season> seasons,
                List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons,
                GameScorePrediction gameScorePrediction
            ) = SetUpPredictGamePost(hostTeamSeason: hostTeamSeason, hostSeasonYear: hostSeasonYear);

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);

            var defaultSeasonYear = 1922;
            testController.HttpContext.Session.GetObject<int>("GuestSeasonYear").ShouldBe(defaultSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(defaultSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons")
                .ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(prediction.GuestName);

            testController.HttpContext.Session.GetObject<int>("HostSeasonYear").ShouldBe(hostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons")
                .ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(hostTeamSeason.TeamIdNavigation.Name);

            A.CallTo(() => testController._gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
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
            var guestTeamSeason = new TeamSeason
            {
                Id = 1,
                TeamIdNavigation = new Team
                {
                    Name = "Guest"
                }
            };
            var guestSeasonYear = 1920;

            (
                GamePredictorController testController, GamePrediction prediction, List<Season> seasons,
                List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons,
                GameScorePrediction gameScorePrediction
            ) = SetUpPredictGamePost(guestTeamSeason: guestTeamSeason, guestSeasonYear: guestSeasonYear);

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);

            testController.HttpContext.Session.GetObject<int>("GuestSeasonYear").ShouldBe(guestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons")
                .ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(prediction.GuestName);

            var defaultSeasonYear = 1922;
            testController.HttpContext.Session.GetObject<int>("HostSeasonYear").ShouldBe(defaultSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(defaultSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons")
                .ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(prediction.HostName);

            A.CallTo(() => testController._gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
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
            var guestTeamSeason = new TeamSeason
            {
                Id = 1,
                TeamIdNavigation = new Team
                {
                    Name = "Guest"
                }
            };
            var guestSeasonYear = 1920;

            var hostTeamSeason = new TeamSeason
            {
                Id = 2,
                TeamIdNavigation = new Team
                {
                    Name = "Host"
                }
            };
            var hostSeasonYear = 1920;

            var ex = new Exception();

            (
                GamePredictorController testController, GamePrediction prediction, List<Season> seasons,
                List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons,
                GameScorePrediction gameScorePrediction
            ) = SetUpPredictGamePost(
                guestTeamSeason: guestTeamSeason, guestSeasonYear: guestSeasonYear,
                hostTeamSeason: hostTeamSeason, hostSeasonYear: hostSeasonYear,
                ex: ex
            );

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons").ShouldBeEquivalentTo(seasons);

            testController.HttpContext.Session.GetObject<int>("GuestSeasonYear").ShouldBe(guestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(guestSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("GuestTeamSeasons")
                .ShouldBeEquivalentTo(guestTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBeEquivalentTo(guestTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagGuests.SelectedValue.ShouldBe(prediction.GuestName);

            testController.HttpContext.Session.GetObject<int>("HostSeasonYear").ShouldBe(hostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBeEquivalentTo(seasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(hostSeasonYear);

            testController.HttpContext.Session.GetObject<IEnumerable<TeamSeason>>("HostTeamSeasons")
                .ShouldBeEquivalentTo(hostTeamSeasons);

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBeEquivalentTo(hostTeamSeasons.Select(ts => ts.TeamIdNavigation.Name).ToList());
            viewBagHosts.SelectedValue.ShouldBe(prediction.HostName);

            A.CallTo(() => testController._gamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                .MustHaveHappenedOnceExactly();

            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("A prediction could not be calculated for the selected teams.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        public readonly struct Filter(int? guestSeasonYear, string? guestName, int? hostSeasonYear, string? hostName)
        {
            public int? GuestSeasonYear { get; init; } = guestSeasonYear;
            public string? GuestName { get; init; } = guestName;
            public int? HostSeasonYear { get; init; } = hostSeasonYear;
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
            fakeSession.SetObject("GuestSeasonYear", startingFilter.GuestSeasonYear);
            fakeSession.SetObject("GuestName", startingFilter.GuestName);
            fakeSession.SetObject("HostSeasonYear", startingFilter.HostSeasonYear);
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
            var result = testController.ApplyFilter(newFilter.GuestSeasonYear, newFilter.GuestName,
                newFilter.HostSeasonYear, newFilter.HostName);

            // Assert
            fakeSession.GetObject<int?>("GuestSeasonYear").ShouldBe(expFilter.GuestSeasonYear);
            fakeSession.GetObject<string?>("GuestName").ShouldBe(expFilter.GuestName);
            fakeSession.GetObject<int?>("HostSeasonYear").ShouldBe(expFilter.HostSeasonYear);
            fakeSession.GetObject<string?>("HostName").ShouldBe(expFilter.HostName);

            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.PredictGame));
        }

        [Fact]
        public void SetTeamSeasonName_WhenTeamNameIsNeitherNullNorEmpty_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            GamePredictorController testController = SetUpSetters();

            // Act
            var sessionKey = "Key";
            var teamName = "Team";
            var result = testController.SetTeamSeasonName(sessionKey, teamName);

            // Assert
            testController.HttpContext.Session.GetObject<string?>(sessionKey).ShouldBe(teamName);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(Index));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SetTeamSeasonName_WhenTeamNameIsNullOrEmpty_ShouldReturnBadRequest(string? teamName)
        {
            // Arrange
            GamePredictorController testController = SetUpSetters();

            // Act
            var sessionKey = "Key";
            var result = testController.SetTeamSeasonName(sessionKey, teamName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetTeamSeasonYear_WhenSeasonYearIsNeitherNullNorEmpty_ShouldSetSessionVariableAndRedirectToIndex()
        {
            // Arrange
            GamePredictorController testController = SetUpSetters();

            // Act
            var sessionKey = "Key";
            int? seasonYear = 1920;
            var result = testController.SetTeamSeasonYear(sessionKey, seasonYear);

            // Assert
            testController.HttpContext.Session.GetObject<int?>(sessionKey).ShouldBe(seasonYear);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(Index));
        }

        [Fact]
        public void SetTeamSeasonYear_WhenSeasonYearIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            GamePredictorController testController = SetUpSetters();

            // Act
            var sessionKey = "Key";
            int? seasonYear = null;
            var result = testController.SetTeamSeasonYear(sessionKey, seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        private static (GamePredictorController, GamePrediction, List<TeamSeason>, List<TeamSeason>)
            SetUp(
                int? guestSeasonYear = null, string? guestName = null,
                int? hostSeasonYear = null, string? hostName = null
            )
        {
            // Arrange
            var prediction = new GamePrediction();
            ISeasonRepository fakeSeasonRepository = SetUpFakeSeasonRepository();
            (ITeamSeasonRepository fakeTeamSeasonRepository, List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons) =
                SetUpFakeTeamSeasonRepository();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            Mock<HttpContext> fakeHttpContext = SetUpHttpContext(guestSeasonYear, guestName, hostSeasonYear, hostName);

            var testController = new GamePredictorController(prediction, fakeSeasonRepository,
                fakeTeamSeasonRepository, fakeGamePredictorService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            return (testController, prediction, guestTeamSeasons, hostTeamSeasons);
        }

        private static (
            GamePredictorController, GamePrediction, List<Season>,
            List<TeamSeason>, List<TeamSeason>, GameScorePrediction
        ) SetUpPredictGamePost(
            TeamSeason? guestTeamSeason = null, int? guestSeasonYear = null,
            TeamSeason? hostTeamSeason = null, int? hostSeasonYear = null,
            Exception? ex = null
        )
        {
            var defaultSeasonYear = 1922;

            var prediction = new GamePrediction
            {
                GuestName = guestTeamSeason is null ? string.Empty : guestTeamSeason.TeamIdNavigation.Name,
                GuestSeasonYear = guestSeasonYear is null ? defaultSeasonYear : guestSeasonYear,
                HostName = hostTeamSeason is null ? string.Empty : hostTeamSeason.TeamIdNavigation.Name,
                HostSeasonYear = hostSeasonYear is null ? defaultSeasonYear : hostSeasonYear,
            };

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            (IGamePredictorService fakeGamePredictorService, GameScorePrediction gameScorePrediction) =
                SetUpFakeGamePredictorService(ex);
            (
                List<Season> seasons, List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons,
                Mock<HttpContext> fakeHttpContext
            ) = SetUpHttpContext(ref guestTeamSeason, ref hostTeamSeason);

            var testController = new GamePredictorController(
                prediction, fakeSeasonRepository, fakeTeamSeasonRepository, fakeGamePredictorService
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            return (testController, prediction, seasons, guestTeamSeasons, hostTeamSeasons, gameScorePrediction);
        }

        private static (
            List<Season> seasons, List<TeamSeason> guestTeamSeasons, List<TeamSeason> hostTeamSeasons, Mock<HttpContext> fakeHttpContext
        ) SetUpHttpContext(ref TeamSeason? guestTeamSeason, ref TeamSeason? hostTeamSeason)
        {
            var fakeSession = new MockHttpSession();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            fakeSession.SetObject("Seasons", seasons);

            guestTeamSeason = guestTeamSeason is null
                ? new TeamSeason { Id = 1, TeamIdNavigation = new Team { Name = "Guest" } }
                : guestTeamSeason;
            var guestTeamSeasons = new List<TeamSeason> { guestTeamSeason };
            fakeSession.SetObject("GuestTeamSeasons", guestTeamSeasons);

            hostTeamSeason = hostTeamSeason is null
                ? new TeamSeason { Id = 1, TeamIdNavigation = new Team { Name = "Host" } }
                : hostTeamSeason;
            var hostTeamSeasons = new List<TeamSeason> { hostTeamSeason };
            fakeSession.SetObject("HostTeamSeasons", hostTeamSeasons);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            return (seasons, guestTeamSeasons, hostTeamSeasons, fakeHttpContext);
        }

        private static (IGamePredictorService fakeGamePredictorService, GameScorePrediction gameScorePrediction)
            SetUpFakeGamePredictorService(Exception? ex)
        {
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var gameScorePrediction = new GameScorePrediction
            {
                GuestScore = 0,
                HostScore = 0,
            };
            if (ex is null)
            {
                A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                    .Returns(gameScorePrediction);
            }
            else
            {
                A.CallTo(() => fakeGamePredictorService.PredictGameScore(A<TeamSeason>.Ignored, A<TeamSeason>.Ignored))
                    .Throws(ex);
            }

            return (fakeGamePredictorService, gameScorePrediction);
        }

        private static ISeasonRepository SetUpFakeSeasonRepository()
        {
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);
            return fakeSeasonRepository;
        }

        private static (ITeamSeasonRepository, List<TeamSeason>, List<TeamSeason>) SetUpFakeTeamSeasonRepository()
        {
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

            return (fakeTeamSeasonRepository, guestTeamSeasons, hostTeamSeasons);
        }

        private static Mock<HttpContext> SetUpHttpContext(int? guestSeasonYear, string? guestName, int? hostSeasonYear, string? hostName)
        {
            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("GuestSeasonYear", guestSeasonYear);
            fakeSession.SetObject("GuestName", guestName);
            fakeSession.SetObject("HostSeasonYear", hostSeasonYear);
            fakeSession.SetObject("HostName", hostName);

            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            return fakeHttpContext;
        }

        private static GamePredictorController SetUpSetters()
        {
            var prediction = new GamePrediction();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeGamePredictorService = A.Fake<IGamePredictorService>();
            var fakeSession = new MockHttpSession();
            var fakeHttpContext = new Mock<HttpContext>();
            fakeHttpContext.Setup(x => x.Session).Returns(fakeSession);

            var testController = new GamePredictorController(
                prediction, fakeSeasonRepository, fakeTeamSeasonRepository, fakeGamePredictorService
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = fakeHttpContext.Object
                }
            };

            return testController;
        }
    }
}
