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
        public async Task Details_WhenIdIsNotNullAndSeasonFound_ShouldReturnSeasonDetailsView()
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
            int? id = 1920;
            var result = await testController.Details(id);

            // Assert
            fakeSeasonDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            fakeSeasonDetailsViewModel.Season.ShouldBe(season);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeSeasonDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int? id = null;
            var result = await testController.Details(id);

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
            int? id = 1920;
            var result = await testController.Details(id);

            // Assert
            fakeSeasonDetailsViewModel.Title.ShouldBe<string>("Season");
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
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
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            var season = new Season { Id = 1921 };
            var result = await testController.Create(season);

            // Assert
            A.CallTo(() => fakeSeasonRepository.AddAsync(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A season with the same id already exists.");
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
                new() { Id = 1920 },
                new() { Id = 1921 },
                new() { Id = 1922 },
            };
            A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns(seasons);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            var season = new Season { Id = 1923 };
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
        public async Task EditGet_WhenIdIsNotNullAndSeasonFound_ShouldReturnSeasonEditView()
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
            int? id = 1920;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
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
            var result = await testController.Edit(year);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndSeasonNotFound_ShouldReturnNotFound()
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
            int? id = 1920;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsSeasonIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateSeasonInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int id = 1920;
            var season = new Season { Id = id };
            var result = await testController.Edit(id, season);

            // Assert
            A.CallTo(() => fakeSeasonRepository.Update(season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualSeasonYear_ShouldReturnNotFound()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int id = 1920;
            var season = new Season { Id = 1921 };
            var result = await testController.Edit(id, season);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndSeasonWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.SeasonExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int id = 1921;
            var season = new Season { Id = id };
            var result = await testController.Edit(id, season);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndSeasonWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();

            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => fakeSeasonRepository.SeasonExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int id = 1921;
            var season = new Season { Id = id };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, season));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaught_ShouldAddModelErrorToModelStateAndReturnViewForSeason()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);

            // Act
            int id = 1920;
            var season = new Season { Id = id };
            var result = await testController.Edit(id, season);

            // Assert
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(season);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnSeasonEditView()
        {
            // Arrange
            var fakeSeasonIndexViewModel = A.Fake<ISeasonIndexViewModel>();
            var fakeSeasonDetailsViewModel = A.Fake<ISeasonDetailsViewModel>();
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new SeasonController(fakeSeasonIndexViewModel, fakeSeasonDetailsViewModel,
                fakeSeasonRepository, fakeSharedRepository);
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            int id = 1920;
            var season = new Season { Id = id };
            var result = await testController.Edit(id, season);

            // Assert
            A.CallTo(() => fakeSeasonRepository.Update(A<Season>.Ignored)).MustNotHaveHappened();
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
            int? id = 1920;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
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
            int? id = null;
            var result = await testController.Delete(id);

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
            int? id = 1920;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(id.Value)).MustHaveHappenedOnceExactly();
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
            int id = 1920;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeSeasonRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
