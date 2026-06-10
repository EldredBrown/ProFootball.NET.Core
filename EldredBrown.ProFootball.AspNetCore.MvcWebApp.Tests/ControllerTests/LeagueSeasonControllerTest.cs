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
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeasonViewModels = new List<LeagueSeasonViewModel>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(A<LeagueSeason>.Ignored))
                .ReturnsNextFromSequence(leagueSeasonViewModels.ToArray());

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            foreach (var leagueSeason in leagueSeasons)
            {
                A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason))
                    .MustHaveHappenedOnceExactly();
            }
            fakeLeagueSeasonIndexViewModel.LeagueSeasons.ShouldBe(leagueSeasonViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeLeagueSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDetailsView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeasonViewModel = new LeagueSeasonViewModel { };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(An<LeagueSeason>.Ignored))
                .Returns(leagueSeasonViewModel);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeason = new LeagueSeason { };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason))
                .MustHaveHappenedOnceExactly();
            fakeLeagueSeasonDetailsViewModel.LeagueSeason.ShouldNotBeNull();
            fakeLeagueSeasonDetailsViewModel.LeagueSeason.ShouldBeOfType<LeagueSeasonViewModel>();
            fakeLeagueSeasonDetailsViewModel.LeagueSeason.ShouldBe(leagueSeasonViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeLeagueSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnLeagueSeasonCreateView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddLeagueSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeason = new LeagueSeason { };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeason = new LeagueSeason { Id = 2 };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A LeagueSeason with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeason = new LeagueSeason { Id = 4, LeagueId = 2, SeasonId = 1920 };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new() { Id = 1, LeagueId = 1, SeasonId = 1920 },
                new() { Id = 2, LeagueId = 2, SeasonId = 1920 },
                new() { Id = 3, LeagueId = 3, SeasonId = 1920 },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A LeagueSeason with the same league name and season year already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_LeagueSeason_League_LeagueId", "LeagueId")]
        [InlineData("FK_LeagueSeason_Season_SeasonId", "SeasonId")]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView(
            string foreignKeyConstraintName, string modelStateKey)
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeason = new LeagueSeason { };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The INSERT statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeason = new LeagueSeason { };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnLeagueSeasonCreateView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            var leagueSeason = new LeagueSeason { };
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Create(leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new() { };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
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
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueSeasonIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateLeagueSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualLeagueSeasonId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int id = 0;
            var leagueSeason = new LeagueSeason { Id = 1 };
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.LeagueSeasonExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.LeagueSeasonExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, leagueSeasonViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            int id = 2;
            var leagueSeason = new LeagueSeason { Id = id, LeagueId = 3, SeasonId = 1921 };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>
            {
                new LeagueSeason { Id = 1, LeagueId = 1, SeasonId = 1920 },
                new LeagueSeason { Id = 2, LeagueId = 3, SeasonId = 1921 },
                new LeagueSeason { Id = 3, LeagueId = 3, SeasonId = 1921 },
            };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A LeagueSeason with the same league name and season year already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Theory]
        [InlineData("FK_LeagueSeason_League_LeagueId", "LeagueId")]
        [InlineData("FK_LeagueSeason_Season_SeasonId", "SeasonId")]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConflict_ShouldHandleExceptionAndReturnViewForSeason(
            string foreignKeyConstraintName, string modelStateKey)
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            int id = 2;
            var leagueSeason = new LeagueSeason { Id = id };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception($"The UPDATE statement conflicted with the FOREIGN KEY constraint \"{foreignKeyConstraintName}\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe($"Unable to save changes. Conflict with a FOREIGN KEY constraint on {modelStateKey}.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            int id = 2;
            var leagueSeason = new LeagueSeason { Id = id };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(A<LeagueSeasonViewModel>.Ignored))
                .Returns(Task.FromResult(leagueSeason));

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            int id = 1;
            var leagueSeason = new LeagueSeason { Id = id };
            var leagueSeasonViewModel = new LeagueSeasonViewModel { LeagueSeason = leagueSeason };

            // Act
            var result = await testController.Edit(id, leagueSeasonViewModel);

            // Assert
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapViewModelToLeagueSeason(leagueSeasonViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(leagueSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDeleteView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var leagueSeasonViewModel = new LeagueSeasonViewModel { };
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(A<LeagueSeason>.Ignored))
                .Returns(leagueSeasonViewModel);

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason { };
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeLeagueSeasonViewModelMapper.MapLeagueSeasonToViewModel(leagueSeason))
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
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteLeagueSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonViewModelMapper = A.Fake<ILeagueSeasonViewModelMapper>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonAdminController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonViewModelMapper, fakeLeagueSeasonRepository,
                fakeSharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
