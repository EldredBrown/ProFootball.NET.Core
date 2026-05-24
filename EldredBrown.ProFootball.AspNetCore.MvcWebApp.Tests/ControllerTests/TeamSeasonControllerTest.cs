using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;
using EldredBrown.ProFootball.Net.Data.Decorators;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamSeasonControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnTeamSeasonsIndexView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            TeamSeasonController.SelectedSeasonYear = selectedSeasonYear;

            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored)).Returns(teamSeasons);

            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeTeamSeasonIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeTeamSeasonIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            fakeTeamSeasonIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            fakeTeamSeasonIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            fakeTeamSeasonIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeTeamSeasonIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(selectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            fakeTeamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonIsFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            string teamName = "Team";
            int seasonYear = 1920;
            TeamSeason? teamSeason = new TeamSeason
            {
                TeamName = teamName,
                SeasonYear = seasonYear
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();

            var teamSeasonScheduleProfile = new List<TeamSeasonOpponentProfile>();
            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(A<string>.Ignored,
                An<int>.Ignored)).Returns(teamSeasonScheduleProfile);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                An<int>.Ignored)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                An<int>.Ignored)).Returns(teamSeasonScheduleAverages);

            var sharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService);

            int? id = 1;

            // Act
            var result = await testController.Details(id);

            // Assert
            fakeTeamSeasonDetailsViewModel.TeamSeason = new TeamSeasonDecorator(teamSeason);

            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeasonScheduleProfile.ShouldBe(teamSeasonScheduleProfile);

            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeasonScheduleTotals.ShouldBe(teamSeasonScheduleTotals);

            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName,
                seasonYear)).MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeasonScheduleAverages.ShouldBe(teamSeasonScheduleAverages);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamSeasonIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService);

            int? id = 1;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RunWeeklyUpdate_ShouldRunWeeklyUpdateAndRedirectToIndex()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService);

            // Act
            var result = await testController.RunWeeklyUpdate();

            // Assert
            A.CallTo(() => fakeWeeklyUpdateService.RunWeeklyUpdate(A<string>.Ignored,
                TeamSeasonController.SelectedSeasonYear)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndex()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService);

            int? seasonYear = 1920;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            TeamSeasonController.SelectedSeasonYear.ShouldBe(seasonYear.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService);

            int? seasonYear = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
