using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Divisions;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class DivisionsControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnDivisionsIndexView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>();
            A.CallTo(() => divisionRepository.GetDivisionsAsync()).Returns(divisions);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => divisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            divisionsIndexViewModel.Divisions.ShouldBe(divisions);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionsIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => divisionRepository.GetDivisionAsync(A<int>.Ignored)).Returns(division);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => divisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionDetailsView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division();
            A.CallTo(() => divisionRepository.GetDivisionAsync(A<int>.Ignored)).Returns(division);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => divisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            divisionsDetailsViewModel.Division.ShouldBe(division);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionsDetailsViewModel);
        }

        [Fact]
        public void CreateGet_ShouldReturnDivisionCreateView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddDivisionToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            var division = new Division();

            // Act
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => divisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnDivisionCreateView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            var division = new Division();

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => divisionRepository.AddAsync(division)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => divisionRepository.GetDivisionAsync(A<int>.Ignored)).Returns(division);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => divisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionEditView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division();
            A.CallTo(() => divisionRepository.GetDivisionAsync(A<int>.Ignored)).Returns(division);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => divisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualDivisionId_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int id = 0;
            var division = new Division
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsDivisionIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsNotCaught_ShouldUpdateDivisionInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int id = 1;
            var division = new Division
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            A.CallTo(() => divisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsDivisionIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndDivisionWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            A.CallTo(() => divisionRepository.Update(A<Division>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => divisionRepository.DivisionExists(A<int>.Ignored)).Returns(false);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int id = 1;
            var division = new Division
            {
                ID = 1
            };

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsDivisionIdAndModelStateIsValidAndDbUpdateConcurrencyExceptionIsCaughtAndDivisionWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            A.CallTo(() => divisionRepository.Update(A<Division>.Ignored)).Throws<DbUpdateConcurrencyException>();
            A.CallTo(() => divisionRepository.DivisionExists(A<int>.Ignored)).Returns(true);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int id = 1;
            var division = new Division
            {
                ID = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, division));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsDivisionIdAndModelStateIsNotValid_ShouldReturnDivisionEditView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int id = 1;
            var division = new Division
            {
                ID = 1
            };
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            A.CallTo(() => divisionRepository.Update(A<Division>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => divisionRepository.GetDivisionAsync(A<int>.Ignored)).Returns(division);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => divisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionDeleteView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();

            var divisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division();
            A.CallTo(() => divisionRepository.GetDivisionAsync(A<int>.Ignored)).Returns(division);

            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => divisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteDivisionFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var divisionsIndexViewModel = A.Fake<IDivisionsIndexViewModel>();
            var divisionsDetailsViewModel = A.Fake<IDivisionsDetailsViewModel>();
            var divisionRepository = A.Fake<IDivisionRepository>();
            var sharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionsController(divisionsIndexViewModel, divisionsDetailsViewModel,
                divisionRepository, sharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => divisionRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
