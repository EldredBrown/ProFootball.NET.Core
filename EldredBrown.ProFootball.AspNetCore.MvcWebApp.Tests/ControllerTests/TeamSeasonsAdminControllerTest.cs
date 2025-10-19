using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeasons;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamSeasonsAdminControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnTeamSeasonsIndexView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
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
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

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

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonIsFound_ShouldReturnTeamSeasonDetailsView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            teamSeasonsDetailsViewModel.TeamSeason.ShouldBe(teamSeason);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonsDetailsViewModel);
        }

        [Fact]
        public void CreateGet_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddTeamSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            var teamSeason = new TeamSeason();

            // Act
            var result = await testController.Create(teamSeason);

            // Assert
            A.CallTo(() => teamSeasonRepository.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            var teamSeason = new TeamSeason();

            testController.ModelState.AddModelError("TeamName", "Please enter a team name.");

            // Act
            var result = await testController.Create(teamSeason);

            // Assert
            A.CallTo(() => teamSeasonRepository.AddAsync(teamSeason)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamSeasonIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamSeasonIsFound_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualTeamSeasonId_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int id = 0;
            var teamSeason = new TeamSeason
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateTeamSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            A.CallTo(() => teamSeasonRepository.Update(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => teamSeasonRepository.Update(A<TeamSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => teamSeasonRepository.TeamSeasonExists(A<int>.Ignored)).Returns(false);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => teamSeasonRepository.Update(A<TeamSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => teamSeasonRepository.TeamSeasonExists(A<int>.Ignored)).Returns(true);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                ID = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, teamSeason));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamSeasonIdAndModelStateIsNotValid_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                ID = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            A.CallTo(() => teamSeasonRepository.Update(A<TeamSeason>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamSeasonIsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamSeasonIsFound_ShouldReturnTeamSeasonDeleteView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();

            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int? id = 1;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteTeamSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var teamSeasonsIndexViewModel = A.Fake<ITeamSeasonsIndexViewModel>();
            var teamSeasonsDetailsViewModel = A.Fake<ITeamSeasonsDetailsViewModel>();
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonsAdminController(teamSeasonsIndexViewModel, teamSeasonsDetailsViewModel,
                teamSeasonRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
