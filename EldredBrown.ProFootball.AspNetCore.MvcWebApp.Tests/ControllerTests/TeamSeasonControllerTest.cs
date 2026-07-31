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
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamSeasonControllerTest
    {
        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNotNull_ShouldSetSelectedSeasonYearAndReturnTeamSeasonsIndexView()
        {
            int? selectedSeasonYear = 1920;
            var teamSeasonViewModels = new List<TeamSeasonViewModel>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            TeamSeason? teamSeason = new()
            {
                TeamId = 1,
                SeasonYear = 1920
            };

            (
                TeamSeasonController testController,
                List<Season> seasons,
                List<TeamSeason> teamSeasons,
                _, _, _
            ) = SetUp(selectedSeasonYear: selectedSeasonYear, teamSeasonViewModels: teamSeasonViewModels, teamSeason: teamSeason);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._teamSeasonIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._teamSeasonIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            testController._teamSeasonIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._teamSeasonIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._teamSeasonIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            testController._teamSeasonIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(selectedSeasonYear.Value))
                .MustHaveHappenedOnceExactly();
            foreach (var ts in teamSeasons)
            {
                A.CallTo(() => testController._teamSeasonViewModelMapper.MapTeamSeasonToViewModel(ts))
                    .MustHaveHappenedOnceExactly();
            }
            testController._teamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._teamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNull_ShouldSetSelectedSeasonYearAndReturnTeamSeasonsIndexView()
        {
            // Arrange
            var teamSeasonViewModels = new List<TeamSeasonViewModel>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            TeamSeason? teamSeason = new()
            {
                TeamId = 1,
                SeasonYear = 1920
            };

            (
                TeamSeasonController testController,
                List<Season> seasons,
                List<TeamSeason> teamSeasons,
                _, _, _
            ) = SetUp(teamSeasonViewModels: teamSeasonViewModels, teamSeason: teamSeason);

            // Act
            var result = await testController.Index();

            // Assert
            var defaultSeasonYear = 1922;
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._teamSeasonIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            testController._teamSeasonIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Year));
            testController._teamSeasonIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Year");
            testController._teamSeasonIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Year");
            testController._teamSeasonIndexViewModel.Seasons.SelectedValue.ShouldBe(defaultSeasonYear);
            testController._teamSeasonIndexViewModel.SelectedSeasonYear.ShouldBe(defaultSeasonYear);
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsBySeasonAsync(defaultSeasonYear))
                .MustHaveHappenedOnceExactly();
            foreach (var ts in teamSeasons)
            {
                A.CallTo(() => testController._teamSeasonViewModelMapper.MapTeamSeasonToViewModel(ts))
                    .MustHaveHappenedOnceExactly();
            }
            testController._teamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._teamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonIsFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonViewModel = new TeamSeasonViewModel { Id = 1 };
            TeamSeason? teamSeason = new()
            {
                TeamId = 1,
                SeasonYear = 1920
            };

            (
                TeamSeasonController testController,
                List<Season> seasons,
                List<TeamSeason> teamSeasons,
                List<TeamSeasonOpponentProfile> teamSeasonScheduleProfile,
                TeamSeasonScheduleTotals teamSeasonScheduleTotals,
                TeamSeasonScheduleAverages teamSeasonScheduleAverages
            ) = SetUp(teamSeasonViewModel: teamSeasonViewModel, teamSeason: teamSeason);

            // Act
            int? id = 1;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                .MustHaveHappenedOnceExactly();
            testController._teamSeasonDetailsViewModel.TeamSeason.ShouldBe(teamSeasonViewModel);

            A.CallTo(() => testController._teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(
                teamSeason.TeamId, teamSeason.SeasonYear
                )).MustHaveHappenedOnceExactly();
            testController._teamSeasonDetailsViewModel.TeamSeasonScheduleProfile.ShouldBe(teamSeasonScheduleProfile);

            A.CallTo(() => testController._teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(
                teamSeason.TeamId, teamSeason.SeasonYear
                )).MustHaveHappenedOnceExactly();
            testController._teamSeasonDetailsViewModel.TeamSeasonScheduleTotals.ShouldBe(teamSeasonScheduleTotals);

            A.CallTo(() => testController._teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(
                teamSeason.TeamId, teamSeason.SeasonYear
                )).MustHaveHappenedOnceExactly();
            testController._teamSeasonDetailsViewModel.TeamSeasonScheduleAverages.ShouldBe(teamSeasonScheduleAverages);

            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._teamSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            (TeamSeasonController testController, _, _, _, _, _) = SetUp();

            // Act
            int? id = null;
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamSeasonIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            (TeamSeasonController testController, _, _, _, _, _) = SetUp();

            // Act
            int? id = 1;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task RunWeeklyUpdate_ShouldRunWeeklyUpdateAndRedirectToIndex()
        {
            // Arrange
            var selectedSeasonYear = 1920;

            (
                TeamSeasonController testController,
                List<Season> seasons,
                List<TeamSeason> teamSeasons,
                List<TeamSeasonOpponentProfile> teamSeasonScheduleProfile,
                TeamSeasonScheduleTotals teamSeasonScheduleTotals,
                TeamSeasonScheduleAverages teamSeasonScheduleAverages
            ) = SetUp(selectedSeasonYear: selectedSeasonYear);

            // Act
            var result = await testController.RunWeeklyUpdate();

            // Assert
            A.CallTo(() => testController._weeklyUpdateService.RunWeeklyUpdate(An<int>.Ignored, selectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndex()
        {
            // Arrange
            var seasonYearIn = 1922;
            (TeamSeasonController testController, _, _, _, _, _) = SetUp(selectedSeasonYear: seasonYearIn);

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
        public void SetSelectedSeasonYear_WhenSeasonYearIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var seasonYearIn = 1922;
            (TeamSeasonController testController, _, _, _, _, _) = SetUp(selectedSeasonYear: seasonYearIn);

            // Act
            var result = testController.SetSelectedSeasonYear(null);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }

        private static (
            TeamSeasonController testController, List<Season> seasons, List<TeamSeason> teamSeasons,
            List<TeamSeasonOpponentProfile> teamSeasonScheduleProfile,
            TeamSeasonScheduleTotals teamSeasonScheduleTotals,
            TeamSeasonScheduleAverages teamSeasonScheduleAverages
        ) SetUp(
            int? selectedSeasonYear = null,
            List<TeamSeasonViewModel>? teamSeasonViewModels = null, TeamSeasonViewModel? teamSeasonViewModel = null,
            TeamSeason? teamSeason = null
        )
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            ITeamSeasonViewModelMapper fakeTeamSeasonViewModelMapper = 
                SetUpFakeTeamSeasonViewModelMapper(teamSeasonViewModels, teamSeasonViewModel);
            (ISeasonRepository fakeSeasonRepository, List<Season> seasons) = SetUpFakeSeasonRepository();
            (ITeamSeasonRepository fakeTeamSeasonRepository, List<TeamSeason> teamSeasons) =
                SetUpFakeTeamSeasonRepositoy(teamSeason);
            (
                ITeamSeasonScheduleRepository fakeTeamSeasonScheduleRepository,
                List<TeamSeasonOpponentProfile> teamSeasonScheduleProfile,
                TeamSeasonScheduleTotals teamSeasonScheduleTotals,
                TeamSeasonScheduleAverages teamSeasonScheduleAverages,
                IWeeklyUpdateService fakeWeeklyUpdateService
            ) = SetUpTeamSeasonScheduleRepository();

            Mock<HttpContext> httpContext = SetUpHttpContext(selectedSeasonYear);

            var testController = new TeamSeasonController(
                fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper,
                fakeSeasonRepository, fakeTeamSeasonRepository, fakeTeamSeasonScheduleRepository,
                fakeWeeklyUpdateService
            )
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            return (
                testController, seasons, teamSeasons,
                teamSeasonScheduleProfile, teamSeasonScheduleTotals, teamSeasonScheduleAverages
            );
        }

        private static ITeamSeasonViewModelMapper SetUpFakeTeamSeasonViewModelMapper(
            List<TeamSeasonViewModel>? teamSeasonViewModels, TeamSeasonViewModel? teamSeasonViewModel
        )
        {
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            if (teamSeasonViewModels is not null)
            {
                A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                    .ReturnsNextFromSequence([.. teamSeasonViewModels]);
            }
            if (teamSeasonViewModel is not null)
            {
                A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                    .Returns(teamSeasonViewModel);
            }

            return fakeTeamSeasonViewModelMapper;
        }

        private static (ISeasonRepository, List<Season>) SetUpFakeSeasonRepository()
        {
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            return (fakeSeasonRepository, seasons);
        }

        private static (ITeamSeasonRepository, List<TeamSeason>)
            SetUpFakeTeamSeasonRepositoy(TeamSeason? teamSeason)
        {
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored)).Returns(teamSeasons);
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            return (fakeTeamSeasonRepository, teamSeasons);
        }

        private static (
            ITeamSeasonScheduleRepository, List<TeamSeasonOpponentProfile>,
            TeamSeasonScheduleTotals, TeamSeasonScheduleAverages, IWeeklyUpdateService
        ) SetUpTeamSeasonScheduleRepository()
        {
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();

            var teamSeasonScheduleProfile = new List<TeamSeasonOpponentProfile>();
            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(teamSeasonScheduleProfile);

            var teamSeasonScheduleTotals = new TeamSeasonScheduleTotals();
            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(teamSeasonScheduleTotals);

            var teamSeasonScheduleAverages = new TeamSeasonScheduleAverages();
            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(An<int>.Ignored,
                An<int>.Ignored)).Returns(teamSeasonScheduleAverages);

            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            return (
                fakeTeamSeasonScheduleRepository,
                teamSeasonScheduleProfile, teamSeasonScheduleTotals, teamSeasonScheduleAverages,
                fakeWeeklyUpdateService
            );
        }

        private static Mock<HttpContext> SetUpHttpContext(int? selectedSeasonYear)
        {
            var fakeSession = new MockHttpSession();
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);
            return httpContext;
        }
    }
}
