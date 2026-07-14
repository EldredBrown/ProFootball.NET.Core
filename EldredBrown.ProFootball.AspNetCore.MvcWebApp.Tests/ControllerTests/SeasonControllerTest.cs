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
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            fakeSeasonIndexViewModel.Seasons.ShouldBe(seasons);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenYearIsNotNullAndSeasonFound_ShouldReturnSeasonDetailsView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int? year = 1920;
            var result = await testController.Details(year);

            // Assert
            fakeSeasonDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            fakeSeasonDetailsViewModel.Season.ShouldBe(season);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenYearIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

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
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int? year = 1920;
            var result = await testController.Details(year);

            // Assert
            fakeSeasonDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnSeasonCreateView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndSaveChangesDoesNotThrowDbUpdateException_ShouldAddSeasonToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            var season = new Season();
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => fakeSeasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            var season = new Season { Year = 1921 };
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => fakeSeasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Year");
            testController.ModelState["Year"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A season with the same year already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>
            {
                new() { Year = 1920 },
                new() { Year = 1921 },
                new() { Year = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            var season = new Season { Year = 1923 };
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => fakeSeasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnSeasonCreateView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("Year", "Please enter a year.");

            // Act
            var season = new Season();
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => fakeSeasonRepository.AddAsync(season)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndSeasonFound_ShouldReturnSeasonDeleteView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int? year = 1920;
            var result = await testController.Delete(year);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

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
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int? year = 1920;
            var result = await testController.Delete(year);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(year.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteSeasonFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int year = 1920;
            var result = await testController.DeleteConfirmed(year);

            // Assert
            A.CallTo(() => fakeSeasonRepository.DeleteAsync(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
