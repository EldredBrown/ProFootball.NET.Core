using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeasons;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class LeagueSeasonsControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnLeagueSeasonsIndexView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            leagueSeasonsIndexViewModel.LeagueSeasons.ShouldBe(leagueSeasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonsIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDetailsView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            leagueSeasonsDetailsViewModel.LeagueSeason.ShouldBe(leagueSeason);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeasonsDetailsViewModel);
        }

        [Fact]
        public void CreateGet_ShouldReturnLeagueSeasonCreateView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddLeagueSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            var leagueSeason = new LeagueSeason();

            // Act
            var result = await testController.Create(leagueSeason);

            // Assert
            A.CallTo(() => leagueSeasonRepository.AddAsync(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnLeagueSeasonCreateView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            var leagueSeason = new LeagueSeason();

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Create(leagueSeason);

            // Assert
            A.CallTo(() => leagueSeasonRepository.AddAsync(leagueSeason)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualLeagueSeasonId_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int id = 0;
            var leagueSeason = new LeagueSeason
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateLeagueSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
            A.CallTo(() => leagueSeasonRepository.Update(leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => leagueSeasonRepository.Update(A<LeagueSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => leagueSeasonRepository.LeagueSeasonExists(A<int>.Ignored)).Returns(false);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndLeagueSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => leagueSeasonRepository.Update(A<LeagueSeason>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => leagueSeasonRepository.LeagueSeasonExists(A<int>.Ignored)).Returns(true);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                ID = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, leagueSeason));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueSeasonIdAndModelStateIsNotValid_ShouldReturnLeagueSeasonEditView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int id = 1;
            var leagueSeason = new LeagueSeason
            {
                ID = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, leagueSeason);

            // Assert
            A.CallTo(() => leagueSeasonRepository.Update(A<LeagueSeason>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueSeasonFound_ShouldReturnLeagueSeasonDeleteView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();

            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leagueSeason);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteLeagueSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var leagueSeasonsIndexViewModel = A.Fake<ILeagueSeasonsIndexViewModel>();
            var leagueSeasonsDetailsViewModel = A.Fake<ILeagueSeasonsDetailsViewModel>();
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeagueSeasonsController(leagueSeasonsIndexViewModel, leagueSeasonsDetailsViewModel,
                leagueSeasonRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
