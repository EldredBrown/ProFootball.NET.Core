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
        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNotNullAndSelectedLeagueNameIsNotNullAndSelectedRankingTypeIsOffensive_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            // Set up leagues.
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new() { Id = 1, ShortName = "APFA", LongName = "American Professional Football Association", FirstSeasonId = 1920 },
                new() { Id = 2, ShortName = "NFL", LongName = "National Football League", FirstSeasonId = 1922 },
                new() { Id = 3, ShortName = "AFL", LongName = "American Football League", FirstSeasonId = 1960 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var seasonRankings = new List<RankingsOffensiveTeamSeason>
            {
                new() { TeamName = "Team A" },
                new() { TeamName = "Team B" },
                new() { TeamName = "Team C" },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetOffensiveRankingsForSeasonAsync(An<int>.Ignored))
                .Returns(seasonRankings);

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            int? selectedSeasonYear = 1920;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            string selectedLeagueName = "APFA";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            SeasonRankingType? selectedRankingType = SeasonRankingType.Offensive;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeLeagueRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify SelectSeason().
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Id).ToList();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBeEquivalentTo(orderedSeasons);

            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify SelectLeague().
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();

            var orderedLeagues = leagues.OrderBy(l => l.Id).ToList();

            var leaguesFromSession = testController.HttpContext.Session.GetObject<IEnumerable<League>>("Leagues");
            leaguesFromSession.ShouldBeEquivalentTo(orderedLeagues);

            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(orderedLeagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeagueName);
            fakeSeasonRankingsIndexViewModel.SelectedLeague.ShouldBe(selectedLeagueName);

            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(selectedLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify SelectRankingType().
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType.Value);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType.Value);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => fakeSeasonRankingsRepository.GetOffensiveRankingsForSeasonAsync(selectedSeasonYear.Value))
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
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            // Set up leagues.
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new() { Id = 1, ShortName = "APFA", LongName = "American Professional Football Association", FirstSeasonId = 1920 },
                new() { Id = 2, ShortName = "NFL", LongName = "National Football League", FirstSeasonId = 1922 },
                new() { Id = 3, ShortName = "AFL", LongName = "American Football League", FirstSeasonId = 1960 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var seasonRankings = new List<RankingsDefensiveTeamSeason>
            {
                new() { TeamName = "Team A" },
                new() { TeamName = "Team B" },
                new() { TeamName = "Team C" },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetDefensiveRankingsForSeasonAsync(An<int>.Ignored))
                .Returns(seasonRankings);

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            int? selectedSeasonYear = 1920;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            string selectedLeagueName = "APFA";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            SeasonRankingType? selectedRankingType = SeasonRankingType.Defensive;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeLeagueRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify SelectSeason().
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Id).ToList();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBeEquivalentTo(orderedSeasons);

            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify SelectLeague().
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();

            var orderedLeagues = leagues.OrderBy(l => l.Id).ToList();

            var leaguesFromSession = testController.HttpContext.Session.GetObject<IEnumerable<League>>("Leagues");
            leaguesFromSession.ShouldBeEquivalentTo(orderedLeagues);

            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(orderedLeagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeagueName);
            fakeSeasonRankingsIndexViewModel.SelectedLeague.ShouldBe(selectedLeagueName);

            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(selectedLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify SelectRankingType().
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType.Value);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType.Value);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => fakeSeasonRankingsRepository.GetDefensiveRankingsForSeasonAsync(selectedSeasonYear.Value))
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
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            // Set up leagues.
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new() { Id = 1, ShortName = "APFA", LongName = "American Professional Football Association", FirstSeasonId = 1920 },
                new() { Id = 2, ShortName = "NFL", LongName = "National Football League", FirstSeasonId = 1922 },
                new() { Id = 3, ShortName = "AFL", LongName = "American Football League", FirstSeasonId = 1960 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            var seasonRankings = new List<RankingsTotalTeamSeason>
            {
                new() { TeamName = "Team A" },
                new() { TeamName = "Team B" },
                new() { TeamName = "Team C" },
            };
            A.CallTo(() => fakeSeasonRankingsRepository.GetTotalRankingsForSeasonAsync(An<int>.Ignored))
                .Returns(seasonRankings);

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            int? selectedSeasonYear = 1920;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            string selectedLeagueName = "APFA";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            SeasonRankingType? selectedRankingType = SeasonRankingType.Total;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeLeagueRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify SelectSeason().
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Id).ToList();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBeEquivalentTo(orderedSeasons);

            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify SelectLeague().
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();

            var orderedLeagues = leagues.OrderBy(l => l.Id).ToList();

            var leaguesFromSession = testController.HttpContext.Session.GetObject<IEnumerable<League>>("Leagues");
            leaguesFromSession.ShouldBeEquivalentTo(orderedLeagues);

            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(orderedLeagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeagueName);
            fakeSeasonRankingsIndexViewModel.SelectedLeague.ShouldBe(selectedLeagueName);

            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(selectedLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify SelectRankingType().
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType.Value);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType.Value);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => fakeSeasonRankingsRepository.GetTotalRankingsForSeasonAsync(selectedSeasonYear.Value))
                .MustHaveHappenedOnceExactly();
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(seasonRankings);

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
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            // Set up leagues.
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new() { Id = 1, ShortName = "APFA", LongName = "American Professional Football Association", FirstSeasonId = 1920 },
                new() { Id = 2, ShortName = "NFL", LongName = "National Football League", FirstSeasonId = 1922 },
                new() { Id = 3, ShortName = "AFL", LongName = "American Football League", FirstSeasonId = 1960 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            int? selectedSeasonYear = 1920;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            string selectedLeagueName = "APFA";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            SeasonRankingType? selectedRankingType = SeasonRankingType.None;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeLeagueRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify SelectSeason().
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Id).ToList();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBeEquivalentTo(orderedSeasons);

            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);

            // Verify SelectLeague().
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();

            var orderedLeagues = leagues.OrderBy(l => l.Id).ToList();

            var leaguesFromSession = testController.HttpContext.Session.GetObject<IEnumerable<League>>("Leagues");
            leaguesFromSession.ShouldBeEquivalentTo(orderedLeagues);

            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(orderedLeagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeagueName);
            fakeSeasonRankingsIndexViewModel.SelectedLeague.ShouldBe(selectedLeagueName);

            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(selectedLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify SelectRankingType().
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType.Value);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType.Value);

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(new List<IRankingsTeamSeason>());

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNull_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            // Set up leagues.
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new() { Id = 1, ShortName = "APFA", LongName = "American Professional Football Association", FirstSeasonId = 1920 },
                new() { Id = 2, ShortName = "NFL", LongName = "National Football League", FirstSeasonId = 1922 },
                new() { Id = 3, ShortName = "AFL", LongName = "American Football League", FirstSeasonId = 1960 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            int? selectedSeasonYear = null;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            string selectedLeagueName = "APFA";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            SeasonRankingType? selectedRankingType = SeasonRankingType.None;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeLeagueRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify SelectSeason().
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Id).ToList();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBeEquivalentTo(orderedSeasons);

            var defaultSeasonYear = 1922;

            var selectedSeasonYearFromSession = testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            selectedSeasonYearFromSession.ShouldBe(defaultSeasonYear);

            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);

            // Verify SelectLeague().
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();

            var orderedLeagues = leagues.OrderBy(l => l.Id).ToList();

            var leaguesFromSession = testController.HttpContext.Session.GetObject<IEnumerable<League>>("Leagues");
            leaguesFromSession.ShouldBeEquivalentTo(orderedLeagues);

            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(orderedLeagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeagueName);
            fakeSeasonRankingsIndexViewModel.SelectedLeague.ShouldBe(selectedLeagueName);

            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(selectedLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify SelectRankingType().
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType.Value);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType.Value);

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(new List<IRankingsTeamSeason>());

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null!)]
        public async Task Index_WhenSelectedSelectedLeagueNameIsNullOrEmpty_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView(
            string selectedLeagueName
        )
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            // Set up leagues.
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new() { Id = 1, ShortName = "APFA", LongName = "American Professional Football Association", FirstSeasonId = 1920 },
                new() { Id = 2, ShortName = "NFL", LongName = "National Football League", FirstSeasonId = 1922 },
                new() { Id = 3, ShortName = "AFL", LongName = "American Football League", FirstSeasonId = 1960 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            int? selectedSeasonYear = null;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            SeasonRankingType? selectedRankingType = SeasonRankingType.None;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeLeagueRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify SelectSeason().
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Id).ToList();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBeEquivalentTo(orderedSeasons);

            var defaultSeasonYear = 1922;

            var selectedSeasonYearFromSession = testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            selectedSeasonYearFromSession.ShouldBe(defaultSeasonYear);

            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);

            // Verify SelectLeague().
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();

            var orderedLeagues = leagues.OrderBy(l => l.Id).ToList();

            var defaultLeagueName = "NFL";

            var selectedLeagueFromSession = testController.HttpContext.Session.GetObject<string>("SelectedLeagueName");
            selectedLeagueFromSession.ShouldBe(defaultLeagueName);

            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(orderedLeagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeagueName);
            fakeSeasonRankingsIndexViewModel.SelectedLeague.ShouldBe(defaultLeagueName);

            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(defaultLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify SelectRankingType().
            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = fakeSeasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            fakeSeasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            fakeSeasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType.Value);
            fakeSeasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType.Value);

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(new List<IRankingsTeamSeason>());

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSelectedRankingTypeIsNull_ShouldSetSelectedValuesToDefaultsAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            // Set up SeasonRankingsIndexViewModel.
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();

            // Set up seasons.
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            // Set up leagues.
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>
            {
                new() { Id = 1, ShortName = "APFA", LongName = "American Professional Football Association", FirstSeasonId = 1920 },
                new() { Id = 2, ShortName = "NFL", LongName = "National Football League", FirstSeasonId = 1922 },
                new() { Id = 3, ShortName = "AFL", LongName = "American Football League", FirstSeasonId = 1960 },
            };
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).Returns(leagues);

            // Set up season rankings.
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            // Set up HTTP session.
            var fakeSession = new MockHttpSession();

            int? selectedSeasonYear = null;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            string selectedLeagueName = string.Empty;
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            SeasonRankingType? selectedRankingType = null;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            // Set up test controller.
            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeLeagueRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            // Verify SelectSeason().
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Id).ToList();

            var seasonsFromSession = testController.HttpContext.Session.GetObject<IEnumerable<Season>>("Seasons");
            seasonsFromSession.ShouldBeEquivalentTo(orderedSeasons);

            var defaultSeasonYear = 1922;

            var selectedSeasonYearFromSession = testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            selectedSeasonYearFromSession.ShouldBe(defaultSeasonYear);

            fakeSeasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Seasons.Items.ShouldBe(orderedSeasons);
            fakeSeasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            fakeSeasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);

            // Verify SelectLeague().
            A.CallTo(() => fakeLeagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();

            var orderedLeagues = leagues.OrderBy(l => l.Id).ToList();

            var defaultLeagueName = "NFL";

            var selectedLeagueFromSession = testController.HttpContext.Session.GetObject<string>("SelectedLeagueName");
            selectedLeagueFromSession.ShouldBe(defaultLeagueName);

            fakeSeasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            fakeSeasonRankingsIndexViewModel.Leagues.Items.ShouldBe(orderedLeagues);
            fakeSeasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            fakeSeasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeagueName);
            fakeSeasonRankingsIndexViewModel.SelectedLeague.ShouldBe(defaultLeagueName);

            A.CallTo(() => fakeLeagueRepository.GetLeagueByShortNameAsync(defaultLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify SelectRankingType().
            var defaultRankingType = SeasonRankingType.None;

            var selectedRankingTypeFromSession = testController.HttpContext.Session
                .GetObject<SeasonRankingType?>("SelectedRankingType");
            selectedRankingTypeFromSession.ShouldBe(defaultRankingType);

            fakeSeasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
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
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(new List<IRankingsTeamSeason>());

            // Verify GetSeasonRankingsAsync().
            fakeSeasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(new List<IRankingsTeamSeason>());

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonRankingsIndexViewModel);
        }

        [Fact]
        public void SetSelectedLeagueName_WhenLeagueNameArgIsNeitherNullNorEmpty_ShouldSetSelectedLeagueNameAndRedirectToIndexView()
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedLeagueNameToSession = "NFL";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueNameToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeLeagueRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            string leagueName = "APFA";

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
        [InlineData(null!)]
        public void SetSelectedLeagueName_WhenLeagueNameArgIsNullOrEmpty_ShouldReturnBadRequest(string leagueName)
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedLeagueNameToSession = "NFL";
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueNameToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeLeagueRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = testController.SetSelectedLeagueName(leagueName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYearToSession = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYearToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeLeagueRepository, seasonRankingsRepository)
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
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYearToSession = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYearToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeLeagueRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            int? seasonId = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonId);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(SeasonRankingType.Offensive)]
        [InlineData(SeasonRankingType.Defensive)]
        [InlineData(SeasonRankingType.Total)]
        [InlineData(SeasonRankingType.None)]
        [InlineData(null!)]
        public void SetSelectedRankingtype_ShouldSetSelectedRankingTypeAndRedirectToIndexView(SeasonRankingType? rankingType)
        {
            // Arrange
            var seasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var fakeLeagueRepository = A.Fake<ILeagueRepository>();
            var seasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedRankingTypeToSession = SeasonRankingType.None;
            fakeSession.SetObject("SelectedRankingType", selectedRankingTypeToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonRankingsController(seasonRankingsIndexViewModel, seasonRepository,
                fakeLeagueRepository, seasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = testController.SetSelectedRankingType(rankingType);

            // Assert
            var selectedRankingTypeFromSession = testController.HttpContext.Session
                .GetObject<SeasonRankingType?>("SelectedRankingType");
            selectedRankingTypeFromSession.ShouldBe(rankingType);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
