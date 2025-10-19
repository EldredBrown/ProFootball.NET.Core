using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Leagues;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class LeaguesControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnLeaguesIndexView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>();
            A.CallTo(() => leagueRepository.GetLeaguesAsync()).Returns(leagues);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => leagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            leaguesIndexViewModel.Leagues.ShouldBe(leagues);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leaguesIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndLeagueFound_ShouldReturnLeagueDetailsView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            leaguesDetailsViewModel.League.ShouldBe(league);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(leaguesDetailsViewModel);
        }

        [Fact]
        public void CreateGet_ShouldReturnLeagueCreateView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddLeagueToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            var league = new League();

            // Act
            var result = await testController.Create(league);

            // Assert
            A.CallTo(() => leagueRepository.AddAsync(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnLeagueCreateView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            var league = new League();

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Create(league);

            // Assert
            A.CallTo(() => leagueRepository.AddAsync(league)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(league);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndLeagueFound_ShouldReturnLeagueEditView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(league);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualLeagueId_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int id = 0;
            var league = new League
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, league);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateLeagueInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int id = 1;
            var league = new League
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, league);

            // Assert
            A.CallTo(() => leagueRepository.Update(league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndLeagueWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => leagueRepository.Update(A<League>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => leagueRepository.LeagueExists(A<int>.Ignored)).Returns(false);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int id = 1;
            var league = new League
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, league);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndLeagueWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => leagueRepository.Update(A<League>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => leagueRepository.LeagueExists(A<int>.Ignored)).Returns(true);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int id = 1;
            var league = new League
            {
                ID = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, league));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsLeagueIdAndModelStateIsNotValid_ShouldReturnLeagueEditView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int id = 1;
            var league = new League
            {
                ID = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, league);

            // Assert
            A.CallTo(() => leagueRepository.Update(A<League>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(league);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndLeagueFound_ShouldReturnLeagueDeleteView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();

            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(league);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteLeagueFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var leaguesIndexViewModel = A.Fake<ILeaguesIndexViewModel>();
            var leaguesDetailsViewModel = A.Fake<ILeaguesDetailsViewModel>();
            var leagueRepository = A.Fake<ILeagueRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new LeaguesController(leaguesIndexViewModel, leaguesDetailsViewModel,
                leagueRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => leagueRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
