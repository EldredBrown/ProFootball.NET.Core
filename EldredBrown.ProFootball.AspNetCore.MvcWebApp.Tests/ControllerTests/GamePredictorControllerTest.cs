using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class GamePredictorControllerTest
    {
        [Fact]
        public async Task PredictGameGet_ShouldReturnTemplateFormView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 1920;
            GamePredictorController.HostSeasonYear = 1921;

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guests = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.GuestSeasonYear))
                .Returns(guests);

            var hosts = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.HostSeasonYear))
                .Returns(hosts);

            var testController = new GamePredictorController(seasonRepository, teamSeasonRepository);

            // Act
            var result = await testController.PredictGame();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

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

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.GuestSeasonYear))
                .MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = (SelectList)testController.ViewBag.Guests;
            viewBagGuests.Items.ShouldBe(guests);
            viewBagGuests.DataValueField.ShouldBe("TeamName");
            viewBagGuests.DataTextField.ShouldBe("TeamName");

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.HostSeasonYear))
                .MustHaveHappenedOnceExactly();
            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);
            var viewBagHosts = (SelectList)testController.ViewBag.Hosts;
            viewBagHosts.Items.ShouldBe(guests);
            viewBagHosts.DataValueField.ShouldBe("TeamName");
            viewBagHosts.DataTextField.ShouldBe("TeamName");

            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestAndHostBothNotFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            int guestSeasonYear = 1922;
            int hostSeasonYear = 1923;

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guests = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(guestSeasonYear)).Returns(guests);
            TeamSeason? guest = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, guestSeasonYear))
                .Returns(guest);

            var hosts = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(hostSeasonYear)).Returns(hosts);
            TeamSeason? host = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, hostSeasonYear))
                .Returns(host);

            var testController = new GamePredictorController(seasonRepository, teamSeasonRepository);

            var prediction = new GamePrediction
            {
                GuestSeasonYear = guestSeasonYear,
                HostSeasonYear = hostSeasonYear
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GamePredictorController.GuestSeasonYear.ShouldBe(prediction.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBe(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.GuestSeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(prediction.GuestName,
                GamePredictorController.GuestSeasonYear)).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBe(guests);
            viewBagGuests.DataValueField.ShouldBe<string>("TeamName");
            viewBagGuests.DataTextField.ShouldBe<string>("TeamName");

            GamePredictorController.HostSeasonYear.ShouldBe(prediction.HostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);

            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBe(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.HostSeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(prediction.HostName,
                GamePredictorController.HostSeasonYear)).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBe(hosts);
            viewBagHosts.DataValueField.ShouldBe<string>("TeamName");
            viewBagHosts.DataTextField.ShouldBe<string>("TeamName");

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestFoundAndHostNotFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            int guestSeasonYear = 1922;
            int hostSeasonYear = 1923;

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guests = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(guestSeasonYear)).Returns(guests);
            TeamSeason? guest = new TeamSeason { TeamName = "Guest" };
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, guestSeasonYear))
                .Returns(guest);

            var hosts = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(hostSeasonYear)).Returns(hosts);
            TeamSeason? host = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, hostSeasonYear))
                .Returns(host);

            var testController = new GamePredictorController(seasonRepository, teamSeasonRepository);

            var prediction = new GamePrediction
            {
                GuestSeasonYear = guestSeasonYear,
                HostSeasonYear = hostSeasonYear
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GamePredictorController.GuestSeasonYear.ShouldBe(prediction.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBe(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.GuestSeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(prediction.GuestName,
                GamePredictorController.GuestSeasonYear)).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBe(guests);
            viewBagGuests.DataValueField.ShouldBe<string>("TeamName");
            viewBagGuests.DataTextField.ShouldBe<string>("TeamName");
            viewBagGuests.SelectedValue.ShouldBe(guest.TeamName);

            GamePredictorController.HostSeasonYear.ShouldBe(prediction.HostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);

            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBe(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.HostSeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(prediction.HostName,
                GamePredictorController.HostSeasonYear)).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBe(hosts);
            viewBagHosts.DataValueField.ShouldBe<string>("TeamName");
            viewBagHosts.DataTextField.ShouldBe<string>("TeamName");

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public async Task PredictGamePost_WhenGuestAndHostBothFound_ShouldPredictGameAndReturnFilledFormView()
        {
            // Arrange
            int guestSeasonYear = 1922;
            int hostSeasonYear = 1923;

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();

            var guests = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(guestSeasonYear)).Returns(guests);
            TeamSeason? guest = new TeamSeason
            {
                TeamName = "Guest"
            };
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, guestSeasonYear))
                .Returns(guest);

            var hosts = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(hostSeasonYear)).Returns(hosts);
            TeamSeason? host = new TeamSeason
            {
                TeamName = "Host"
            };
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(A<string>.Ignored, hostSeasonYear))
                .Returns(host);

            var testController = new GamePredictorController(seasonRepository, teamSeasonRepository);

            var prediction = new GamePrediction
            {
                GuestSeasonYear = guestSeasonYear,
                HostSeasonYear = hostSeasonYear
            };

            // Act
            var result = await testController.PredictGame(prediction);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);

            GamePredictorController.GuestSeasonYear.ShouldBe(prediction.GuestSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.GuestSeasons);
            var viewBagGuestSeasons = (SelectList)testController.ViewBag.GuestSeasons;
            viewBagGuestSeasons.Items.ShouldBe(orderedSeasons);
            viewBagGuestSeasons.DataValueField.ShouldBe("Year");
            viewBagGuestSeasons.DataTextField.ShouldBe("Year");
            viewBagGuestSeasons.SelectedValue.ShouldBe(GamePredictorController.GuestSeasonYear);

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.GuestSeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(prediction.GuestName,
                GamePredictorController.GuestSeasonYear)).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Guests);
            var viewBagGuests = ((SelectList)testController.ViewBag.Guests);
            viewBagGuests.Items.ShouldBe(guests);
            viewBagGuests.DataValueField.ShouldBe<string>("TeamName");
            viewBagGuests.DataTextField.ShouldBe<string>("TeamName");
            viewBagGuests.SelectedValue.ShouldBe(guest.TeamName);

            GamePredictorController.HostSeasonYear.ShouldBe(prediction.HostSeasonYear);

            Assert.IsType<SelectList>(testController.ViewBag.HostSeasons);

            var viewBagHostSeasons = (SelectList)testController.ViewBag.HostSeasons;
            viewBagHostSeasons.Items.ShouldBe(orderedSeasons);
            viewBagHostSeasons.DataValueField.ShouldBe("Year");
            viewBagHostSeasons.DataTextField.ShouldBe("Year");
            viewBagHostSeasons.SelectedValue.ShouldBe(GamePredictorController.HostSeasonYear);

            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(GamePredictorController.HostSeasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonByTeamAndSeasonAsync(prediction.HostName,
                GamePredictorController.HostSeasonYear)).MustHaveHappenedOnceExactly();

            Assert.IsType<SelectList>(testController.ViewBag.Hosts);
            var viewBagHosts = ((SelectList)testController.ViewBag.Hosts);
            viewBagHosts.Items.ShouldBe(hosts);
            viewBagHosts.DataValueField.ShouldBe<string>("TeamName");
            viewBagHosts.DataTextField.ShouldBe<string>("TeamName");
            viewBagHosts.SelectedValue.ShouldBe(host.TeamName);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(prediction);
        }

        [Fact]
        public void ApplyFilter_WhenGuestSeasonYearAndHostSeasonYearAreBothNull_ShouldNotApplyFilterAndShouldRedirectToGamePredictorView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 0;
            GamePredictorController.HostSeasonYear = 0;
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testController = new GamePredictorController(seasonRepository, teamSeasonRepository);

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

        [Fact]
        public void ApplyFilter_WhenGuestSeasonYearIsNullAndHostSeasonYearIsNotNull_ShouldNotApplyFilterAndShouldRedirectToGamePredictorView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 0;
            GamePredictorController.HostSeasonYear = 0;
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testController = new GamePredictorController(seasonRepository, teamSeasonRepository);

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
        public void ApplyFilter_WhenGuestSeasonYearAndHostSeasonYearAreNotNull_ShouldNotApplyFilterAndShouldRedirectToGamePredictorView()
        {
            // Arrange
            GamePredictorController.GuestSeasonYear = 0;
            GamePredictorController.HostSeasonYear = 0;
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var testController = new GamePredictorController(seasonRepository, teamSeasonRepository);

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
    }
}
