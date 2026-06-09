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
        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldReturnIndexView()
        {
            // Arrange
            var fakeSeasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<StandingsTeamSeason>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored)).Returns(seasonStandings);

            var fakeSession = new MockHttpSession();
            int? selectedSeasonYear = 1920;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(fakeSeasonStandingsIndexViewModel, fakeSeasonRepository,
                fakeSeasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            fakeSeasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeSeasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(selectedSeasonYear.Value))
                .MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.SeasonStandings.ShouldBe(seasonStandings);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonStandingsIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNull_ShouldSetSelectedSeasonYearAndReturnSeasonStandingsIndexView()
        {
            // Arrange
            var fakeSeasonStandingsIndexViewModel = A.Fake<ISeasonStandingsIndexViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<StandingsTeamSeason>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored)).Returns(seasonStandings);

            var fakeSession = new MockHttpSession();
            int? selectedSeasonYear = null;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(fakeSeasonStandingsIndexViewModel, fakeSeasonRepository,
                fakeSeasonStandingsRepository)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonStandingsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeSeasonStandingsIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            fakeSeasonStandingsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeSeasonStandingsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeSeasonStandingsIndexViewModel.Seasons.SelectedValue.ShouldBe(1922);
            fakeSeasonStandingsIndexViewModel.SelectedSeasonYear.ShouldBe(1922);
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(1922))
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
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYearToSession = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYearToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository)
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

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository);

            int? seasonId = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonId);

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
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();

            var fakeSession = new MockHttpSession();
            fakeSession.SetObject<bool?>("GroupByDivision", null);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new SeasonStandingsController(seasonStandingsIndexViewModel, seasonRepository,
                seasonStandingsRepository)
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
