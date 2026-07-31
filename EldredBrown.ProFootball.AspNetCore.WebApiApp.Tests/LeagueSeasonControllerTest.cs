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
    public class LeagueSeasonControllerTest
    {
        [Fact]
        public async Task GetLeagueSeasons_WhenNoExceptionIsCaught_ShouldGetLeagues()
        {
            // Arrange
            List<LeagueSeason> leagueSeasons = [];
            LeagueSeasonController testController = SetUp(leagueSeasons: leagueSeasons);

            // Act
            var result = await testController.GetLeagueSeasons();

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<LeagueSeasonModel[]>(leagueSeasons))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<LeagueSeasonModel[]>>();
            result.Value.ShouldBe(testController._mapper.Map<LeagueSeasonModel[]>(leagueSeasons));
        }

        [Fact]
        public async Task GetLeagueSeasons_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            LeagueSeasonController testController = SetUp(ex: ex);

            // Act
            var result = await testController.GetLeagueSeasons();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetLeagueSeason_WhenLeagueSeasonIsNull_ShouldReturnNotFound()
        {
            // Arrange
            LeagueSeason leagueSeason = null!;
            LeagueSeasonController testController = SetUp(leagueSeason: leagueSeason);

            // Act
            int id = 1;
            var result = await testController.GetLeagueSeason(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetLeagueSeason_WhenLeagueSeasonIsNotNull_ShouldReturnLeagueSeasonModelOfDesiredLeagueSeason()
        {
            // Arrange
            LeagueSeason leagueSeason = new();
            LeagueSeasonModel leagueSeasonModel = new();
            LeagueSeasonController testController = SetUp(
                leagueSeason: leagueSeason, leagueSeasonModel: leagueSeasonModel
            );

            // Act
            int id = 1;
            var result = await testController.GetLeagueSeason(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<LeagueSeasonModel>(leagueSeason))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<LeagueSeasonModel>();
        }

        [Fact]
        public async Task GetLeagueSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            LeagueSeasonModel leagueSeasonModel = new();
            var ex = new Exception();
            LeagueSeasonController testController = SetUp(leagueSeasonModel: leagueSeasonModel, ex: ex);

            // Act
            int id = 1;
            var result = await testController.GetLeagueSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutLeagueSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            LeagueSeason leagueSeason = null!;
            LeagueSeasonController testController = SetUp(leagueSeason: leagueSeason);

            // Act
            int id = 1;
            var model = new LeagueSeasonModel();
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find leagueSeason with Id of {id}");
        }

        [Fact]
        public async Task PutLeagueSeason_WhenLeagueIsFoundAndSaved_ShouldReturnModelOfLeagueSeason()
        {
            // Arrange
            LeagueSeason leagueSeason = new();
            int numOfRecordsUpdated = 1;
            var returnModel = new LeagueSeasonModel();
            LeagueSeasonController testController = SetUp(
                leagueSeason: leagueSeason, numOfRecordsUpdated: numOfRecordsUpdated,
                leagueSeasonModel: returnModel
            );

            // Act
            int id = 1;
            var model = new LeagueSeasonModel();
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<LeagueSeasonModel>(leagueSeason))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutLeagueSeason_WhenLeagueSeasonIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            LeagueSeason leagueSeason = new();
            int numOfRecordsUpdated = 0;
            var returnModel = new LeagueSeasonModel();
            LeagueSeasonController testController = SetUp(
                leagueSeason: leagueSeason, numOfRecordsUpdated: numOfRecordsUpdated, leagueSeasonModel: returnModel
            );

            // Act
            int id = 1;
            var model = new LeagueSeasonModel();
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, leagueSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<LeagueSeasonModel>(leagueSeason))
                .MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PutLeagueSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            LeagueSeasonController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var model = new LeagueSeasonModel();
            var result = await testController.PutLeagueSeason(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenLeagueSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            LeagueSeason leagueSeason = null!;
            LeagueSeasonController testController = SetUp(leagueSeason: leagueSeason);

            // Act
            int id = 1;
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find leagueSeason with Id of {id}");
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenLeagueSeasonIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            LeagueSeason leagueSeason = new();
            int numOfRecordsUpdated = 1;
            LeagueSeasonController testController = SetUp(
                leagueSeason: leagueSeason, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenLeagueSeasonIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            LeagueSeason leagueSeason = new();
            int numOfRecordsUpdated = 0;
            LeagueSeasonController testController = SetUp(
                leagueSeason: leagueSeason, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            A.CallTo(() => testController._leagueSeasonRepository.GetLeagueSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteLeagueSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            LeagueSeasonController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var result = await testController.DeleteLeagueSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static LeagueSeasonController SetUp(
            List<LeagueSeason>? leagueSeasons = null, LeagueSeason? leagueSeason = null,
            int? numOfRecordsUpdated = null, LeagueSeasonModel? leagueSeasonModel = null, Exception? ex = null
        )
        {
            ILeagueSeasonRepository fakeLeagueSeasonRepository = 
                SetUpFakeLeagueSeasonRepository(leagueSeasons, leagueSeason, ex);
            ISharedRepository fakeSharedRepository = 
                SetUpFakeSharedRepository(numOfRecordsUpdated);
            IMapper fakeMapper = 
                SetUpFakeMapper(leagueSeasonModel);
            LinkGenerator fakeLinkGenerator = 
                SetUpFakeLinkGenerator();

            return new LeagueSeasonController(
                fakeLeagueSeasonRepository, fakeSharedRepository, fakeMapper, fakeLinkGenerator
            );
        }

        private static ILeagueSeasonRepository SetUpFakeLeagueSeasonRepository(
            List<LeagueSeason>? leagueSeasons, LeagueSeason? leagueSeason, Exception? ex
        )
        {
            var fakeLeagueSeasonRepository = A.Fake<ILeagueSeasonRepository>();
            if (ex is null)
            {
                A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Returns(leagueSeasons);
                A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Returns(leagueSeason);
            }
            else
            {
                A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonsAsync()).Throws(ex);
                A.CallTo(() => fakeLeagueSeasonRepository.GetLeagueSeasonAsync(An<int>.Ignored)).Throws(ex);
            }

            return fakeLeagueSeasonRepository;
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

        private static IMapper SetUpFakeMapper(LeagueSeasonModel? leagueSeasonModel)
        {
            var fakeMapper = A.Fake<IMapper>();
            if (leagueSeasonModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<LeagueSeasonModel>(A<LeagueSeason>.Ignored)).Returns(leagueSeasonModel);
            }

            return fakeMapper;
        }

        private static LinkGenerator SetUpFakeLinkGenerator()
        {
            return A.Fake<LinkGenerator>();
        }
    }
}
