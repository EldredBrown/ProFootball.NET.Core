using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamSeasonAdminControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnTeamSeasonIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeTeamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonDetailsView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel,
                fakeTeamSeasonDetailsViewModel, fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            fakeTeamSeasonDetailsViewModel.TeamSeason.ShouldBe(teamSeason);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddTeamSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            var teamSeason = new TeamSeason();

            // Act
            var result = await testController.Create(teamSeason);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            var teamSeason = new TeamSeason();

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Create(teamSeason);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.AddAsync(teamSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateTeamSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.Update(teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualTeamSeasonId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int id = 0;
            var teamSeason = new TeamSeason
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                Id = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => fakeTeamSeasonRepository.TeamSeasonExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, teamSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => fakeTeamSeasonRepository.Update(A<TeamSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => fakeTeamSeasonRepository.TeamSeasonExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int id = 1;
            var teamSeason = new TeamSeason
            {
                Id = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, teamSeason));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonDeleteView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();

            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteTeamSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamSeasonAdminController(fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel,
                fakeTeamSeasonRepository, fakeSharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeTeamSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
