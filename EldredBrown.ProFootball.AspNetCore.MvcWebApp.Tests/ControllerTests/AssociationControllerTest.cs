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
            var associationViewModels = new List<AssociationViewModel>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            var associations = new List<Association>
            {
                new() { Id = 1 },
                new() { Id = 2 },
                new() { Id = 3 },
            };
            AssociationController testController = SetUp(
                associationViewModels: associationViewModels, associations: associations
            );

            // Act
            var result = await testController.Index();

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            foreach (var association in associations)
            {
                A.CallTo(() => testController._associationViewModelMapper.MapAssociationToViewModel(association))
                    .MustHaveHappenedOnceExactly();
            }
            testController._associationIndexViewModel.Associations.ShouldBe(associationViewModels);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._associationIndexViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNotNullAndAssociationFound_ShouldReturnAssociationDetailsView()
        {
            // Arrange
            var associationViewModel = new AssociationViewModel();
            var association = new Association();
            AssociationController testController = SetUp(
                associationViewModel: associationViewModel, association: association
            );

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id.Value)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationViewModelMapper.MapAssociationToViewModel(association))
                .MustHaveHappenedOnceExactly();
            testController._associationDetailsViewModel.Association.ShouldNotBeNull();
            testController._associationDetailsViewModel.Association.ShouldBeOfType<AssociationViewModel>();
            testController._associationDetailsViewModel.Association.ShouldBe(associationViewModel);
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(testController._associationDetailsViewModel);
        }

        [Fact]
        public async Task Details_WhenIdIsNull_ShouldReturnNotFound()
        {
            // Arrange
            AssociationController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Details(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Details_WhenAssociationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            AssociationController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Details(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public void CreateGet_ShouldReturnAssociationCreateView()
        {
            // Arrange
            AssociationController testController = SetUp();

            // Act
            var result = testController.Create();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsValidAndNoExceptionCaught_ShouldAddAssociationToDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var association = new Association();
            AssociationController testController = SetUp(association: association);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForPrimaryKeyViolation_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var association = new Association
            {
                Id = 2,
                LongName = "Association 4",
                ShortName = "A4",
                FirstSeasonYear = 1920
            };

            var ex = new DbUpdateException();

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("Id");
            testController.ModelState["Id"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An association with the same Id already exists.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForLongNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'long_name'."));

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered LongName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForShortNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'short_name'."));

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered ShortName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolationOnLongName_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var association = new Association
            {
                Id = 4,
                LongName = "Association 4",
                ShortName = "A2",
                FirstSeasonYear = 1920
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_LongName'."));

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint LongName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForUniqueKeyViolationOnShortName_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var association = new Association
            {
                Id = 4,
                LongName = "Association 4",
                ShortName = "A2",
                FirstSeasonYear = 1920
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_ShortName'."));

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint ShortName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyViolationOnFirstSeasonYear_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var association = new Association();

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_FirstSeasonYear\".")
            );

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForForeignKeyViolationOnLastSeasonYear_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var association = new Association();

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The INSERT statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_LastSeasonYear\".")
            );

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenSaveChangesThrowsDbUpdateExceptionForSomethingElse_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var association = new Association();

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );

            AssociationController testController = SetUp(associations: associations, association: association, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task CreatePost_WhenModelStateIsNotValid_ShouldReturnAssociationCreateView()
        {
            // Arrange
            AssociationController testController = SetUp();
            testController.ModelState.AddModelError("LongName", "Please enter a long name.");

            // Act
            var association = new Association { };
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Create(associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => testController._associationRepository.AddAsync(association))
                .MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditGet_WhenIdIsNotNullAndAssociationFound_ShouldReturnAssociationEditView()
        {
            // Arrange
            Association? association = new();

            AssociationController testController = SetUp(association: association);

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id.Value))
                .MustHaveHappenedOnceExactly();
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
            AssociationController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Edit(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditGet_WhenAssociationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            AssociationController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Edit(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenIdEqualsAssociationIdAndModelStateIsValidAndNoExceptionCaught_ShouldUpdateAssociationInDataStoreAndRedirectToIndexView()
        {
            // Arrange
            var association = new Association
            {
                Id = 1
            };

            AssociationController testController = SetUp(association: association);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        [Fact]
        public async Task EditPost_WhenIdDoesNotEqualAssociationId_ShouldReturnNotFound()
        {
            // Arrange
            AssociationController testController = SetUp();

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
            var association = new Association
            {
                Id = 1
            };

            var ex = new DbUpdateConcurrencyException();

            AssociationController testController = SetUp(association: association, associationExists: false, ex: ex);

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateConcurrencyExceptionIsCaughtAndAssociationWithIdExists_ShouldRethrowException()
        {
            // Arrange
            var association = new Association
            {
                Id = 1
            };

            var ex = new DbUpdateConcurrencyException();

            AssociationController testController = SetUp(association: association, associationExists: true, ex: ex);

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

            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'long_name'."));

            AssociationController testController = SetUp(
                associations: associations, association: association, associationExists: true, ex: ex
            );

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("LongName");
            testController.ModelState["LongName"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered LongName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenSaveChangesThrowsDbUpdateExceptionForShortNameTooLong_ShouldHandleExceptionAndReturnSeasonCreateView()
        {
            // Arrange
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

            var ex = new DbUpdateException(
                message: string.Empty,
                innerException: new Exception("String or binary data would be truncated in table 'ProFootballDb_Proposed.dbo.Association', column 'short_name'."));

            AssociationController testController = SetUp(
                associations: associations, association: association, associationExists: true, ex: ex
            );

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey("ShortName");
            testController.ModelState["ShortName"]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. The entered ShortName is too long.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolationOnLongName_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
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

            var association = new Association
            {
                Id = 2,
                LongName = "Association 2",
                ShortName = "A4",
                FirstSeasonYear = 1920
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_LongName'."));

            AssociationController testController = SetUp(
                associations: associations, association: association, associationExists: true, ex: ex
            );

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint LongName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForUniqueKeyViolationOnShortName_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
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

            var association = new Association
            {
                Id = 2,
                LongName = "Association 4",
                ShortName = "A2",
                FirstSeasonYear = 1920
            };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Violation of UNIQUE KEY constraint 'UQ_Association_ShortName'."));

            AssociationController testController = SetUp(
                associations: associations, association: association, associationExists: true, ex: ex
            );

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(association.Id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Violation of UNIQUE KEY constraint ShortName.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConflictOnFirstSeasonYear_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
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

            int id = 2;
            var association = new Association { Id = id };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_FirstSeasonYear\".")
            );

            AssociationController testController = SetUp(
                associations: associations, association: association, associationExists: true, ex: ex
            );

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on FirstSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForForeignKeyConflictOnLastSeasonYear_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
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

            int id = 2;
            var association = new Association { Id = id };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("The UPDATE statement conflicted with the FOREIGN KEY constraint \"FK_Association_Season_LastSeasonYear\".")
            );

            AssociationController testController = SetUp(
                associations: associations, association: association, associationExists: true, ex: ex
            );

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. Conflict with a FOREIGN KEY constraint on LastSeasonYear.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenDbUpdateExceptionIsCaughtForSomethingElse_ShouldHandleExceptionAndReturnViewForSeason()
        {
            // Arrange
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

            int id = 2;
            var association = new Association { Id = id };

            var ex = new DbUpdateException(
                message: "DbUpdateException",
                innerException: new Exception("Exception")
            );

            AssociationController testController = SetUp(
                associations: associations, association: association, associationExists: true, ex: ex
            );

            // Act
            var associationViewModel = new AssociationViewModel { Association = association };
            var result = await testController.Edit(id, associationViewModel);

            // Assert
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            testController.ModelState.IsValid.ShouldBeFalse();
            testController.ModelState.ErrorCount.ShouldBe(1);
            testController.ModelState.ShouldContainKey(string.Empty);
            testController.ModelState[string.Empty]?.Errors[0].ErrorMessage
                .ShouldBe("Unable to save changes. An unexpected error occurred.");
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task EditPost_WhenModelStateIsNotValid_ShouldReturnAssociationEditView()
        {
            // Arrange
            AssociationController testController = SetUp();
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
            A.CallTo(() => testController._associationViewModelMapper.MapViewModelToAssociation(associationViewModel))
                .MustNotHaveHappened();
            A.CallTo(() => testController._associationRepository.Update(association))
                .MustNotHaveHappened();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustNotHaveHappened();
            result.ShouldBeOfType<ViewResult>();
            ((ViewResult)result).Model.ShouldBe(associationViewModel);
        }

        [Fact]
        public async Task Delete_WhenIdIsNotNullAndAssociationFound_ShouldReturnAssociationDeleteView()
        {
            // Arrange
            var associationViewModel = new AssociationViewModel { };
            Association? association = new();

            AssociationController testController = SetUp(
                associationViewModel: associationViewModel, association: association
            );

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._associationViewModelMapper.MapAssociationToViewModel(association))
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
            AssociationController testController = SetUp();

            // Act
            int? id = null;
            var result = await testController.Delete(id);

            // Assert
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenAssociationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            AssociationController testController = SetUp();

            // Act
            int? id = 0;
            var result = await testController.Delete(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id.Value))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ShouldDeleteAssociationFromDataStoreAndRedirectToIndexView()
        {
            // Arrange
            AssociationController testController = SetUp();

            // Act
            int id = 1;
            var result = await testController.DeleteConfirmed(id);

            // Assert
            A.CallTo(() => testController._associationRepository.DeleteAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<RedirectToActionResult>();
            ((RedirectToActionResult)result).ActionName.ShouldBe<string>(nameof(testController.Index));
        }

        private static AssociationController SetUp(
            List<AssociationViewModel>? associationViewModels = null, AssociationViewModel? associationViewModel = null,
            List<Association>? associations = null, Association? association = null, bool? associationExists = null,
            Exception? ex = null
        )
        {
            var fakeAssociationIndexViewModel = A.Fake<IAssociationIndexViewModel>();
            var fakeAssociationDetailsViewModel = A.Fake<IAssociationDetailsViewModel>();
            IAssociationViewModelMapper fakeAssociationViewModelMapper = 
                SetUpFakeAssociationViewModelMapper(associationViewModels, associationViewModel, association);
            IAssociationRepository fakeAssociationRepository =
                SetUpFakeAssociationRepository(associations, association, associationExists);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(ex);

            return new AssociationController(
                fakeAssociationIndexViewModel, fakeAssociationDetailsViewModel, fakeAssociationViewModelMapper,
                fakeAssociationRepository, fakeSharedRepository
            );
        }

        private static IAssociationViewModelMapper SetUpFakeAssociationViewModelMapper(List<AssociationViewModel>? associationViewModels, AssociationViewModel? associationViewModel, Association? association)
        {
            var fakeAssociationViewModelMapper = A.Fake<IAssociationViewModelMapper>();
            if (associationViewModels is not null)
            {
                A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(An<Association>.Ignored))
                    .ReturnsNextFromSequence([.. associationViewModels]);
            }
            if (associationViewModel is not null)
            {
                A.CallTo(() => fakeAssociationViewModelMapper.MapAssociationToViewModel(An<Association>.Ignored))
                    .Returns(associationViewModel);
            }
            A.CallTo(() => fakeAssociationViewModelMapper.MapViewModelToAssociation(An<AssociationViewModel>.Ignored))
                .Returns(association);

            return fakeAssociationViewModelMapper;
        }

        private static IAssociationRepository SetUpFakeAssociationRepository(List<Association>? associations, Association? association, bool? associationExists)
        {
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);
            A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);
            if (associationExists.HasValue)
            {
                A.CallTo(() => fakeAssociationRepository.AssociationExistsAsync(An<int>.Ignored))
                    .Returns(associationExists.Value);
            }

            return fakeAssociationRepository;
        }

        private static ISharedRepository SetUpFakeSharedRepository(Exception? ex)
        {
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            if (ex is not null)
            {
                A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Throws(ex);
            }

            return fakeSharedRepository;
        }
    }
}
