using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class AssociationControllerTest
    {
        [Fact]
        public async Task Index_ShouldReturnAssociationIndexView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var associationViewModels = new List<AssociationViewModel>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(An<Association>.Ignored))
                .ReturnsNextFromSequence(associationViewModels.ToArray());

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            foreach (var association in associations)
            {
                A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(association))
                    .MustHaveHappenedOnceExactly();
            }
            fakeAssociationIndexViewModel.Associations.ShouldBe(associationViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeAssociationIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndAssociationFound_ShouldReturnAssociationDetailsView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var associationViewModel = new AssociationViewModel { };
            A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(An<Association>.Ignored))
                .Returns(associationViewModel);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var association = new Association { };
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(association))
                .MustHaveHappenedOnceExactly();
            fakeAssociationDetailsViewModel.Association.ShouldNotBeNull();
            fakeAssociationDetailsViewModel.Association.ShouldBeOfType<AssociationViewModel>();
            fakeAssociationDetailsViewModel.Association.ShouldBe(associationViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(fakeAssociationDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var result = await testController.Details(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenAssociationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            Association? association = null;
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnAssociationCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddAssociationToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association { };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 2,
                LongName = "Association 4",
                ShortName = "A4",
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateException>();

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An association with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForLongNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var longName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                longName.Append('Z');
            }
            var association = new Association
            {
                Id = 4,
                LongName = longName.ToString(),
                ShortName = "A4",
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'long_name'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered LongName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForShortNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var shortName = new StringBuilder();
            for (int i = 0; i <= 5; i++)
            {
                shortName.Append('Z');
            }
            var association = new Association
            {
                Id = 4,
                LongName = "Association 4",
                ShortName = shortName.ToString(),
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'short_name'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered ShortName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolationOnLongName_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 4,
                LongName = "Association 4",
                ShortName = "A2",
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_LongName'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint LongName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolationOnShortName_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 4,
                LongName = "Association 4",
                ShortName = "A2",
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_ShortName'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint ShortName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyViolationOnFirstSeasonYear_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association { };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_FirstSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyViolationOnLastSeasonYear_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association { };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_LastSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association { };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnAssociationCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var association = new Association { };
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeAssociationRepository.AddAsync(association)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndAssociationFound_ShouldReturnAssociationEditView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            Association? association = new();
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<AssociationViewModel>();
            ((AssociationViewModel)resultModel).Association.ShouldBe(association);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var result = await testController.Edit(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenAssociationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            Association? association = null;
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsAssociationIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateAssociationInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 1
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualAssociationId_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var association = new Association
            {
                Id = 1
            };
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(0, associationViewModel);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndAssociationWithIdDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 1
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => fakeAssociationRepository.AssociationExistsAsync(An<int>.Ignored)).Returns(false);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndAssociationWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 1
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => fakeAssociationRepository.AssociationExistsAsync(An<int>.Ignored)).Returns(true);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws<DbUpdateConcurrencyException>();

            var testController = new AssociationController(fakeAssociationIndexViewModel,
                fakeAssociationDetailsViewModel, fakeAssociationViewModelMapper, fakeAssociationRepository,
                fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var func = new Func<Task<IActionResult>>(
                async () => await testController.Edit(association.Id, associationViewModel));

            // Assert
            await func.ShouldThrowAsync<DbUpdateConcurrencyException>();
        }

        [Fact]
        public async Task EditPost_WhenSaveChangesThrowsDbUpdateExceptionForLongNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var longName = new StringBuilder();
            for (int i = 0; i <= 100; i++)
            {
                longName.Append('Z');
            }
            var association = new Association
            {
                Id = 4,
                LongName = longName.ToString(),
                ShortName = "A4",
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'long_name'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel,
                fakeAssociationDetailsViewModel, fakeAssociationViewModelMapper, fakeAssociationRepository,
                fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered LongName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenSaveChangesThrowsDbUpdateExceptionForShortNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var shortName = new StringBuilder();
            for (int i = 0; i <= 5; i++)
            {
                shortName.Append('Z');
            }
            var association = new Association
            {
                Id = 4,
                LongName = "Association 4",
                ShortName = shortName.ToString(),
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 2",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'short_name'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, 
                fakeAssociationDetailsViewModel, fakeAssociationViewModelMapper, fakeAssociationRepository,
                fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered ShortName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolationOnLongName_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 2,
                LongName = "Association 2",
                ShortName = "A4",
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 3",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_LongName'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint LongName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolationOnShortName_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var association = new Association
            {
                Id = 2,
                LongName = "Association 4",
                ShortName = "A2",
                FirstSeasonYear = 1920
            };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    LongName = "Association 1",
                    ShortName = "A1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    LongName = "Association 3",
                    ShortName = "A2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    LongName = "Association 3",
                    ShortName = "A3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_ShortName'."));
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint ShortName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConflictOnFirstSeasonYear_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            int id = 2;
            var association = new Association { Id = id };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ShortName = "A1",
                    LongName = "Association 1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    ShortName = "A2",
                    LongName = "Association 2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    ShortName = "A3",
                    LongName = "Association 3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_FirstSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConflictOnLastSeasonYear_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            int id = 2;
            var association = new Association { Id = id };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ShortName = "A1",
                    LongName = "Association 1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    ShortName = "A2",
                    LongName = "Association 2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    ShortName = "A3",
                    LongName = "Association 3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_LastSeasonYear\".")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            int id = 2;
            var association = new Association { Id = id };
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>
            {
                new()
                {
                    Id = 1,
                    ShortName = "A1",
                    LongName = "Association 1",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 2,
                    ShortName = "A2",
                    LongName = "Association 2",
                    FirstSeasonYear = 1920
                },
                new()
                {
                    Id = 3,
                    ShortName = "A3",
                    LongName = "Association 3",
                    FirstSeasonYear = 1920
                },
            };
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);

            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty].Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnAssociationEditView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            int id = 1;
            var association = new Association
            {
                Id = 1
            };
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(id, associationViewModel);

            // Assert
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => fakeAssociationRepository.Update(association)).MustNotHaveHappened();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndAssociationFound_ShouldReturnAssociationDeleteView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();

            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var associationViewModel = new AssociationViewModel { };
            A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(An<Association>.Ignored))
                .Returns(associationViewModel);

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            Association? association = new();
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(association))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ViewResult>();
            var resultModel = ((ViewResult)result).Model;
            resultModel.ShouldNotBeNull();
            resultModel.ShouldBeOfType<AssociationViewModel>();
            resultModel.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            var result = await testController.Delete(null);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenAssociationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();

            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            Association? association = null;
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(id.Value)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteAssociationFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            var testController = new AssociationController(fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel,
                fakeAssociationViewModelMapper, fakeAssociationRepository, fakeSharedRepository);

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => fakeAssociationRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }
    }
}
