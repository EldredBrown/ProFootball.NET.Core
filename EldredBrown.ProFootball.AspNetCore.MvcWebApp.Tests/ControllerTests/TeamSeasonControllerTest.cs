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
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeasonViewModels = new List<TeamSeasonViewModel>
            {
                new TeamSeasonViewModel { Id = 1 },
                new TeamSeasonViewModel { Id = 2 },
                new TeamSeasonViewModel { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                .ReturnsNextFromSequence(teamSeasonViewModels.ToArray());

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season{ Id = 1920 },
                new Season{ Id = 1921 },
                new Season{ Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1 },
                new TeamSeason { Id = 2 },
                new TeamSeason { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored)).Returns(teamSeasons);

            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var fakeSession = new MockHttpSession();
            int? selectedSeasonYear = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService)
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
            fakeTeamSeasonIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeTeamSeasonIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            fakeTeamSeasonIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeTeamSeasonIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeTeamSeasonIndexViewModel.Seasons.SelectedValue.ShouldBe(selectedSeasonYear);
            fakeTeamSeasonIndexViewModel.SelectedSeasonYear.ShouldBe(selectedSeasonYear);
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(selectedSeasonYear.Value))
                .MustHaveHappenedOnceExactly();
            foreach (var ts in teamSeasons)
            {
                A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(ts))
                    .MustHaveHappenedOnceExactly();
            }
            fakeTeamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Index_WhenSelectedSeasonYearIsNull_ShouldSetSelectedSeasonYearAndReturnTeamSeasonsIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeasonViewModels = new List<TeamSeasonViewModel>
            {
                new TeamSeasonViewModel { Id = 1 },
                new TeamSeasonViewModel { Id = 2 },
                new TeamSeasonViewModel { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                .ReturnsNextFromSequence(teamSeasonViewModels.ToArray());

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new Season{ Id = 1920 },
                new Season{ Id = 1921 },
                new Season{ Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>
            {
                new TeamSeason { Id = 1 },
                new TeamSeason { Id = 2 },
                new TeamSeason { Id = 3 },
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(An<int>.Ignored)).Returns(teamSeasons);

            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var fakeSession = new MockHttpSession();
            int? selectedSeasonYear = null;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService)
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
            fakeTeamSeasonIndexViewModel.Seasons.ShouldBeOfType<SelectList>();
            fakeTeamSeasonIndexViewModel.Seasons.Items.ShouldBe(seasons.OrderByDescending(s => s.Id));
            fakeTeamSeasonIndexViewModel.Seasons.DataValueField.ShouldBe<string>("Id");
            fakeTeamSeasonIndexViewModel.Seasons.DataTextField.ShouldBe<string>("Id");
            fakeTeamSeasonIndexViewModel.Seasons.SelectedValue.ShouldBe(1922);
            fakeTeamSeasonIndexViewModel.SelectedSeasonYear.ShouldBe(1922);
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsBySeasonAsync(1922))
                .MustHaveHappenedOnceExactly();
            foreach (var ts in teamSeasons)
            {
                A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(ts))
                    .MustHaveHappenedOnceExactly();
            }
            fakeTeamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonIsFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var teamSeasonViewModel = new TeamSeasonViewModel { Id = 1 };
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                .Returns(teamSeasonViewModel);

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            int teamId = 1;
            int seasonId = 1920;
            TeamSeason? teamSeason = new TeamSeason
            {
                TeamId = teamId,
                SeasonId = seasonId
            };
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

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

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService);

            // Act
            int? id = 1;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                .MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeason.ShouldBe(teamSeasonViewModel);

            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamId, seasonId))
                .MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeasonScheduleProfile.ShouldBe(teamSeasonScheduleProfile);

            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamId, seasonId))
                .MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeasonScheduleTotals.ShouldBe(teamSeasonScheduleTotals);

            A.CallTo(() => fakeTeamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamId, seasonId))
                .MustHaveHappenedOnceExactly();
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
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamSeasonIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService);

            // Act
            int? id = 1;
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
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYear = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYear);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            var result = await testController.RunWeeklyUpdate();

            // Assert
            A.CallTo(() => fakeWeeklyUpdateService.RunWeeklyUpdate(An<int>.Ignored, selectedSeasonYear))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearIsNotNull_ShouldSetSelectedSeasonYearAndRedirectToIndex()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var fakeSession = new MockHttpSession();
            var selectedSeasonYearToSession = 1922;
            fakeSession.SetObject("SelectedSeasonYear", selectedSeasonYearToSession);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.Session).Returns(fakeSession);

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext.Object
                }
            };

            // Act
            int? seasonId = 1920;
            var result = testController.SetSelectedSeasonYear(seasonId);

            // Assert
            var selectedSeasonYearFromSession = testController.HttpContext.Session.GetObject<int?>("SelectedSeasonYear");
            selectedSeasonYearFromSession.ShouldBe(seasonId.Value);
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public void SetSelectedSeasonYear_WhenSeasonYearIsNull_ShouldReturnBadRequest()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeTeamSeasonScheduleRepository = A.Fake<ITeamSeasonScheduleRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new TeamSeasonController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonViewModelMapper, fakeSeasonRepository, fakeTeamSeasonRepository,
                fakeTeamSeasonScheduleRepository, fakeWeeklyUpdateService);

            int? seasonId = null;

            // Act
            var result = testController.SetSelectedSeasonYear(seasonId);

            // Assert
            result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
