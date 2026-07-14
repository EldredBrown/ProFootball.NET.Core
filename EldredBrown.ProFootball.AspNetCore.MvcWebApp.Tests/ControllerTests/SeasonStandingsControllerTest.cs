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
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class SeasonStandingsControllerTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Index_WhenSelectedSeasonYearIsNullAndSelectedLeagueNameIsNullOrEmpty_ShouldSetSelectedSeasonYearAndReturnSeasonStandingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            var fakeSeasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            var defaultSeason = seasons.First(s => s.Year == 1922);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = 1920,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1920),
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
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
            Association defaultLeague = leagues.First();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(defaultLeague);

            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<StandingsTeamSeason>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonStandings);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", null);
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(fakeSeasonStandingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeason.Year);
            fakeSeasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonStandingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonStandingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonStandingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeague.ShortName);
            fakeSeasonStandingsIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeague.ShortName);

            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(defaultSeason.Year, An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonStandingsIndexViewModel);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldSetSelectedSeasonYearAndReturnSeasonStandingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            var fakeSeasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = 1920,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1920),
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)
                .ToList();
            Association defaultLeague = leagues.First();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(defaultLeague);

            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<StandingsTeamSeason>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonStandings);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject<string>("SelectedLeagueName", selectedLeagueName);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(fakeSeasonStandingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonStandingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonStandingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonStandingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeague.ShortName);
            fakeSeasonStandingsIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeague.ShortName);

            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(selectedSeason.Year, An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonStandingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonLeagueNameIsNeitherNullNorEmpty_ShouldSetSelectedSeasonYearAndReturnSeasonStandingsIndexView()
        {
            // Arrange
            var fakeSeasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            var selectedSeason = seasons.First(s => s.Year == 1920);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ParentId = null,
                    LongName = "American Professional Football Association",
                    ShortName = "APFA",
                    FirstSeasonYear = 1920,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1920),
                    LastSeasonYear = 1922,
                    LastSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 2,
                    ParentId = null,
                    LongName = "National Football League",
                    ShortName = "NFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 3,
                    ParentId = null,
                    LongName = "American Football League",
                    ShortName = "AFL",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 4,
                    ParentId = null,
                    LongName = "American Football Conference",
                    ShortName = "AFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
                new()
                {
                    Id = 5,
                    ParentId = null,
                    LongName = "National Football Conference",
                    ShortName = "NFC",
                    FirstSeasonYear = 1922,
                    FirstSeasonYearNavigation = seasons.First(s => s.Year == 1922)
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);
            var leagues = associations
                .Where(a => a.ParentId is null)
                .Where(
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)
                .ToList();
            Association selectedLeague = leagues.First();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(selectedLeague);

            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<StandingsTeamSeason>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonStandings);

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject<string>("SelectedLeagueName", selectedLeague.ShortName);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(fakeSeasonStandingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonStandingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonStandingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonStandingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeSeasonStandingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);

            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(selectedSeason.Year, An<int>.Ignored))
                .MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonStandingsIndexViewModel);
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYearToSession = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYearToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            int? seasonId = 1920;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonId);

            // Assert
            var selectedSeasonYearFromSession = testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            selectedSeasonYearFromSession.ShouldBe(seasonId.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonStandingsRepository);

            int? seasonId = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonId);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedLeagueName_WhenLeagueNameArgIsNotNull_ShouldSetSelectedLeawgueNameAndRedirectToIndexView()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedLeagueNameToSession = "NFL";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueNameToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            string leagueName = "NFL";

            // Act
            var result = testController.SetSelectedLeagueName(leagueName);

            // Assert
            var selectedLeagueNameFromSession = testController.HttpContext.Session.GetObject<string>("SelectedLeagueName");
            selectedLeagueNameFromSession.ShouldBe(leagueName);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetSelectedLeagueName_WhenLeagueNameArgIsNullOrEmpty_ShouldReturnBadRequest(string? leagueName)
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonStandingsRepository);

            // Act
            var result = testController.SetSelectedLeagueName(leagueName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(false)]
        [InlineData(true)]
        public void SetGroupByDivision_ShouldSetGroupByDivisionAndRedirectToIndexView(
            bool? groupByDivision
        )
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<bool?>("GroupByDivision", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = testController.SetGroupByDivision(groupByDivision);

            // Assert
            var groupByDivisionFromSession = testController.HttpContext.Session.GetObject<bool?>("GroupByDivision");
            groupByDivisionFromSession.ShouldBe(groupByDivision);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
