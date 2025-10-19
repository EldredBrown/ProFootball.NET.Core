using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeasons;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamSeasonsControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnTeamSeasonsIndexView()
        {
            // Arrange
            int selectedSeasonYear = 1920;
            TeamSeasonsController.SelectedSeasonYear = selectedSeasonYear;

            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(A<int>.Ignored)).Returns(teamSeasons);

            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonsController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                seasonRepository, teamSeasonRepository, teamSeasonScheduleRepository, sharedRepository,
                weeklyUpdateService);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            teamSeasonsIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            teamSeasonsIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            teamSeasonsIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            teamSeasonsIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            teamSeasonsIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            teamSeasonsIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsBySeasonAsync(selectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            teamSeasonsIndexViewModel.TeamSeasons.ShouldBe(teamSeasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonsIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonsController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                seasonRepository, teamSeasonRepository, teamSeasonScheduleRepository, sharedRepository,
                weeklyUpdateService);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonsController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                seasonRepository, teamSeasonRepository, teamSeasonScheduleRepository, sharedRepository,
                weeklyUpdateService);

            int? id = 1;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonIsFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            string teamName = "Team";
            int seasonYear = 1920;
            TeamSeason? teamSeason = new TeamSeason
            {
                TeamName = teamName,
                SeasonYear = seasonYear
            };
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();

            var teamSeasonScheduleProfile = new List<TeamSeasonOpponentProfile>();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleProfile);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(A<string>.Ignored,
                A<int>.Ignored)).Returns(teamSeasonScheduleAverages);

            var sharedRepository = A.Fake<ISharedRepository>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonsController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                seasonRepository, teamSeasonRepository, teamSeasonScheduleRepository, sharedRepository,
                weeklyUpdateService);

            int? id = 1;

            // Act
            var result = await testController.Details(id);

            // Assert
            teamSeasonsDetailsViewModel.TeamSeason = teamSeason;

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            teamSeasonsDetailsViewModel.TeamSeasonScheduleProfile.ShouldBe(teamSeasonScheduleProfile);

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear))
                .MustHaveHappenedOnceExactly();
            teamSeasonsDetailsViewModel.TeamSeasonScheduleTotals.ShouldBe(teamSeasonScheduleTotals);

            A.CallTo(() => teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName,
                seasonYear)).MustHaveHappenedOnceExactly();
            teamSeasonsDetailsViewModel.TeamSeasonScheduleAverages.ShouldBe(teamSeasonScheduleAverages);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonsDetailsViewModel);
        }

        [Fact]
        public async Task RunWeeklyUpdate_ShouldRunWeeklyUpdateAndRedirectToIndex()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonsController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                seasonRepository, teamSeasonRepository, teamSeasonScheduleRepository, sharedRepository,
                weeklyUpdateService);

            // Act
            var result = await testController.RunWeeklyUpdate();

            // Assert
            A.CallTo(() => weeklyUpdateService.RunWeeklyUpdate(TeamSeasonsController.SelectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonsController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                seasonRepository, teamSeasonRepository, teamSeasonScheduleRepository, sharedRepository,
                weeklyUpdateService);

            int? seasonYear = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndex()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonsController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                seasonRepository, teamSeasonRepository, teamSeasonScheduleRepository, sharedRepository,
                weeklyUpdateService);

            int? seasonYear = 1920;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonYear);

            // Assert
            TeamSeasonsController.SelectedSeasonYear.ShouldBe(seasonYear.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
