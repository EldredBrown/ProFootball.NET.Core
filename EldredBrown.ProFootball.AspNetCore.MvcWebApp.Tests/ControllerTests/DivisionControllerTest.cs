using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class DivisionControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnDivisionIndexView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            fakeDivisionIndexViewModel.Divisions.ShouldBe(divisions);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeDivisionIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionDetailsView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division();
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            fakeDivisionDetailsViewModel.Division.ShouldBe(division);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeDivisionDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnDivisionCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValid_ShouldAddDivisionToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            var division = new Division();

            // Act
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var division = new Division
            {
                Id = 2,
                Name = "Division 4",
                LeagueName = "L4",
                ConferenceName = "C4",
                FirstSeasonYear = 1920
            };
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A division with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var division = new Division
            {
                Id = 4,
                Name = "Division 2",
                LeagueName = "L4",
                ConferenceName = "C4",
                FirstSeasonYear = 1920
            };
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A division with the same name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyLeagueNameViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_League_LeagueName\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var division = new Division();
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LeagueName");
            testController.ModelState["LeagueName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyConferenceNameViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_Conference_ConferenceName\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var division = new Division();
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ConferenceName");
            testController.ModelState["ConferenceName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on ConferenceName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyFirstSeasonYearViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_FirstSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var division = new Division();
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("FirstSeasonYear");
            testController.ModelState["FirstSeasonYear"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyLastSeasonYearViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_LastSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var division = new Division();
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LastSeasonYear");
            testController.ModelState["LastSeasonYear"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            var division = new Division();
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnDivisionCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            var division = new Division();

            testController.ModelState.AddModelError("Name", "Please enter a name.");

            // Act
            var result = await testController.Create(division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionEditView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division();
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsDivisionIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateDivisionInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int id = 1;
            var division = new Division
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualDivisionId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int id = 0;
            var division = new Division
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndDivisionWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            A.CallTo(() => fakeDivisionRepository.DivisionExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int id = 1;
            var division = new Division
            {
                Id = 1
            };

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndDivisionWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            A.CallTo(() => fakeDivisionRepository.DivisionExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int id = 1;
            var division = new Division
            {
                Id = 1
            };

            // Act
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, division));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var division = new Division
            {
                Id = 2,
                Name = "Division 3",
                LeagueName = "L",
                ConferenceName = "C",
                FirstSeasonYear = 1920
            };
            var result = await testController.Edit(id, division);

            // Assert
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A division with the same name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyLeagueNameConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_League_LeagueName\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var division = new Division { Id = 2 };
            var result = await testController.Edit(id, division);

            // Assert
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LeagueName");
            testController.ModelState["LeagueName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConferenceNameConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_Conference_ConferenceName\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var division = new Division { Id = 2 };
            var result = await testController.Edit(id, division);

            // Assert
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ConferenceName");
            testController.ModelState["ConferenceName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on ConferenceName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyFirstSeasonYearConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_FirstSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var division = new Division { Id = 2 };
            var result = await testController.Edit(id, division);

            // Assert
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("FirstSeasonYear");
            testController.ModelState["FirstSeasonYear"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyLastSeasonYearConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_LastSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var division = new Division { Id = 2 };
            var result = await testController.Edit(id, division);

            // Assert
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LastSeasonYear");
            testController.ModelState["LastSeasonYear"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueName = "L",
                    ConferenceName = "C",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 2;
            var division = new Division { Id = 2 };
            var result = await testController.Edit(id, division);

            // Assert
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnDivisionEditView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int id = 1;
            var division = new Division
            {
                Id = 1
            };
            testController.ModelState.AddModelError("Name", "Please enter a name.");

            // Act
            var result = await testController.Edit(id, division);

            // Assert
            A.CallTo(() => fakeDivisionRepository.Update(A<Division>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionDeleteView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division();
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(division);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = null;

            // Act
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int? id = 0;

            // Act
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteDivisionFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionRepository, fakeSharedRepository);

            int id = 1;

            // Act
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
