using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Team;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class TeamControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnTeamIndexView()
        {
            // Arrange
            var teams = new List<Team>();
            TeamController testController = SetUp(teams: teams);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._teamIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamDetailsView()
        {
            // Arrange
            var team = new Team();
            TeamController testController = SetUp(team: team);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            testController._teamDetailsViewModel.Team.ShouldNotBeNull();
            testController._teamDetailsViewModel.Team.ShouldBeOfType<Team>();
            testController._teamDetailsViewModel.Team.ShouldBe(team);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._teamDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnTeamCreateView()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddTeamToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            var team = new Team { };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => testController._teamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };

            var ex = new DbUpdateException();

            TeamController testController = SetUp(teams: teams, ex: ex);

            // Act
            var team = new Team { Id = 2, Name = "Team 4" };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => testController._teamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A team with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Team', column 'name'.")
            );

            TeamController testController = SetUp(teams: teams, ex: ex);

            // Act
            var name = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                name.Append('Z');
            }
            var team = new Team { Id = 4, Name = name.ToString() };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => testController._teamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered Name is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForUniqueKeyViolationOnName_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Team_Name'.")
            );

            TeamController testController = SetUp(teams: teams, ex: ex);

            // Act
            var team = new Team { Id = 4, Name = "Team 2" };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => testController._teamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint Name.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );

            TeamController testController = SetUp(teams: teams, ex: ex);

            // Act
            var team = new Team();
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => testController._teamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnTeamCreateView()
        {
            // Arrange
            TeamController testController = SetUp();
            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var team = new Team { };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => testController._teamRepository.AddAsync(team)).MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamEditView()
        {
            // Arrange
            Team team = new();

            TeamController testController = SetUp(team: team);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<Team>();
            resultModel.ShouldBe(team);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateTeamInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int id = 1;
            var team = new Team { Id = id };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => testController._teamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualTeamId_ShouldReturnNotFound()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int id = 0;
            var team = new Team { Id = 1 };
            var result = await testController.Edit(id, team);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var ex = new DbUpdateConcurrencyException();

            TeamController testController = SetUp(teamExists: false, ex: ex);

            // Act
            int id = 1;
            var team = new Team { Id = id };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => testController._teamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var ex = new DbUpdateConcurrencyException();

            TeamController testController = SetUp(teamExists: true, ex: ex);

            // Act
            int id = 1;
            var team = new Team { Id = id };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, team));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForNameTooLong_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Team', column 'name'.")
            );

            TeamController testController = SetUp(teams: teams, ex: ex);

            // Act
            int id = 2;
            var team = new Team { Id = id, Name = "Team 3" };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => testController._teamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered Name is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolationOnName_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Team_Name'.")
            );

            TeamController testController = SetUp(teams: teams, ex: ex);

            // Act
            int id = 2;
            var team = new Team { Id = id, Name = "Team 3" };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => testController._teamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint Name.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var teams = new List<Team>
            {
                new() { Id = 1, Name = "Team 1" },
                new() { Id = 2, Name = "Team 2" },
                new() { Id = 3, Name = "Team 3" },
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );

            TeamController testController = SetUp(teams: teams, ex: ex);

            // Act
            int id = 2;
            var team = new Team { Id = id };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => testController._teamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnTeamEditView()
        {
            // Arrange
            TeamController testController = SetUp();
            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            int id = 1;
            var team = new Team { Id = 1 };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => testController._teamRepository.Update(team)).MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamDeleteView()
        {
            // Arrange
            Team? team = new();

            TeamController testController = SetUp(team: team);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<Team>();
            resultModel.ShouldBe(team);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteTeamFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            TeamController testController = SetUp();

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => testController._teamRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static TeamController SetUp(List<Team>? teams = null, Team? team = null, bool? teamExists = null, Exception? ex = null)
        {
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            ITeamRepository fakeTeamRepository = SetUpFakeTeamRepository(teams, team, teamExists);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(ex);

            return new TeamController(
                fakeTeamIndexViewModel, fakeTeamDetailsViewModel, fakeTeamRepository, fakeSharedRepository
            );
        }

        private static ITeamRepository SetUpFakeTeamRepository(List<Team>? teams, Team? team, bool? teamExists)
        {
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);
            if (teamExists.HasValue)
            {
                A.CallTo(() => fakeTeamRepository.TeamExistsAsync(An<int>.Ignored)).Returns(teamExists.Value);
            }

            return fakeTeamRepository;
        }

        private static ISharedRepository SetUpFakeSharedRepository(Exception? ex)
        {
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            if (ex is not null)
            {
                A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);
            }

            return fakeSharedRepository;
        }
    }
}
