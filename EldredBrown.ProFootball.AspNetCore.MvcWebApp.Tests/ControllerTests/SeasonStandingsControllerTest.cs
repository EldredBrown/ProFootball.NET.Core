using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

using FakeItEasy;
using Moq;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class SeasonStandingsControllerTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Index_WhenSelectedSeasonYearIsNullAndSelectedLeagueNameIsNullOrEmpty_ShouldSetSelectedSeasonYearAndReturnSeasonStandingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            var defaultSeasonYear = 1922;
            var defaultLeagueName = "NFL";

            (
                SeasonStandingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                List<StandingsTeamSeason> seasonStandings
            ) = SetUp(leagueName: selectedLeagueName);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            testController._seasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonStandingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonStandingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonStandingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonStandingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeagueName);
            testController._seasonStandingsIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeagueName);

            A.CallTo(() => testController._seasonStandingsRepository.GetSeasonStandingsAsync(selectedSeason.Year, An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonStandingsIndexViewModel);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldSetSelectedSeasonYearAndReturnSeasonStandingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            var selectedSeasonYear = 1920;

            (
                SeasonStandingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                List<StandingsTeamSeason> seasonStandings
            ) = SetUp(seasonYear: selectedSeasonYear, leagueName: selectedLeagueName);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonStandingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonStandingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonStandingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonStandingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._seasonStandingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            A.CallTo(() => testController._seasonStandingsRepository.GetSeasonStandingsAsync(selectedSeason.Year, An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonStandingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonLeagueNameIsNeitherNullNorEmpty_ShouldSetSelectedSeasonYearAndReturnSeasonStandingsIndexView()
        {
            // Arrange
            int? selectedSeasonYear = 1920;
            string? selectedLeagueName = "APFA";

            (
                SeasonStandingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                List<StandingsTeamSeason> seasonStandings
            ) = SetUp(seasonYear: selectedSeasonYear, leagueName: selectedLeagueName);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonStandingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonStandingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonStandingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonStandingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._seasonStandingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            A.CallTo(() => testController._seasonStandingsRepository.GetSeasonStandingsAsync(selectedSeason.Year, An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            testController._seasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonStandingsIndexViewModel);
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            int? seasonYearIn = 1922;
            string? selectedLeagueName = "APFA";

            (SeasonStandingsController testController, _, _, _, _, _) =
                SetUp(seasonYear: seasonYearIn, leagueName: selectedLeagueName);

            // Act
            int? selectedSeasonYear = 1920;
            var result = testController.SetSelectedSeasonYear(selectedSeasonYear);

            // Assert
            var seasonYearOut = testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            seasonYearOut.ShouldBe(selectedSeasonYear.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            int? seasonYearIn = 1922;
            string? selectedLeagueName = "APFA";

            (SeasonStandingsController testController, _, _, _, _, _) =
                SetUp(seasonYear: seasonYearIn, leagueName: selectedLeagueName);

            // Act
            int? seasonYear = null;
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedLeagueName_WhenLeagueNameArgIsNotNull_ShouldSetSelectedLeawgueNameAndRedirectToIndexView()
        {
            // Arrange
            string? leagueNameIn = "APFA";
            (SeasonStandingsController testController, _, _, _, _, _) = SetUp(leagueName: leagueNameIn);

            // Act
            string selectedLeagueName = "NFL";
            var result = testController.SetSelectedLeagueName(selectedLeagueName);

            // Assert
            var leagueNameOut = testController.HttpContext.Session.GetObject<string>("SelectedLeagueName");
            leagueNameOut.ShouldBe(selectedLeagueName);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void SetSelectedLeagueName_WhenLeagueNameArgIsNullOrEmpty_ShouldReturnBadRequest(string? selectedLeagueName)
        {
            // Arrange
            string? leagueNameIn = "APFA";
            (SeasonStandingsController testController, _, _, _, _, _) = SetUp(leagueName: leagueNameIn);

            // Act
            var result = testController.SetSelectedLeagueName(selectedLeagueName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void SetGroupByDivision_ShouldSetGroupByDivisionAndRedirectToIndexView(
            bool? groupByDivisionIn
        )
        {
            // Arrange
            (SeasonStandingsController testController, _, _, _, _, _) = SetUp();

            // Act
            var result = testController.SetGroupByDivision(groupByDivisionIn);

            // Assert
            var groupByDivisionOut = testController.HttpContext.Session.GetObject<bool?>("GroupByDivision");
            groupByDivisionOut.ShouldBe(groupByDivisionIn);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static (SeasonStandingsController, List<Season>, Season, List<Association>, Association, List<StandingsTeamSeason>)
            SetUp(int? seasonYear = null, string? leagueName = null)
        {
            var fakeSeasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            (ISeasonRepository fakeSeasonRepository, int? selectedSeasonYear, List<Season> seasons, Season defaultSeason) =
                SetUpFakeSeasonRepository(seasonYear);
            (IAssociationRepository fakeAssociationRepository, List<Association> leagues, Association selectedLeague) =
                SetUpFakeAssociationRepository(seasons, defaultSeason);
            (ISeasonStandingsRepository fakeSeasonStandingsRepository, List<StandingsTeamSeason> seasonStandings) =
                SetUpFakeSeasonStandingsRepository();
            Mock<HttpContext> httpContext = SetUpHttpContext(leagueName, selectedSeasonYear, selectedLeague);

            var testController = new SeasonStandingsController(
                fakeSeasonStandingsIndexViewModel,
                fakeSeasonRepository, fakeAssociationRepository, fakeSeasonStandingsRepository
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            return (testController, seasons, defaultSeason, leagues, selectedLeague, seasonStandings);
        }

        private static (ISeasonRepository, int?, List<Season>, Season) SetUpFakeSeasonRepository(int? seasonYear)
        {
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var selectedSeasonYear = seasonYear is null ? 1922 : seasonYear;
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            var selectedSeason = seasons.First(s => s.Year == selectedSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            return (fakeSeasonRepository, selectedSeasonYear, seasons, selectedSeason);
        }

        private static (IAssociationRepository, List<Association>, Association) 
            SetUpFakeAssociationRepository(List<Season> seasons, Season defaultSeason)
        {
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    ParentId = null,
                    FirstSeasonYear = 1920,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1920),
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    ParentId = null,
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    ParentId = 2,
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
                    ParentId = 2,
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= defaultSeason.Year
                    && (l.LastSeasonYearNavigation is null || defaultSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)
                .ToList();
            var selectedLeague = leagues.First();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(selectedLeague);

            return (fakeAssociationRepository, leagues, selectedLeague);
        }

        private static (ISeasonStandingsRepository, List<StandingsTeamSeason>) SetUpFakeSeasonStandingsRepository()
        {
            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<StandingsTeamSeason>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonStandings);

            return (fakeSeasonStandingsRepository, seasonStandings);
        }

        private static Mock<HttpContext> SetUpHttpContext(string? leagueName, int? selectedSeasonYear, Association selectedLeague)
        {
            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var selectedLeagueName = leagueName.IsNullOrEmpty() ? selectedLeague.ShortName : leagueName;
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            fakeSession.SetObject<bool?>("GroupByDivision", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);
            return httpContext;
        }
    }
}
