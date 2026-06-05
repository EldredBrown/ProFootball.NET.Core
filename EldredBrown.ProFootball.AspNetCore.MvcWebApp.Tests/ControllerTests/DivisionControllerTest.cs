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

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var divisionViewModels = new List<DivisionViewModel>
            {
                new DivisionViewModel { Id = 1 },
                new DivisionViewModel { Id = 2 },
                new DivisionViewModel { Id = 3 },
            };
            A.CallTo(() => fakeDivisionViewModelMapper.MapDivisionToViewModel(A<Division>.Ignored))
                .ReturnsNextFromSequence(divisionViewModels.ToArray());

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division { Id = 1 },
                new Division { Id = 2 },
                new Division { Id = 3 },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            foreach (var division in divisions)
            {
                A.CallTo(() => fakeDivisionViewModelMapper.MapDivisionToViewModel(division))
                    .MustHaveHappenedOnceExactly();
            }
            fakeDivisionIndexViewModel.Divisions.ShouldBe(divisionViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeDivisionIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionDetailsView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var divisionViewModel = new DivisionViewModel { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapDivisionToViewModel(An<Division>.Ignored))
                .Returns(divisionViewModel);

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var division = new Division { };
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionViewModelMapper.MapDivisionToViewModel(division))
                .MustHaveHappenedOnceExactly();
            fakeDivisionDetailsViewModel.Division.ShouldNotBeNull();
            fakeDivisionDetailsViewModel.Division.ShouldBeOfType<DivisionViewModel>();
            fakeDivisionDetailsViewModel.Division.ShouldBe(divisionViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeDivisionDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int? id = 0;
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
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddDivisionToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
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

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division
            {
                Id = 2,
                Name = "Division 4",
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A division with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division
            {
                Id = 4,
                Name = "Division 2",
                LeagueId = 1,
                ConferenceId = 1,
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A division with the same name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyLeagueIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_League_LeagueId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyConferenceIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_Conference_ConferenceId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on ConferenceId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyFirstSeasonIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_FirstSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyLastSeasonIdViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_LastSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var division = new Division { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    FirstSeasonId = 1920
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
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnDivisionCreateView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            var division = new Division { };
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Create(divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeDivisionRepository.AddAsync(division)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionEditView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division { };
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<DivisionViewModel>();
            ((DivisionViewModel)resultModel).Division.ShouldBe(division);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var result = await testController.Edit(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int? id = 0;
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

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 1;
            var division = new Division
            {
                Id = id
            };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
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
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 0;
            var division = new Division
            {
                Id = 1
            };
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndDivisionWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 1;
            var division = new Division
            {
                Id = id
            };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            A.CallTo(() => fakeDivisionRepository.DivisionExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndDivisionWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 1;
            var division = new Division
            {
                Id = id
            };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            A.CallTo(() => fakeDivisionRepository.DivisionExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var func = new Func<Task<IActionResult>>(async () => await testController.Edit(id, divisionViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolation_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 2;
            var division = new Division
            {
                Id = id,
                Name = "Division 3",
                LeagueId = 1,
                ConferenceId = 1,
                FirstSeasonId = 1920
            };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Name");
            testController.ModelState["Name"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. A division with the same name already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyLeagueIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 2;
            var division = new Division { Id = id };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_League_LeagueId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LeagueId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConferenceIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 2;
            var division = new Division { Id = id };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_Conference_ConferenceId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on ConferenceId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyFirstSeasonIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 2;
            var division = new Division { Id = id };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_FirstSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyLastSeasonIdConflict_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 2;
            var division = new Division { Id = id };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
            };
            A.CallTo(() => fakeDivisionRepository.GetDivisionsAsync()).Returns(divisions);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Division_Season_LastSeasonId\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonId.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            int id = 2;
            var division = new Division { Id = id };
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(A<DivisionViewModel>.Ignored))
                .Returns(Task.FromResult(division));

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var divisions = new List<Division>
            {
                new Division
                {
                    Id = 1,
                    Name = "Division 1",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 2,
                    Name = "Division 2",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
                },
                new Division
                {
                    Id = 3,
                    Name = "Division 3",
                    LeagueId = 1,
                    ConferenceId = 1,
                    FirstSeasonId = 1920
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
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnDivisionEditView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("Name", "Please enter a long name.");

            // Act
            int id = 1;
            var division = new Division
            {
                Id = 1
            };
            var divisionViewModel = new DivisionViewModel { Division = division };
            var result = await testController.Edit(id, divisionViewModel);

            // Assert
            A.CallTo(() => fakeDivisionViewModelMapper.MapViewModelToDivision(divisionViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeDivisionRepository.Update(division)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndDivisionFound_ShouldReturnDivisionDeleteView()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();

            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var divisionViewModel = new DivisionViewModel { };
            A.CallTo(() => fakeDivisionViewModelMapper.MapDivisionToViewModel(A<Division>.Ignored))
                .Returns(divisionViewModel);

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = new Division { };
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeDivisionViewModelMapper.MapDivisionToViewModel(division))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<DivisionViewModel>();
            resultModel.ShouldBe(divisionViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            var result = await testController.Delete(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenDivisionNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeDivisionIndexViewModel = A.Fake<IDivisionIndexViewModel>();
            var fakeDivisionDetailsViewModel = A.Fake<IDivisionDetailsViewModel>();
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();

            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            Division? division = null;
            A.CallTo(() => fakeDivisionRepository.GetDivisionAsync(An<int>.Ignored)).Returns(division);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int? id = 0;
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
            var fakeDivisionViewModelMapper = A.Fake<IDivisionViewModelMapper>();
            var fakeDivisionRepository = A.Fake<IDivisionRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new DivisionController(fakeDivisionIndexViewModel, fakeDivisionDetailsViewModel,
                fakeDivisionViewModelMapper, fakeDivisionRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeDivisionRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
