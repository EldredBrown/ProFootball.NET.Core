using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Seasons;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class SeasonsControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnSeasonsIndexView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            seasonsIndexViewModel.Seasons.ShouldBe(seasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(seasonsIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            seasonsDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => seasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndSeasonFound_ShouldReturnSeasonDetailsView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            seasonsDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => seasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            seasonsDetailsViewModel.Season.ShouldBe(season);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(seasonsDetailsViewModel);
        }

        [Fact]
        public void CreateGet_ShouldReturnSeasonCreateView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            var season = new Season();

            // Act
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => seasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnSeasonCreateView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            var season = new Season();

            testController.ModelState.AddModelError("Year", "Please enter a year.");

            // Act
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => seasonRepository.AddAsync(season)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndSeasonFound_ShouldReturnSeasonEditView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualSeasonId_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int id = 0;
            var season = new Season
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, season);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int id = 1;
            var season = new Season
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, season);

            // Assert
            A.CallTo(() => seasonRepository.Update(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => seasonRepository.Update(A<Season>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => seasonRepository.SeasonExists(A<int>.Ignored)).Returns(false);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int id = 1;
            var season = new Season
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, season);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsSeasonIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => seasonRepository.Update(A<Season>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => seasonRepository.SeasonExists(A<int>.Ignored)).Returns(true);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int id = 1;
            var season = new Season
            {
                ID = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, season));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsSeasonIdAndModelStateIsNotValid_ShouldReturnSeasonEditView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int id = 1;
            var season = new Season
            {
                ID = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, season);

            // Assert
            A.CallTo(() => seasonRepository.Update(A<Season>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndSeasonFound_ShouldReturnSeasonDeleteView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();

            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var seasonsIndexViewModel = A.Fake<ISeasonsIndexViewModel>();
            var seasonsDetailsViewModel = A.Fake<ISeasonsDetailsViewModel>();
            var seasonRepository = A.Fake<ISeasonRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonsController(seasonsIndexViewModel, seasonsDetailsViewModel,
                seasonRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => seasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
