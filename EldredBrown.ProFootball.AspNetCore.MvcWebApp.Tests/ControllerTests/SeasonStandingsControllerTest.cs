using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class SeasonStandingsControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnIndexView()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<SeasonTeamStanding>();
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandingsAsync(
                SeasonStandingsController.SelectedSeasonYear)).Returns(seasonStandings);

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();

            var orderedSeasons = seasons.OrderByDescending(s => s.Year);
            seasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            seasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons);
            seasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            seasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            seasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(SeasonStandingsController.SelectedSeasonYear);
            seasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(SeasonStandingsController.SelectedSeasonYear);

            A.CallTo(() => seasonStandingsRepository.GetSeasonStandingsAsync(
                SeasonStandingsController.SelectedSeasonYear)).MustHaveHappenedOnceExactly();
            seasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(seasonStandingsIndexViewModel);
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository);

            int? seasonYear = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearArgIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndexView()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository);

            int? seasonYear = 1920;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            SeasonStandingsController.SelectedSeasonYear.ShouldBe(seasonYear.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetGroupByDivision_WhenGroupByDivisionIsNull_ShouldNotSetGroupByDivisionAndShouldRedirectToIndexView()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository);

            bool? groupByDivision = null;

            // Act
            var result = testController.SetGroupByDivision(groupByDivision);

            // Assert
            SeasonStandingsController.GroupByDivision.ShouldBeFalse();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetGroupByDivision_WhenGroupByDivisionIsNotNull_ShouldSetGroupByDivisionAndRedirectToIndexView()
        {
            // Arrange
            var seasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository);

            bool? groupByDivision = true;

            // Act
            var result = testController.SetGroupByDivision(groupByDivision);

            // Assert
            SeasonStandingsController.GroupByDivision.ShouldBeTrue();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
