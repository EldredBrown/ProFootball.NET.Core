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
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonRankings;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class SeasonRankingsControllerTest
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Index_WhenSelectedSeasonYearIsNullAndSelectedLeagueNameIsNullOrEmptyAndSelectedRankingTypeIsNull_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
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

            // Set up associations.
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

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var defaultRankingType = SeasonRankingType.None;

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", null);
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);
            fakeSession.SetObject<SeasonRankingType?>("SelectedRankingType", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
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
            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeason.Year);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeague.ShortName);
            fakeSeasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeague.ShortName);
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(defaultLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(defaultRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(defaultRankingType);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(defaultRankingType);

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
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

            // Set up associations.
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

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var defaultRankingType = SeasonRankingType.None;

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);
            fakeSession.SetObject<SeasonRankingType?>("SelectedRankingType", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
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
            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeague.ShortName);
            fakeSeasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeague.ShortName);
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(defaultLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(defaultRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(defaultRankingType);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(defaultRankingType);

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedLeagueNameIsNeitherNullNorEmpty_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
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

            // Set up associations.
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

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var defaultRankingType = SeasonRankingType.None;

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            fakeSession.SetObject<SeasonRankingType?>("SelectedRankingType", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
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
            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeSeasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(defaultRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(defaultRankingType);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(defaultRankingType);

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsNone_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
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

            // Set up associations.
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

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var selectedRankingType = SeasonRankingType.None;

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            fakeSession.SetObject<SeasonRankingType?>("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
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
            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeSeasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsOffensive_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
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

            // Set up associations.
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

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var seasonRankings = new List<RankingsOffensiveTeamSeason>
                {
                    new() { TeamName = "Team A" },
                    new() { TeamName = "Team B" },
                    new() { TeamName = "Team C" },
                };
            A.CallTo(() => fakeSeasonRankingsRepository.GetOffensiveRankingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonRankings);
            var selectedRankingType = SeasonRankingType.Offensive;

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            fakeSession.SetObject<SeasonRankingType?>("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
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
            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeSeasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => fakeSeasonRankingsRepository.GetOffensiveRankingsAsync(selectedSeason.Year, selectedLeague.Id))
                .MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(seasonRankings);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsDefensive_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
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

            // Set up associations.
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

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var seasonRankings = new List<RankingsDefensiveTeamSeason>
                {
                    new() { TeamName = "Team A" },
                    new() { TeamName = "Team B" },
                    new() { TeamName = "Team C" },
                };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDefensiveRankingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonRankings);
            var selectedRankingType = SeasonRankingType.Defensive;

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            fakeSession.SetObject<SeasonRankingType?>("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
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
            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeSeasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => fakeSeasonRankingsRepository.GetDefensiveRankingsAsync(selectedSeason.Year, selectedLeague.Id))
                .MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(seasonRankings);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsTotal_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
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

            // Set up associations.
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

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var seasonRankings = new List<RankingsTotalTeamSeason>
                {
                    new() { TeamName = "Team A" },
                    new() { TeamName = "Team B" },
                    new() { TeamName = "Team C" },
                };
            A.CallTo(() => fakeSeasonRankingsRepository.GetTotalRankingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonRankings);
            var selectedRankingType = SeasonRankingType.Total;

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject<int?>("SelectedSeasonYear", selectedSeason.Year);
            fakeSession.SetObject("SelectedLeagueName", selectedLeague.ShortName);
            fakeSession.SetObject<SeasonRankingType?>("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
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
            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            fakeSeasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => fakeSeasonRankingsRepository.GetTotalRankingsAsync(selectedSeason.Year, selectedLeague.Id))
                .MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(seasonRankings);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYearToSession = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYearToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            int? seasonYear = 1920;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            var selectedSeasonYearFromSession = testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            selectedSeasonYearFromSession.ShouldBe(seasonYear.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYearToSession = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYearToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            int? seasonYear = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedLeagueName_WhenAssociationNameArgIsNeitherNullNorEmpty_ShouldSetSelectedLeagueNameAndRedirectToIndexView()
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedAssociationNameToSession = "NFL";
            fakeSession.SetObject("SelectedLeagueName", selectedAssociationNameToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            string associationName = "APFA";

            // Act
            var result = testController.SetSelectedLeagueName(associationName);

            // Assert
            testController.HttpContext.Session.GetObject<string>("SelectedLeagueName").ShouldBe(associationName);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SetSelectedLeagueName_WhenAssociationNameArgIsNullOrEmpty_ShouldReturnBadRequest(string? associationName)
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedAssociationNameToSession = "NFL";
            fakeSession.SetObject("SelectedLeagueName", selectedAssociationNameToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = testController.SetSelectedLeagueName(associationName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(SeasonRankingType.Offensive)]
        [InlineData(SeasonRankingType.Defensive)]
        [InlineData(SeasonRankingType.Total)]
        [InlineData(SeasonRankingType.None)]
        public void SetSelectedRankingType_ShouldSetSelectedRankingTypeAndRedirectToIndexView(SeasonRankingType? rankingType)
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedRankingTypeToSession = SeasonRankingType.None;
            fakeSession.SetObject("SelectedRankingType", selectedRankingTypeToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeAssociationRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = testController.SetSelectedRankingType(rankingType.Value);

            // Assert
            var selectedRankingTypeFromSession = testController.HttpContext.Session
                .GetObject<SeasonRankingType?>("SelectedRankingType");
            selectedRankingTypeFromSession.ShouldBe(rankingType);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
