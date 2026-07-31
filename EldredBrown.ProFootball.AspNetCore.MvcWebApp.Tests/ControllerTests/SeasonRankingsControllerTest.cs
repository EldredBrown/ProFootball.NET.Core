using System;
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
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonRankings;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class SeasonRankingsControllerTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task Index_WhenSelectedSeasonYearIsNullAndSelectedLeagueNameIsNullOrEmptyAndSelectedRankingTypeIsNull_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            var defaultSeasonYear = 1922;
            var defaultLeagueName = "NFL";
            var defaultRankingType = SeasonRankingType.None;

            (
                SeasonRankingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                SeasonRankingType selectedRankingType
            ) = SetUp(selectedLeagueName);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            testController._seasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeagueName);
            testController._seasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeagueName);
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(defaultLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            testController._seasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(defaultRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = testController._seasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            testController._seasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            testController._seasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            testController._seasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(defaultRankingType);
            testController._seasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(defaultRankingType);

            // Verify GetSeasonRankingsAsync().
            testController._seasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonRankingsIndexViewModel);
        }

        [Theory]
        [InlineData(null)]
        //[InlineData("")]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView(
            string? selectedLeagueName
        )
        {
            // Arrange
            int? selectedSeasonYear = 1922;
            var defaultLeagueName = "NFL";
            var defaultRankingType = SeasonRankingType.None;

            (
                SeasonRankingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                SeasonRankingType selectedRankingType
            ) = SetUp(selectedLeagueName, seasonYear: selectedSeasonYear);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(defaultLeagueName);
            testController._seasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(defaultLeagueName);
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(defaultLeagueName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            testController._seasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(defaultRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = testController._seasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            testController._seasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            testController._seasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            testController._seasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(defaultRankingType);
            testController._seasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(defaultRankingType);

            // Verify GetSeasonRankingsAsync().
            testController._seasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedLeagueNameIsNeitherNullNorEmpty_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            int? selectedSeasonYear = 1920;
            var selectedLeagueName = "APFA";
            var defaultRankingType = SeasonRankingType.None;

            (
                SeasonRankingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                SeasonRankingType selectedRankingType
            ) = SetUp(selectedLeagueName, seasonYear: selectedSeasonYear);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._seasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            testController._seasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(defaultRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = testController._seasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            testController._seasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            testController._seasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            testController._seasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(defaultRankingType);
            testController._seasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(defaultRankingType);

            // Verify GetSeasonRankingsAsync().
            testController._seasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsNone_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            int? selectedSeasonYear = 1920;
            string selectedLeagueName = "APFA";
            var selectedRankingType = SeasonRankingType.None;

            (
                SeasonRankingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                _
            ) = SetUp(selectedLeagueName, seasonYear: selectedSeasonYear, rankingType: selectedRankingType);

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._seasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            testController._seasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = testController._seasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            testController._seasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            testController._seasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            testController._seasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            testController._seasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            testController._seasonRankingsIndexViewModel.SeasonRankings.ShouldBe([]);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsOffensive_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            int? selectedSeasonYear = 1920;
            string selectedLeagueName = "APFA";
            var selectedRankingType = SeasonRankingType.Offensive;
            var offensiveRankings = new List<RankingsOffensiveTeamSeason>
            {
                new() { TeamName = "Team A" },
                new() { TeamName = "Team B" },
                new() { TeamName = "Team C" },
            };

            (
                SeasonRankingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                _
            ) = SetUp(
                selectedLeagueName, seasonYear: selectedSeasonYear, rankingType: selectedRankingType,
                offensiveRankings: offensiveRankings
            );

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._seasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            testController._seasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = testController._seasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            testController._seasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            testController._seasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            testController._seasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            testController._seasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => testController._seasonRankingsRepository.GetOffensiveRankingsAsync(selectedSeason.Year, selectedLeague.Id))
                .MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(offensiveRankings);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsDefensive_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            int? selectedSeasonYear = 1920;
            string selectedLeagueName = "APFA";
            var selectedRankingType = SeasonRankingType.Defensive;
            var defensiveRankings = new List<RankingsDefensiveTeamSeason>
            {
                new() { TeamName = "Team A" },
                new() { TeamName = "Team B" },
                new() { TeamName = "Team C" },
            };

            (
                SeasonRankingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                _
            ) = SetUp(
                selectedLeagueName, seasonYear: selectedSeasonYear, rankingType: selectedRankingType,
                defensiveRankings: defensiveRankings
            );

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._seasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            testController._seasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = testController._seasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            testController._seasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            testController._seasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            testController._seasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            testController._seasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => testController._seasonRankingsRepository.GetDefensiveRankingsAsync(selectedSeason.Year, selectedLeague.Id))
                .MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(defensiveRankings);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonRankingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedRankingTypeIsTotal_ShouldSetSelectedValuesAndReturnSeasonRankingsIndexView()
        {
            // Arrange
            int? selectedSeasonYear = 1920;
            string selectedLeagueName = "APFA";
            var selectedRankingType = SeasonRankingType.Total;
            var totalRankings = new List<RankingsTotalTeamSeason>
            {
                new() { TeamName = "Team A" },
                new() { TeamName = "Team B" },
                new() { TeamName = "Team C" },
            };

            (
                SeasonRankingsController testController,
                List<Season> seasons, Season selectedSeason,
                List<Association> leagues, Association selectedLeague,
                _
            ) = SetUp(
                selectedLeagueName, seasonYear: selectedSeasonYear, rankingType: selectedRankingType,
                totalRankings: totalRankings
            );

            // Act
            var result = await testController.Index();

            // Assert
            // Verify seasons.
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            testController._seasonRankingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._seasonRankingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeason.Year);
            testController._seasonRankingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeason.Year);

            // Verify leagues.
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.Leagues.ShouldBeOfType<SelectList>();
            testController._seasonRankingsIndexViewModel.Leagues.Items.ShouldBe(leagues);
            testController._seasonRankingsIndexViewModel.Leagues.DataValueField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.DataTextField.ShouldBe<string>("ShortName");
            testController._seasonRankingsIndexViewModel.Leagues.SelectedValue.ShouldBe(selectedLeague.ShortName);
            testController._seasonRankingsIndexViewModel.SelectedLeagueName.ShouldBe(selectedLeague.ShortName);
            A.CallTo(() => testController._associationRepository.GetAssociationByShortNameAsync(selectedLeague.ShortName))
                .MustHaveHappenedOnceExactly();

            // Verify ranking types.
            testController._seasonRankingsIndexViewModel.RankingTypes.ShouldBeOfType<SelectList>();
            testController.HttpContext.Session.GetObject<SeasonRankingType?>("SelectedRankingType")
                .ShouldBe(selectedRankingType);
            var expectedRankingTypesSelectListItems = Enum.GetValues<SeasonRankingType>()
                .Select(e => new { Value = (int)e, Text = e.ToString() });
            for (int i = 0; i < expectedRankingTypesSelectListItems.Count(); i++)
            {
                var actualItem = testController._seasonRankingsIndexViewModel.RankingTypes.ElementAt(i).Value;
                var expectedItem = expectedRankingTypesSelectListItems.ElementAt(i);
            }
            testController._seasonRankingsIndexViewModel.RankingTypes.DataValueField.ShouldBe<string>("Value");
            testController._seasonRankingsIndexViewModel.RankingTypes.DataTextField.ShouldBe<string>("Text");
            testController._seasonRankingsIndexViewModel.RankingTypes.SelectedValue.ShouldBe(selectedRankingType);
            testController._seasonRankingsIndexViewModel.SelectedRankingType.ShouldBe(selectedRankingType);

            // Verify GetSeasonRankingsAsync().
            A.CallTo(() => testController._seasonRankingsRepository.GetTotalRankingsAsync(selectedSeason.Year, selectedLeague.Id))
                .MustHaveHappenedOnceExactly();
            testController._seasonRankingsIndexViewModel.SeasonRankings.ShouldBeEquivalentTo(totalRankings);

            // Verify result.
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonRankingsIndexViewModel);
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            var seasonYearIn = 1922;
            (SeasonRankingsController testController, _, _, _, _, _) = SetUp(seasonYear: seasonYearIn);

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
            var seasonYearIn = 1922;
            (SeasonRankingsController testController, _, _, _, _, _) = SetUp(seasonYear: seasonYearIn);

            // Act
            int? selectedSeasonYear = null;
            var result = testController.SetSelectedSeasonYear(selectedSeasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedLeagueName_WhenLeagueNameArgIsNeitherNullNorEmpty_ShouldSetSelectedLeagueNameAndRedirectToIndexView()
        {
            // Arrange
            string? leagueNameIn = "APFA";
            (SeasonRankingsController testController, _, _, _, _, _) = SetUp(leagueName: leagueNameIn);

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
            (SeasonRankingsController testController, _, _, _, _, _) = SetUp(leagueName: leagueNameIn);

            // Act
            var result = testController.SetSelectedLeagueName(selectedLeagueName);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(SeasonRankingType.Offensive)]
        [InlineData(SeasonRankingType.Defensive)]
        [InlineData(SeasonRankingType.Total)]
        [InlineData(SeasonRankingType.None)]
        public void SetSelectedRankingType_ShouldSetSelectedRankingTypeAndRedirectToIndexView(SeasonRankingType selectedRankingType)
        {
            // Arrange
            SeasonRankingType? rankingTypeIn = null;
            (SeasonRankingsController testController, _, _, _, _, _) = SetUp(rankingType: rankingTypeIn);

            // Act
            var result = testController.SetSelectedRankingType(selectedRankingType);

            // Assert
            var rankingTypeOut = testController.HttpContext.Session.GetObject<SeasonRankingType>("SelectedRankingType");
            rankingTypeOut.ShouldBe(selectedRankingType);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static (SeasonRankingsController, List<Season>, Season, List<Association>, Association, SeasonRankingType)
            SetUp(
                string? leagueName = null, int? seasonYear = null, SeasonRankingType? rankingType = null,
                List<RankingsOffensiveTeamSeason>? offensiveRankings = null,
                List<RankingsDefensiveTeamSeason>? defensiveRankings = null,
                List<RankingsTotalTeamSeason>? totalRankings = null
            )
        {
            var fakeSeasonRankingsIndexViewModel = A.Fake<ISeasonRankingsIndexViewModel>();
            (ISeasonRepository fakeSeasonRepository, List<Season> seasons, int? selectedSeasonYear, Season selectedSeason) =
                SetUpFakeSeasonRepository(seasonYear);
            (IAssociationRepository fakeAssociationRepository, List<Association> leagues, Association selectedLeague) =
                SetUpLeagues(seasons, selectedSeason);
            ISeasonRankingsRepository fakeSeasonRankingsRepository =
                SetUpFakeSeasonRankingsRepository(offensiveRankings, defensiveRankings, totalRankings);
            (SeasonRankingType? selectedRankingType, Mock<HttpContext> httpContext) =
                SetUpHttpContext(selectedSeasonYear, leagueName, rankingType, selectedLeague);

            var testController = new SeasonRankingsController(fakeSeasonRankingsIndexViewModel, fakeSeasonRepository,
                fakeAssociationRepository, fakeSeasonRankingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            return (testController, seasons, selectedSeason, leagues, selectedLeague, selectedRankingType.Value);
        }

        private static (ISeasonRepository, List<Season>, int?, Season) SetUpFakeSeasonRepository(int? seasonYear)
        {
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            seasons = [.. seasons.OrderByDescending(s => s.Year)];
            var selectedSeasonYear = seasonYear is null ? 1922 : seasonYear;
            var selectedSeason = seasons.First(s => s.Year == selectedSeasonYear);
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            return (fakeSeasonRepository, seasons, selectedSeasonYear, selectedSeason);
        }

        private static (IAssociationRepository, List<Association>, Association)
            SetUpLeagues(List<Season> seasons, Season selectedSeason)
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
                    l => l.FirstSeasonYearNavigation.Year <= selectedSeason.Year
                    && (l.LastSeasonYearNavigation is null || selectedSeason.Year <= l.LastSeasonYearNavigation.Year)
                )
                .OrderByDescending(a => a.ShortName)
                .ToList();
            var selectedLeague = leagues.First();
            A.CallTo(() => fakeAssociationRepository.GetAssociationByShortNameAsync(A<string>.Ignored))
                .Returns(selectedLeague);

            return (fakeAssociationRepository, leagues, selectedLeague);
        }

        private static ISeasonRankingsRepository SetUpFakeSeasonRankingsRepository(List<RankingsOffensiveTeamSeason>? offensiveRankings, List<RankingsDefensiveTeamSeason>? defensiveRankings, List<RankingsTotalTeamSeason>? totalRankings)
        {
            var fakeSeasonRankingsRepository = A.Fake<ISeasonRankingsRepository>();
            if (offensiveRankings is not null)
            {
                A.CallTo(() => fakeSeasonRankingsRepository.GetOffensiveRankingsAsync(An<int>.Ignored, An<int>.Ignored))
                    .Returns(offensiveRankings);
            }
            if (defensiveRankings is not null)
            {
                A.CallTo(() => fakeSeasonRankingsRepository.GetDefensiveRankingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(defensiveRankings);
            }
            if (totalRankings is not null)
            {
                A.CallTo(() => fakeSeasonRankingsRepository.GetTotalRankingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(totalRankings);
            }

            return fakeSeasonRankingsRepository;
        }

        private static (SeasonRankingType? selectedRankingType, Mock<HttpContext> httpContext)
            SetUpHttpContext(int? selectedSeasonYear, string? leagueName, SeasonRankingType? rankingType, Association selectedLeague)
        {
            var fakeSession = new MockHttpSession();

            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var selectedLeagueName = leagueName.IsNullOrEmpty() ? selectedLeague.ShortName : leagueName;
            fakeSession.SetObject("SelectedLeagueName", selectedLeagueName);

            var selectedRankingType = rankingType is null ? SeasonRankingType.None : rankingType;
            fakeSession.SetObject("SelectedRankingType", selectedRankingType);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            return (selectedRankingType, httpContext);
        }
    }
}
