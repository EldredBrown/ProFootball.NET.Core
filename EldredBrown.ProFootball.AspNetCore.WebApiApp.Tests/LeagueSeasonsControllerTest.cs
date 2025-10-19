using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class LeagueSeasonsControllerTest
    {
        [Fact]
        public async Task GetLeagueSeasons_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonsAsync()).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper,
                linkGenerator);

            // Act
            var result = await testController.GetLeagueSeasons();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetLeagueSeasons_WhenNoExceptionIsCaught_ShouldGetLeagueSeasons()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            var leagueSeasons = new List<LeagueSeason>();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetLeagueSeasons();

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueSeasonModel[]>(leagueSeasons)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<LeagueSeasonModel[]>>();
            result.Value.ShouldBe(mapper.Map<LeagueSeasonModel[]>(leagueSeasons));
        }

        [Fact]
        public async Task GetLeagueSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            LeagueSeasonModel? leagueSeasonModel = new LeagueSeasonModel();
            A.CallTo(() => mapper.Map<LeagueSeasonModel>(A<LeagueSeason>.Ignored)).Returns(leagueSeasonModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetLeagueSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetLeagueSeason_WhenLeagueSeasonIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetLeagueSeason(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetLeagueSeason_WhenLeagueSeasonIsNotNull_ShouldReturnLeagueSeasonModelOfDesiredLeagueSeason()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            LeagueSeasonModel? leagueSeasonModel = new LeagueSeasonModel();
            A.CallTo(() => mapper.Map<LeagueSeasonModel>(A<LeagueSeason>.Ignored)).Returns(leagueSeasonModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetLeagueSeason(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueSeasonModel>(leagueSeason)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<LeagueSeasonModel>();
        }

        [Fact]
        public async Task PutLeagueSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueSeasonModel();

            // Act
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutLeagueSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueSeasonModel();

            // Act
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find leagueSeason with ID of {id}");
        }

        [Fact]
        public async Task PutLeagueSeason_WhenLeagueSeasonIsFoundAndSaved_ShouldReturnModelOfLeagueSeason()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var returnModel = new LeagueSeasonModel();
            A.CallTo(() => mapper.Map<LeagueSeasonModel>(leagueSeason)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueSeasonModel();

            // Act
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueSeasonModel>(leagueSeason)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutLeagueSeason_WhenLeagueSeasonIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var returnModel = new LeagueSeasonModel();
            A.CallTo(() => mapper.Map<LeagueSeasonModel>(leagueSeason)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueSeasonModel();

            // Act
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, leagueSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueSeasonModel>(leagueSeason)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = null;
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find leagueSeason with ID of {id}");
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenLeagueSeasonIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenLeagueSeasonIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var leagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            LeagueSeason? leagueSeason = new LeagueSeason();
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(A<int>.Ignored)).Returns(leagueSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeagueSeasonsController(leagueSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            A.CallTo(() => leagueSeasonRepository.GetLeagueSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
