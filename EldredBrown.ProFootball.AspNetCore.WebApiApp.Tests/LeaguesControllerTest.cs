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
    public class LeaguesControllerTest
    {
        [Fact]
        public async Task GetLeagues_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => leagueRepository.GetLeaguesAsync()).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetLeagues();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetLeagues_WhenNoExceptionIsCaught_ShouldGetLeagues()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            var leagues = new List<League>();
            A.CallTo(() => leagueRepository.GetLeaguesAsync()).Returns(leagues);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetLeagues();

            // Assert
            A.CallTo(() => leagueRepository.GetLeaguesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueModel[]>(leagues)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<LeagueModel[]>>();
            result.Value.ShouldBe(mapper.Map<LeagueModel[]>(leagues));
        }

        [Fact]
        public async Task GetLeague_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            LeagueModel? leagueModel = new LeagueModel();
            A.CallTo(() => mapper.Map<LeagueModel>(A<League>.Ignored)).Returns(leagueModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetLeague(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetLeague_WhenLeagueIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetLeague(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetLeague_WhenLeagueIsNotNull_ShouldReturnLeagueModelOfDesiredLeague()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            LeagueModel? leagueModel = new LeagueModel();
            A.CallTo(() => mapper.Map<LeagueModel>(A<League>.Ignored)).Returns(leagueModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetLeague(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueModel>(league)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<LeagueModel>();
        }

        [Fact]
        public async Task PutLeague_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueModel();

            // Act
            var result = await testController.PutLeague(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutLeague_WhenLeagueIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueModel();

            // Act
            var result = await testController.PutLeague(id, model);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find league with ID of {id}");
        }

        [Fact]
        public async Task PutLeague_WhenLeagueIsFoundAndSaved_ShouldReturnModelOfLeague()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var returnModel = new LeagueModel();
            A.CallTo(() => mapper.Map<LeagueModel>(league)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueModel();

            // Act
            var result = await testController.PutLeague(id, model);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueModel>(league)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutLeague_WhenLeagueIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var returnModel = new LeagueModel();
            A.CallTo(() => mapper.Map<LeagueModel>(league)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new LeagueModel();

            // Act
            var result = await testController.PutLeague(id, model);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, league)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<LeagueModel>(league)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteLeague_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeague(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteLeague_WhenLeagueIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = null;
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeague(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find league with ID of {id}");
        }

        [Fact]
        public async Task DeleteLeague_WhenLeagueIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeague(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteLeague_WhenLeagueIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var leagueRepository = A.Fake<ILeagueRepository>();
            League? league = new League();
            A.CallTo(() => leagueRepository.GetLeagueAsync(A<int>.Ignored)).Returns(league);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new LeaguesController(leagueRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteLeague(id);

            // Assert
            A.CallTo(() => leagueRepository.GetLeagueAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
