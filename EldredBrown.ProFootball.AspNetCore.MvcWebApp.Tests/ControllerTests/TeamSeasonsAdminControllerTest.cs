using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
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
            var teamSeasonViewModels = new List<TeamSeasonViewModel>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            TeamSeasonAdminController testController = 
                SetUp(teamSeasonViewModels: teamSeasonViewModels, teamSeasons: teamSeasons);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            foreach (var teamSeason in teamSeasons)
            {
                A.CallTo(() => testController._teamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                    .MustHaveHappenedOnceExactly();
            }
            testController._teamSeasonIndexViewModel.TeamSeasons.ShouldBe(teamSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._teamSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonDetailsView()
        {
            // Arrange
            var teamSeasonViewModel = new TeamSeasonViewModel();
            var teamSeason = new TeamSeason();
            TeamSeasonAdminController testController =
                SetUp(teamSeasonViewModel: teamSeasonViewModel, teamSeason: teamSeason);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                .MustHaveHappenedOnceExactly();
            testController._teamSeasonDetailsViewModel.TeamSeason.ShouldNotBeNull();
            testController._teamSeasonDetailsViewModel.TeamSeason.ShouldBeOfType<TeamSeasonViewModel>();
            testController._teamSeasonDetailsViewModel.TeamSeason.ShouldBe(teamSeasonViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._teamSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddTeamSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var teamSeason = new TeamSeason();
            TeamSeasonAdminController testController = SetUp(teamSeason: teamSeason);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.AddAsync(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            var teamSeason = new TeamSeason
            {
                Id = 2
            };
            var ex = new DbUpdateException(
                "DbUpdateException",
                new Exception("Violation of PRIMARY KEY constraint 'PK_TeamSeason'.")
            );
            TeamSeasonAdminController testController = SetUp(teamSeasons: teamSeasons, teamSeason: teamSeason, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.AddAsync(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A TeamSeason with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamId = 1, SeasonYear = 1920 },
                new() { Id = 2, TeamId = 2, SeasonYear = 1920 },
                new() { Id = 3, TeamId = 3, SeasonYear = 1920 },
            };
            var teamSeason = new TeamSeason
            {
                Id = 4,
                TeamId = 2,
                SeasonYear = 1920
            };
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_TeamSeason_Team_Season")
            );
            TeamSeasonAdminController testController = SetUp(teamSeasons: teamSeasons, teamSeason: teamSeason, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.AddAsync(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_TeamSeason_Team_TeamId", "TeamId")]
        [InlineData("FK_TeamSeason_Season_SeasonYear", "SeasonYear")]
        [InlineData("FK_TeamSeason_Association_AssociationId", "AssociationId")]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForForeignKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView(
            string foreignKeyConstraintName, string modelStateKey
        )
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamId = 1, SeasonYear = 1920 },
                new() { Id = 2, TeamId = 2, SeasonYear = 1920 },
                new() { Id = 3, TeamId = 3, SeasonYear = 1920 },
            };
            var teamSeason = new TeamSeason();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The INSERT statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );
            TeamSeasonAdminController testController = SetUp(teamSeasons: teamSeasons, teamSeason: teamSeason, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.AddAsync(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamId = 1, SeasonYear = 1920 },
                new() { Id = 2, TeamId = 2, SeasonYear = 1920 },
                new() { Id = 3, TeamId = 3, SeasonYear = 1920 },
            };
            var teamSeason = new TeamSeason();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Something else")
            );
            TeamSeasonAdminController testController = SetUp(teamSeasons: teamSeasons, teamSeason: teamSeason, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.AddAsync(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnTeamSeasonCreateView()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();
            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var teamSeason = new TeamSeason { };
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Create(teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => testController._teamSeasonRepository.AddAsync(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            TeamSeason? teamSeason = new();
            TeamSeasonAdminController testController = SetUp(teamSeason: teamSeason);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<TeamSeasonViewModel>();
            ((TeamSeasonViewModel)resultModel).TeamSeason.ShouldBe(teamSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsTeamSeasonYearAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateTeamSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            TeamSeasonAdminController testController = SetUp(teamSeason: teamSeason);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.Update(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualTeamSeasonYear_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int id = 0;
            var teamSeason = new TeamSeason { Id = 1 };
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            var ex = new DbUpdateConcurrencyException();
            TeamSeasonAdminController testController = SetUp(teamSeason: teamSeason, teamSeasonExists: false, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.Update(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndTeamSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            var ex = new DbUpdateConcurrencyException();
            TeamSeasonAdminController testController = SetUp(teamSeason: teamSeason, teamSeasonExists: true, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, teamSeasonViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var teamSeasons = new List<TeamSeason>
            {
                new() { Id = 1, TeamId = 1, SeasonYear = 1920 },
                new() { Id = 2, TeamId = 3, SeasonYear = 1921 },
                new() { Id = 3, TeamId = 3, SeasonYear = 1921 },
            };

            int id = 2;
            var teamSeason = new TeamSeason
            {
                Id = id,
                TeamId = 3,
                SeasonYear = 1921
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_TeamSeason_Team_Season")
            );

            TeamSeasonAdminController testController = 
                SetUp(teamSeasons: teamSeasons, teamSeason: teamSeason, teamSeasonExists: true, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.Update(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_TeamSeason_League_LeagueId", "LeagueId")]
        [InlineData("FK_TeamSeason_Season_SeasonYear", "SeasonYear")]
        [InlineData("FK_TeamSeason_Team_TeamId", "TeamId")]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConflict_ShouldHandleExceptionAndReturnViewForSeason(
            string foreignKeyConstraintName, string modelStateKey
        )
        {
            // Arrange
            int id = 2;
            var teamSeason = new TeamSeason { Id = id };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The UPDATE statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );

            TeamSeasonAdminController testController = SetUp(teamSeason: teamSeason, teamSeasonExists: true, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.Update(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            int id = 2;
            var teamSeason = new TeamSeason { Id = id };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Something else")
            );

            TeamSeasonAdminController testController = SetUp(teamSeason: teamSeason, teamSeasonExists: true, ex: ex);

            // Act
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonRepository.Update(teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnTeamSeasonEditView()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();
            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            int id = 1;
            var teamSeason = new TeamSeason { Id = id };
            var teamSeasonViewModel = new TeamSeasonViewModel { TeamSeason = teamSeason };
            var result = await testController.Edit(id, teamSeasonViewModel);

            // Assert
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapViewModelToTeamSeason(teamSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => testController._teamSeasonRepository.Update(teamSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndTeamSeasonFound_ShouldReturnTeamSeasonDeleteView()
        {
            // Arrange
            var teamSeasonViewModel = new TeamSeasonViewModel();
            var teamSeason = new TeamSeason();
            TeamSeasonAdminController testController = 
                SetUp(teamSeasonViewModel: teamSeasonViewModel, teamSeason: teamSeason);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._teamSeasonViewModelMapper.MapTeamSeasonToViewModel(teamSeason))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<TeamSeasonViewModel>();
            resultModel.ShouldBe(teamSeasonViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenTeamSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteTeamSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            TeamSeasonAdminController testController = SetUp();

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static TeamSeasonAdminController SetUp(
            List<TeamSeasonViewModel>? teamSeasonViewModels = null, TeamSeasonViewModel? teamSeasonViewModel = null,
            List<TeamSeason>? teamSeasons = null, TeamSeason? teamSeason = null, bool? teamSeasonExists = null,
            Exception? ex = null
        )
        {
            var fakeTeamSeasonIndexViewModel = A.Fake<ITeamSeasonIndexViewModel>();
            var fakeTeamSeasonDetailsViewModel = A.Fake<ITeamSeasonDetailsViewModel>();
            ITeamSeasonViewModelMapper fakeTeamSeasonViewModelMapper = 
                SetUpFakeTeamSeasonViewModelMapper(teamSeasonViewModels, teamSeasonViewModel, teamSeason);
            ITeamSeasonRepository fakeTeamSeasonRepository = SetUpFakeTeamSeasonRepository(teamSeasons, teamSeason, teamSeasonExists);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(ex);

            return new TeamSeasonAdminController(
                fakeTeamSeasonIndexViewModel, fakeTeamSeasonDetailsViewModel, fakeTeamSeasonViewModelMapper,
                fakeTeamSeasonRepository, fakeSharedRepository
            );
        }

        private static ITeamSeasonViewModelMapper SetUpFakeTeamSeasonViewModelMapper(List<TeamSeasonViewModel>? teamSeasonViewModels, TeamSeasonViewModel? teamSeasonViewModel, TeamSeason? teamSeason)
        {
            var fakeTeamSeasonViewModelMapper = A.Fake<ITeamSeasonViewModelMapper>();
            if (teamSeasonViewModels is not null)
            {
                A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(A<TeamSeason>.Ignored))
                    .ReturnsNextFromSequence(teamSeasonViewModels.ToArray());
            }
            if (teamSeasonViewModel is not null)
            {
                A.CallTo(() => fakeTeamSeasonViewModelMapper.MapTeamSeasonToViewModel(An<TeamSeason>.Ignored))
                    .Returns(teamSeasonViewModel);
            }
            A.CallTo(() => fakeTeamSeasonViewModelMapper.MapViewModelToTeamSeason(A<TeamSeasonViewModel>.Ignored))
                .Returns(teamSeason);
            return fakeTeamSeasonViewModelMapper;
        }

        private static ITeamSeasonRepository SetUpFakeTeamSeasonRepository(List<TeamSeason>? teamSeasons, TeamSeason? teamSeason, bool? teamSeasonExists)
        {
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);
            A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);
            if (teamSeasonExists is not null)
            {
                A.CallTo(() => fakeTeamSeasonRepository.TeamSeasonExistsAsync(An<int>.Ignored))
                    .Returns(teamSeasonExists.Value);
            }

            return fakeTeamSeasonRepository;
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
