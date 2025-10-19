using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Teams;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamsControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnTeamsIndexView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>();
            A.CallTo(() => teamRepository.GetTeamsAsync()).Returns(teams);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            teamsIndexViewModel.Teams.ShouldBe(teams);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamsIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamDetailsView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            teamsDetailsViewModel.Team.ShouldBe(team);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamsDetailsViewModel);
        }

        [Fact]
        public void CreateGet_ShouldReturnTeamCreateView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddTeamToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            var team = new Team();

            // Act
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => teamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnTeamCreateView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            var team = new Team();

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => teamRepository.AddAsync(team)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamEditView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualTeamId_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int id = 0;
            var team = new Team
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, team);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateTeamInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int id = 1;
            var team = new Team
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => teamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndTeamWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => teamRepository.Update(A<Team>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => teamRepository.TeamExists(A<int>.Ignored)).Returns(false);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int id = 1;
            var team = new Team
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, team);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndTeamWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => teamRepository.Update(A<Team>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => teamRepository.TeamExists(A<int>.Ignored)).Returns(true);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int id = 1;
            var team = new Team
            {
                ID = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, team));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamIdAndModelStateIsNotValid_ShouldReturnTeamEditView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int id = 1;
            var team = new Team
            {
                ID = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => teamRepository.Update(A<Team>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamDeleteView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();

            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteTeamFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var teamsIndexViewModel = A.Fake<ITeamsIndexViewModel>();
            var teamsDetailsViewModel = A.Fake<ITeamsDetailsViewModel>();
            var teamRepository = A.Fake<ITeamRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamsController(teamsIndexViewModel, teamsDetailsViewModel,
                teamRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => teamRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
