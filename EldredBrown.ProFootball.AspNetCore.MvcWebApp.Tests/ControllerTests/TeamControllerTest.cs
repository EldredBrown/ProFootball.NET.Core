using System;
using System.Collections.Generic;
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
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team> { };
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamDetailsView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var team = new Team { };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            fakeTeamDetailsViewModel.Team.ShouldNotBeNull();
            fakeTeamDetailsViewModel.Team.ShouldBeOfType<Team>();
            fakeTeamDetailsViewModel.Team.ShouldBe(team);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeTeamDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnTeamCreateView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddTeamToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var team = new Team { };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => fakeTeamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>
            {
                new Team
                {
                    Id = 1,
                    Name = "Team 1"
                },
                new Team
                {
                    Id = 2,
                    Name = "Team 2"
                },
                new Team
                {
                    Id = 3,
                    Name = "Team 3"
                },
            };
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var team = new Team
            {
                Id = 2,
                Name = "Team 4"
            };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => fakeTeamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A team with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>
            {
                new Team
                {
                    Id = 1,
                    Name = "Team 1"
                },
                new Team
                {
                    Id = 2,
                    Name = "Team 2"
                },
                new Team
                {
                    Id = 3,
                    Name = "Team 3"
                },
            };
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var team = new Team
            {
                Id = 4,
                Name = "Team 2"
            };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => fakeTeamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A team with the same name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>
            {
                new Team
                {
                    Id = 1,
                    Name = "Team 1"
                },
                new Team
                {
                    Id = 2,
                    Name = "Team 2"
                },
                new Team
                {
                    Id = 3,
                    Name = "Team 3"
                },
            };
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var team = new Team { };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => fakeTeamRepository.AddAsync(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnTeamCreateView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var team = new Team { };
            var result = await testController.Create(team);

            // Assert
            A.CallTo(() => fakeTeamRepository.AddAsync(team)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamEditView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team { };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
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
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var result = await testController.Edit(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateTeamInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var team = new Team
            {
                Id = id
            };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => fakeTeamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualTeamId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int id = 0;
            var team = new Team
            {
                Id = 1
            };
            var result = await testController.Edit(id, team);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => fakeTeamRepository.TeamExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var team = new Team
            {
                Id = id
            };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => fakeTeamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => fakeTeamRepository.TeamExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var team = new Team
            {
                Id = id
            };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, team));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>
            {
                new Team
                {
                    Id = 1,
                    Name = "Team 1"
                },
                new Team
                {
                    Id = 2,
                    Name = "Team 3"
                },
                new Team
                {
                    Id = 3,
                    Name = "Team 3"
                },
            };
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var team = new Team
            {
                Id = id,
                Name = "Team 3"
            };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => fakeTeamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A team with the same name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>
            {
                new Team
                {
                    Id = 1,
                    Name = "Team 1"
                },
                new Team
                {
                    Id = 2,
                    Name = "Team 2"
                },
                new Team
                {
                    Id = 3,
                    Name = "Team 3"
                },
            };
            A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var team = new Team { Id = id };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => fakeTeamRepository.Update(team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnTeamEditView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            int id = 1;
            var team = new Team
            {
                Id = 1
            };
            var result = await testController.Edit(id, team);

            // Assert
            A.CallTo(() => fakeTeamRepository.Update(team)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(team);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamFound_ShouldReturnTeamDeleteView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team { };
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
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
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            var result = await testController.Delete(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenTeamNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();

            var fakeTeamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeTeamRepository.GetTeamAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteTeamFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeTeamIndexViewModel = A.Fake<ITeamIndexViewModel>();
            var fakeTeamDetailsViewModel = A.Fake<ITeamDetailsViewModel>();
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new TeamController(fakeTeamIndexViewModel, fakeTeamDetailsViewModel,
                fakeTeamRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeTeamRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
