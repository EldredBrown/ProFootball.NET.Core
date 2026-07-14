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
    public class LeagueControllerTest
    {
        [Fact]
        public async Task GetLeagues_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => associationRepository.GetAssociationsAsync()).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetAssociations();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetLeagues_WhenNoExceptionIsCaught_ShouldGetLeagues()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            var associations = new List<Association>();
            A.CallTo(() => associationRepository.GetAssociationsAsync()).Returns(associations);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetAssociations();

            // Assert
            A.CallTo(() => associationRepository.GetAssociationsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<AssociationModel[]>(associations)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<AssociationModel[]>>();
            result.Value.ShouldBe(mapper.Map<AssociationModel[]>(associations));
        }

        [Fact]
        public async Task GetAssociation_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = new Association();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            AssociationModel? associationModel = new AssociationModel();
            A.CallTo(() => mapper.Map<AssociationModel>(A<Association>.Ignored)).Returns(associationModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetAssociation(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetAssociation_WhenAssociationIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = null;
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetAssociation(id);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAssociation_WhenAssociationIsNotNull_ShouldReturnAssociationModelOfDesiredAssociation()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = new Association();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            AssociationModel? associationModel = new AssociationModel();
            A.CallTo(() => mapper.Map<AssociationModel>(A<Association>.Ignored)).Returns(associationModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetAssociation(id);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<AssociationModel>(association)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<AssociationModel>();
        }

        [Fact]
        public async Task PutAssociation_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new AssociationModel();

            // Act
            var result = await testController.PutAssociation(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutAssociation_WhenAssociationIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = null;
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new AssociationModel();

            // Act
            var result = await testController.PutAssociation(id, model);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find association with Id of {id}");
        }

        [Fact]
        public async Task PutLeague_WhenLeagueIsFoundAndSaved_ShouldReturnModelOfLeague()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = new Association();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var returnModel = new AssociationModel();
            A.CallTo(() => mapper.Map<AssociationModel>(association)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new AssociationModel();

            // Act
            var result = await testController.PutAssociation(id, model);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<AssociationModel>(association)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutAssociation_WhenAssociationIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = new Association();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var returnModel = new AssociationModel();
            A.CallTo(() => mapper.Map<AssociationModel>(association)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new AssociationModel();

            // Act
            var result = await testController.PutAssociation(id, model);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, association)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<AssociationModel>(association)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteAssociation_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteAssociation(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteAssociation_WhenAssociationIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = null;
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteAssociation(id);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find association with Id of {id}");
        }

        [Fact]
        public async Task DeleteAssociation_WhenAssociationIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = new Association();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteAssociation(id);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteAssociation_WhenAssociationIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var associationRepository = A.Fake<IAssociationRepository>();
            Association? association = new Association();
            A.CallTo(() => associationRepository.GetAssociationAsync(An<int>.Ignored)).Returns(association);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new AssociationController(associationRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteAssociation(id);

            // Assert
            A.CallTo(() => associationRepository.GetAssociationAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
