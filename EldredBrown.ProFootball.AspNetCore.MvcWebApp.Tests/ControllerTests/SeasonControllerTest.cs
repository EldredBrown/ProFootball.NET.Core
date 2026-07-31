using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class SeasonControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnSeasonIndexView()
        {
            // Arrange
            var seasons = new List<Season>();
            SeasonController testController = SetUp(seasons:seasons);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController._seasonIndexViewModel.Seasons.ShouldBe(seasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenYearIsNotNullAndSeasonFound_ShouldReturnSeasonDetailsView()
        {
            // Arrange
            var season = new Season();
            SeasonController testController = SetUp(season: season);

            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            // Act
            int? year = 1920;
            var result = await testController.Details(year);

            // Assert
            testController._seasonDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            testController._seasonDetailsViewModel.Season.ShouldBe(season);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._seasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenYearIsNull_ShouldReturnNotFound()
        {
            // Arrange
            SeasonController testController = SetUp();

            // Act
            int? year = null;
            var result = await testController.Details(year);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            SeasonController testController = SetUp();

            // Act
            int? year = 1920;
            var result = await testController.Details(year);

            // Assert
            testController._seasonDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnSeasonCreateView()
        {
            // Arrange
            SeasonController testController = SetUp();

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndSaveChangesDoesNotThrowDbUpdateException_ShouldAddSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            SeasonController testController = SetUp();

            // Act
            var season = new Season();
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => testController._seasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var ex = new DbUpdateException(
                "DbUpdateException",
                new Exception("Violation of PRIMARY KEY constraint 'PK_Season'.")
            );
            SeasonController testController = SetUp(seasons: seasons, ex: ex);

            // Act
            var season = new Season
            {
                Year = 1921
            };
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => testController._seasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Year");
            testController.ModelState["Year"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A season with the same year already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var ex = new DbUpdateException(
                "DbUpdateException",
                new Exception("Something else.")
            );
            SeasonController testController = SetUp(seasons: seasons, ex: ex);

            // Act
            var season = new Season
            {
                Year = 1923
            };
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => testController._seasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnSeasonCreateView()
        {
            // Arrange
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            var ex = new DbUpdateException(
                "DbUpdateException",
                new Exception("Something else.")
            );
            SeasonController testController = SetUp(seasons: seasons, ex: ex);

            testController.ModelState.AddModelError("Year", "Please enter a year.");

            // Act
            var season = new Season();
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => testController._seasonRepository.AddAsync(season)).MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndSeasonFound_ShouldReturnSeasonDeleteView()
        {
            // Arrange
            var season = new Season();
            SeasonController testController = SetUp(season: season);

            // Act
            int? year = 1920;
            var result = await testController.Delete(year);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            SeasonController testController = SetUp();

            // Act
            int? year = null;
            var result = await testController.Delete(year);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenSeasonNotFound_ShouldReturnNotFound()
        {
            // Arrange
            SeasonController testController = SetUp();

            // Act
            int? year = 1920;
            var result = await testController.Delete(year);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            SeasonController testController = SetUp();

            // Act
            int year = 1920;
            var result = await testController.DeleteConfirmed(year);

            // Assert
            A.CallTo(() => testController._seasonRepository.DeleteAsync(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static SeasonController SetUp(
            IEnumerable<Season>? seasons = null, Season? season = null, DbUpdateException? ex = null
        )
        {
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            ISeasonRepository fakeSeasonRepository = SetUpFakeSeasonRepository(seasons, season);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(ex);

            return new SeasonController(
                fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel, fakeSeasonRepository, fakeSharedRepository
            );
        }

        private static ISeasonRepository SetUpFakeSeasonRepository(IEnumerable<Season>? seasons, Season? season)
        {
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            return fakeSeasonRepository;
        }

        private static ISharedRepository SetUpFakeSharedRepository(DbUpdateException? ex)
        {
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            if (ex != null)
            {
                A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);
            }

            return fakeSharedRepository;
        }
    }
}
