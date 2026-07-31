using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using AutoMapper;
using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class AssociationControllerTest
    {
        [Fact]
        public async Task GetAssociations_WhenNoExceptionIsCaught_ShouldGetLeagues()
        {
            // Arrange
            List<Association> associations = [];
            AssociationController testController = SetUp(associations: associations);

            // Act
            var result = await testController.GetAssociations();

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<AssociationModel[]>(associations)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<AssociationModel[]>>();
            result.Value.ShouldBe(testController._mapper.Map<AssociationModel[]>(associations));
        }

        [Fact]
        public async Task GetAssociations_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            AssociationController testController = SetUp(ex: ex);

            // Act
            var result = await testController.GetAssociations();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetAssociation_WhenAssociationIsNull_ShouldReturnNotFound()
        {
            // Arrange
            Association association = null!;
            AssociationController testController = SetUp(association: association);

            // Act
            int id = 1;
            var result = await testController.GetAssociation(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAssociation_WhenAssociationIsNotNull_ShouldReturnAssociationModelOfDesiredAssociation()
        {
            // Arrange
            Association association = new();
            AssociationModel associationModel = new();
            AssociationController testController = SetUp(association: association, associationModel: associationModel);

            // Act
            int id = 1;
            var result = await testController.GetAssociation(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<AssociationModel>(association)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<AssociationModel>();
        }

        [Fact]
        public async Task GetAssociation_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            AssociationModel associationModel = new();
            var ex = new Exception();
            AssociationController testController = SetUp(associationModel: associationModel, ex: ex);

            // Act
            int id = 1;
            var result = await testController.GetAssociation(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutAssociation_WhenAssociationIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Association association = null!;
            AssociationController testController = SetUp(association: association);

            // Act
            int id = 1;
            var model = new AssociationModel();
            var result = await testController.PutAssociation(id, model);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find association with Id of {id}");
        }

        [Fact]
        public async Task PutAssociation_WhenLeagueIsFoundAndSaved_ShouldReturnModelOfAssociation()
        {
            // Arrange
            Association association = new();
            int numOfRecordsUpdated = 1;
            var returnModel = new AssociationModel();
            AssociationController testController = SetUp(
                association: association, numOfRecordsUpdated: numOfRecordsUpdated,
                associationModel: returnModel
            );

            // Act
            int id = 1;
            var model = new AssociationModel();
            var result = await testController.PutAssociation(id, model);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<AssociationModel>(association)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutAssociation_WhenAssociationIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            Association association = new();
            int numOfRecordsUpdated = 0;
            var returnModel = new AssociationModel();
            AssociationController testController = SetUp(
                association: association, numOfRecordsUpdated: numOfRecordsUpdated, associationModel: returnModel
            );

            // Act
            int id = 1;
            var model = new AssociationModel();
            var result = await testController.PutAssociation(id, model);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<AssociationModel>(association)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PutAssociation_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            AssociationController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var model = new AssociationModel();
            var result = await testController.PutAssociation(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteAssociation_WhenAssociationIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Association association = null!;
            AssociationController testController = SetUp(association: association);

            // Act
            int id = 1;
            var result = await testController.DeleteAssociation(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find association with Id of {id}");
        }

        [Fact]
        public async Task DeleteAssociation_WhenAssociationIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            Association association = new();
            int numOfRecordsUpdated = 1;
            AssociationController testController = SetUp(
                association: association, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteAssociation(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteAssociation_WhenAssociationIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            Association association = new();
            int numOfRecordsUpdated = 0;
            AssociationController testController = SetUp(
                association: association, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteAssociation(id);

            // Assert
            A.CallTo(() => testController._associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteAssociation_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            AssociationController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var result = await testController.DeleteAssociation(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static AssociationController SetUp(
            List<Association>? associations = null, Association? association = null, int? numOfRecordsUpdated = null,
            AssociationModel? associationModel = null, Exception? ex = null
        )
        {
            IAssociationRepository fakeAssociationRepository = SetUpFakeAssociationRepository(associations, association, ex);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(numOfRecordsUpdated);
            IMapper fakeMapper = SetUpFakeMapper(associationModel);
            LinkGenerator fakeLinkGenerator = SetUpFakeLinkGenerator();

            return new AssociationController(
                fakeAssociationRepository, fakeSharedRepository, fakeMapper, fakeLinkGenerator
            );
        }

        private static IAssociationRepository SetUpFakeAssociationRepository(
            List<Association>? associations, Association? association, Exception? ex
        )
        {
            var fakeAssociationRepository = A.Fake<IAssociationRepository>();
            if (ex is null)
            {
                A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Returns(associations);
                A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);
            }
            else
            {
                A.CallTo(() => fakeAssociationRepository.GetAssociationsAsync()).Throws(ex);
                A.CallTo(() => fakeAssociationRepository.GetAssociationAsync(An<int>.Ignored)).Throws(ex);
            }

            return fakeAssociationRepository;
        }

        private static ISharedRepository SetUpFakeSharedRepository(int? numOfRecordsUpdated)
        {
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            if (numOfRecordsUpdated.HasValue)
            {
                A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Returns(numOfRecordsUpdated.Value);
            }

            return fakeSharedRepository;
        }

        private static IMapper SetUpFakeMapper(AssociationModel? associationModel)
        {
            var fakeMapper = A.Fake<IMapper>();
            if (associationModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<AssociationModel>(A<Association>.Ignored)).Returns(associationModel);
            }

            return fakeMapper;
        }

        private static LinkGenerator SetUpFakeLinkGenerator()
        {
            return A.Fake<LinkGenerator>();
        }
    }
}
