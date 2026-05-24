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
        public async Task Index_ShouldReturnLeagueSeasonsIndexView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeLeagueSeasonIndexViewModel.LeagueSeasons.ShouldBe(leagueSeasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeLeagueSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDetailsView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            fakeLeagueSeasonDetailsViewModel.LeagueSeason.ShouldBe(leagueSeason);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeLeagueSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

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

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

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
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddLeagueSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            var leagueSeason = new LeagueSeason();

            // Act
            var result = await testController.Create(leagueSeason);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnLeagueSeasonCreateView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            var leagueSeason = new LeagueSeason();

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Create(leagueSeason);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.AddAsync(leagueSeason)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

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

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateLeagueSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
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
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int id = 0;
            var leagueSeason = new LeagueSeason
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => fakeLeagueSeasonRepository.LeagueSeasonExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => fakeLeagueSeasonRepository.Update(A<LeagueSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => fakeLeagueSeasonRepository.LeagueSeasonExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                Id = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, leagueSeason));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDeleteView()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeLeagueSeasonIndexViewModel = A.Fake<ILeagueSeasonIndexViewModel>();
            var fakeLeagueSeasonDetailsViewModel = A.Fake<ILeagueSeasonDetailsViewModel>();
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

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

            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

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
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonController(fakeLeagueSeasonIndexViewModel,
                fakeLeagueSeasonDetailsViewModel, fakeLeagueSeasonRepository, fakeSharedRepository);

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
