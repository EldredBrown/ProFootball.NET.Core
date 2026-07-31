using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class LeagueSeasonControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnLeagueSeasonIndexView()
        {
            // Arrange
            var leagueSeasonViewModels = new List<LeagueSeasonViewModel>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };

            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };

            LeagueSeasonController testController = 
                SetUp(leagueSeasonViewModels: leagueSeasonViewModels, leagueSeasons: leagueSeasons);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            foreach (var leagueSeason in leagueSeasons)
            {
                A.CallTo(() => testController._leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason))
                    .MustHaveHappenedOnceExactly();
            }
            testController._leagueSeasonIndexViewModel.LeagueSeasons.ShouldBe(leagueSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._leagueSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDetailsView()
        {
            // Arrange
            var leagueSeasonViewModel = new LeagueSeasonViewModel();
            var leagueSeason = new LeagueSeason();

            LeagueSeasonController testController =
                SetUp(leagueSeasonViewModel: leagueSeasonViewModel, leagueSeason: leagueSeason);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason))
                .MustHaveHappenedOnceExactly();
            testController._leagueSeasonDetailsViewModel.LeagueSeason.ShouldNotBeNull();
            testController._leagueSeasonDetailsViewModel.LeagueSeason.ShouldBeOfType<LeagueSeasonViewModel>();
            testController._leagueSeasonDetailsViewModel.LeagueSeason.ShouldBe(leagueSeasonViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._leagueSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnLeagueSeasonCreateView()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddLeagueSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var leagueSeason = new LeagueSeason();
            LeagueSeasonController testController = SetUp(leagueSeason: leagueSeason);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.AddAsync(leagueSeason))
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
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            var leagueSeason = new LeagueSeason { Id = 2 };
            var ex = new DbUpdateException(
                "DbUpdateException",
                new Exception("Violation of PRIMARY KEY constraint 'PK_LeagueSeason'.")
            );
            LeagueSeasonController testController = SetUp(leagueSeasons: leagueSeasons, leagueSeason: leagueSeason, ex: ex);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.AddAsync(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A LeagueSeason with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonYear = 1920 },
                new() { Id = 2, LeagueId = 2, SeasonYear = 1920 },
                new() { Id = 3, LeagueId = 3, SeasonYear = 1920 },
            };
            var leagueSeason = new LeagueSeason { Id = 4, LeagueId = 2, SeasonYear = 1920 };
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_LeagueSeason_League_Season")
            );
            LeagueSeasonController testController = SetUp(leagueSeasons: leagueSeasons, leagueSeason: leagueSeason, ex: ex);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.AddAsync(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_LeagueSeason_League_LeagueId", "LeagueId")]
        [InlineData("FK_LeagueSeason_Season_SeasonYear", "SeasonYear")]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForForeignKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView(
            string foreignKeyConstraintName, string modelStateKey)
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonYear = 1920 },
                new() { Id = 2, LeagueId = 2, SeasonYear = 1921 },
                new() { Id = 3, LeagueId = 3, SeasonYear = 1922 },
            };
            var leagueSeason = new LeagueSeason();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The INSERT statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );
            LeagueSeasonController testController = 
                SetUp(leagueSeasons: leagueSeasons, leagueSeason: leagueSeason, ex: ex);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.AddAsync(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenDbUpdateExceptionCaughtForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonYear = 1920 },
                new() { Id = 2, LeagueId = 2, SeasonYear = 1921 },
                new() { Id = 3, LeagueId = 3, SeasonYear = 1922 },
            };
            var leagueSeason = new LeagueSeason();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Something else")
            );
            LeagueSeasonController testController = 
                SetUp(leagueSeasons: leagueSeasons, leagueSeason: leagueSeason, ex: ex);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.AddAsync(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnLeagueSeasonCreateView()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();
            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var leagueSeason = new LeagueSeason { };
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => testController._leagueSeasonRepository.AddAsync(leagueSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            LeagueSeason? leagueSeason = new();
            LeagueSeasonController testController = SetUp(leagueSeason: leagueSeason);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<LeagueSeasonViewModel>();
            ((LeagueSeasonViewModel)resultModel).LeagueSeason.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueSeasonYearAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateLeagueSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            LeagueSeasonController testController = SetUp(leagueSeason: leagueSeason);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualLeagueSeasonYear_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int id = 0;
            var leagueSeason = new LeagueSeason { Id = 1 };
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            bool leagueSeasonExists = false;
            var ex = new DbUpdateConcurrencyException();
            LeagueSeasonController testController = 
                SetUp(leagueSeason: leagueSeason, leagueSeasonExists: leagueSeasonExists, ex: ex);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            bool leagueSeasonExists = true;
            var ex = new DbUpdateConcurrencyException();
            LeagueSeasonController testController =
                SetUp(leagueSeason: leagueSeason, leagueSeasonExists: leagueSeasonExists, ex: ex);

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, leagueSeasonViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonYear = 1920 },
                new() { Id = 2, LeagueId = 3, SeasonYear = 1921 },
                new() { Id = 3, LeagueId = 3, SeasonYear = 1921 },
            };

            int id = 2;
            var leagueSeason = new LeagueSeason { Id = id, LeagueId = 3, SeasonYear = 1921 };

            bool leagueSeasonExists = false;

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint UQ_LeagueSeason_League_Season")
            );

            LeagueSeasonController testController = SetUp(
                leagueSeasons: leagueSeasons, leagueSeason: leagueSeason, leagueSeasonExists: leagueSeasonExists,
                ex: ex
            );

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_LeagueSeason_League_LeagueId", "LeagueId")]
        [InlineData("FK_LeagueSeason_Season_SeasonYear", "SeasonYear")]
        public async Task EditPost_WhenDbUpdateExceptionCaughtForForeignKeyConflict_ShouldHandleExceptionAndReturnViewForSeason(
            string foreignKeyConstraintName, string modelStateKey)
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonYear = 1920 },
                new() { Id = 2, LeagueId = 3, SeasonYear = 1921 },
                new() { Id = 3, LeagueId = 3, SeasonYear = 1921 },
            };

            int id = 2;
            var leagueSeason = new LeagueSeason { Id = id };

            bool leagueSeasonExists = false;

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The UPDATE statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );

            LeagueSeasonController testController = SetUp(
                leagueSeasons: leagueSeasons, leagueSeason: leagueSeason, leagueSeasonExists: leagueSeasonExists,
                ex: ex
            );

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonYear = 1920 },
                new() { Id = 2, LeagueId = 3, SeasonYear = 1921 },
                new() { Id = 3, LeagueId = 3, SeasonYear = 1921 },
            };

            int id = 2;
            var leagueSeason = new LeagueSeason { Id = id };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );

            LeagueSeasonController testController = SetUp(
                leagueSeasons: leagueSeasons, leagueSeason: leagueSeason, ex: ex
            );

            // Act
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonRepository.Update(leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };
            LeagueSeasonController testController = 
                SetUp(leagueSeason: leagueSeason, leagueSeasonViewModel: leagueSeasonViewModel);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => testController._leagueSeasonRepository.Update(leagueSeason))
                .MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDeleteView()
        {
            // Arrange
            var leagueSeasonViewModel = new LeagueSeasonViewModel();
            LeagueSeason? leagueSeason = new();
            LeagueSeasonController testController =
                SetUp(leagueSeasonViewModel: leagueSeasonViewModel, leagueSeason: leagueSeason);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._leagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<LeagueSeasonViewModel>();
            resultModel.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteLeagueSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            LeagueSeasonController testController = SetUp();

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static LeagueSeasonController SetUp(
            List<LeagueSeasonViewModel>? leagueSeasonViewModels = null, LeagueSeasonViewModel? leagueSeasonViewModel = null,
            List<LeagueSeason>? leagueSeasons = null, LeagueSeason? leagueSeason = null, bool? leagueSeasonExists = null,
            Exception? ex = null
        )
        {
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            ILeagueSeasonViewModelMapper fakeLeagueSeasonViewModelMapper = 
                SetUpFakeLeagueSeasonViewModelMapper(leagueSeasonViewModels, leagueSeasonViewModel, leagueSeason);
            ILeagueSeasonRepository fakeLeagueSeasonRepository =
                SetUpFakeLeagueSeasonRepository(leagueSeasons, leagueSeason, leagueSeasonExists);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(ex);

            return new LeagueSeasonController(
                fakeLeagueSeasonIndexViewModel, fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper,
                fakeLeagueSeasonRepository, fakeSharedRepository
            );
        }

        private static ILeagueSeasonViewModelMapper SetUpFakeLeagueSeasonViewModelMapper(
            List<LeagueSeasonViewModel>? leagueSeasonViewModels, 
            LeagueSeasonViewModel? leagueSeasonViewModel, LeagueSeason? leagueSeason
        )
        {
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            if (leagueSeasonViewModels is not null)
            {
                A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(A<LeagueSeason>.Ignored))
                    .ReturnsNextFromSequence(leagueSeasonViewModels.ToArray());
            }
            if (leagueSeasonViewModel is not null)
            {
                A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(A<LeagueSeason>.Ignored))
                    .Returns(leagueSeasonViewModel);
            }
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(leagueSeason);
            return fakeLeagueSeasonViewModelMapper;
        }

        private static ILeagueSeasonRepository SetUpFakeLeagueSeasonRepository(
            List<LeagueSeason>? leagueSeasons, LeagueSeason? leagueSeason, bool? leagueSeasonExists
        )
        {
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);
            if (leagueSeasonExists.HasValue)
            {
                A.CallTo(() => fakeLeagueSeasonRepository.LeagueSeasonExistsAsync(An<int>.Ignored))
                    .Returns(leagueSeasonExists.Value);
            }

            return fakeLeagueSeasonRepository;
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
