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
    public class TeamSeasonControllerTest
    {
        [Fact]
        public async Task GetTeamSeasons_WhenNoExceptionIsCaught_ShouldGetLeagues()
        {
            // Arrange
            List<TeamSeason> teamSeasons = [];
            TeamSeasonController testController = SetUp(teamSeasons: teamSeasons);

            // Act
            var result = await testController.GetTeamSeasons();

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonsAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamSeasonModel[]>(teamSeasons))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<TeamSeasonModel[]>>();
            result.Value.ShouldBe(testController._mapper.Map<TeamSeasonModel[]>(teamSeasons));
        }

        [Fact]
        public async Task GetTeamSeasons_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamSeasonController testController = SetUp(ex: ex);

            // Act
            var result = await testController.GetTeamSeasons();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeason_WhenTeamSeasonIsNull_ShouldReturnNotFound()
        {
            // Arrange
            TeamSeason teamSeason = null!;
            TeamSeasonController testController = SetUp(teamSeason: teamSeason);

            // Act
            int id = 1;
            var result = await testController.GetTeamSeason(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetTeamSeason_WhenTeamSeasonIsNotNull_ShouldReturnTeamSeasonModelOfDesiredTeamSeason()
        {
            // Arrange
            TeamSeason teamSeason = new();
            TeamSeasonModel teamSeasonModel = new();
            TeamSeasonController testController = SetUp(
                teamSeason: teamSeason, teamSeasonModel: teamSeasonModel
            );

            // Act
            int id = 1;
            var result = await testController.GetTeamSeason(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamSeasonModel>(teamSeason))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamSeasonModel>();
        }

        [Fact]
        public async Task GetTeamSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            TeamSeasonModel teamSeasonModel = new();
            var ex = new Exception();
            TeamSeasonController testController = SetUp(teamSeasonModel: teamSeasonModel, ex: ex);

            // Act
            int id = 1;
            var result = await testController.GetTeamSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutTeamSeason_WhenTeamSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            TeamSeason teamSeason = null!;
            TeamSeasonController testController = SetUp(teamSeason: teamSeason);

            // Act
            int id = 1;
            var model = new TeamSeasonModel();
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find teamSeason with Id of {id}");
        }

        [Fact]
        public async Task PutTeamSeason_WhenTeamSeasonIsFoundAndSaved_ShouldReturnModelOfLeague()
        {
            // Arrange
            TeamSeason teamSeason = new();
            int numOfRecordsUpdated = 1;
            var returnModel = new TeamSeasonModel();
            TeamSeasonController testController = SetUp(
                teamSeason: teamSeason, numOfRecordsUpdated: numOfRecordsUpdated,
                teamSeasonModel: returnModel
            );

            // Act
            int id = 1;
            var model = new TeamSeasonModel();
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamSeasonModel>(teamSeason))
                .MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutTeamSeason_WhenTeamSeasonIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            TeamSeason teamSeason = new();
            int numOfRecordsUpdated = 0;
            var returnModel = new TeamSeasonModel();
            TeamSeasonController testController = SetUp(
                teamSeason: teamSeason, numOfRecordsUpdated: numOfRecordsUpdated, teamSeasonModel: returnModel
            );

            // Act
            int id = 1;
            var model = new TeamSeasonModel();
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, teamSeason))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamSeasonModel>(teamSeason))
                .MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PutTeamSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamSeasonController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var model = new TeamSeasonModel();
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenTeamSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            TeamSeason teamSeason = null!;
            TeamSeasonController testController = SetUp(teamSeason: teamSeason);

            // Act
            int id = 1;
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find teamSeason with Id of {id}");
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenTeamSeasonIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            TeamSeason teamSeason = new();
            int numOfRecordsUpdated = 1;
            TeamSeasonController testController = SetUp(
                teamSeason: teamSeason, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenTeamSeasonIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            TeamSeason teamSeason = new();
            int numOfRecordsUpdated = 0;
            TeamSeasonController testController = SetUp(
                teamSeason: teamSeason, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            A.CallTo(() => testController._teamSeasonRepository.GetTeamSeasonAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamSeasonController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static TeamSeasonController SetUp(
            List<TeamSeason>? teamSeasons = null, TeamSeason? teamSeason = null,
            int? numOfRecordsUpdated = null, TeamSeasonModel? teamSeasonModel = null, Exception? ex = null
        )
        {
            ITeamSeasonRepository fakeTeamSeasonRepository =
                SetUpFakeTeamSeasonRepository(teamSeasons, teamSeason, ex);
            ISharedRepository fakeSharedRepository =
                SetUpFakeSharedRepository(numOfRecordsUpdated);
            IMapper fakeMapper =
                SetUpFakeMapper(teamSeasonModel);
            LinkGenerator fakeLinkGenerator =
                SetUpFakeLinkGenerator();

            return new TeamSeasonController(
                fakeTeamSeasonRepository, fakeSharedRepository, fakeMapper, fakeLinkGenerator
            );
        }

        private static ITeamSeasonRepository SetUpFakeTeamSeasonRepository(
            List<TeamSeason>? teamSeasons, TeamSeason? teamSeason, Exception? ex
        )
        {
            var fakeTeamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            if (ex is null)
            {
                A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);
                A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Returns(teamSeason);
            }
            else
            {
                A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonsAsync()).Throws(ex);
                A.CallTo(() => fakeTeamSeasonRepository.GetTeamSeasonAsync(An<int>.Ignored)).Throws(ex);
            }

            return fakeTeamSeasonRepository;
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

        private static IMapper SetUpFakeMapper(TeamSeasonModel? teamSeasonModel)
        {
            var fakeMapper = A.Fake<IMapper>();
            if (teamSeasonModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<TeamSeasonModel>(A<TeamSeason>.Ignored)).Returns(teamSeasonModel);
            }

            return fakeMapper;
        }

        private static LinkGenerator SetUpFakeLinkGenerator()
        {
            return A.Fake<LinkGenerator>();
        }
    }
}
