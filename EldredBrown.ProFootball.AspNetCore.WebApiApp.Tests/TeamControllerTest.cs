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
    public class TeamControllerTest
    {
        [Fact]
        public async Task GetTeams_WhenNoExceptionIsCaught_ShouldGetLeagues()
        {
            // Arrange
            List<Team> teams = [];
            TeamController testController = SetUp(teams: teams);

            // Act
            var result = await testController.GetTeams();

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamModel[]>(teams)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<TeamModel[]>>();
            result.Value.ShouldBe(testController._mapper.Map<TeamModel[]>(teams));
        }

        [Fact]
        public async Task GetTeams_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamController testController = SetUp(ex: ex);

            // Act
            var result = await testController.GetTeams();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeam_WhenTeamIsNull_ShouldReturnNotFound()
        {
            // Arrange
            Team team = null!;
            TeamController testController = SetUp(team: team);

            // Act
            int id = 1;
            var result = await testController.GetTeam(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetTeam_WhenTeamIsNotNull_ShouldReturnTeamModelOfDesiredTeam()
        {
            // Arrange
            Team team = new();
            TeamModel teamModel = new();
            TeamController testController = SetUp(team: team, teamModel: teamModel);

            // Act
            int id = 1;
            var result = await testController.GetTeam(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamModel>(team)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamModel>();
        }

        [Fact]
        public async Task GetTeam_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            TeamModel teamModel = new();
            var ex = new Exception();
            TeamController testController = SetUp(teamModel: teamModel, ex: ex);

            // Act
            int id = 1;
            var result = await testController.GetTeam(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutTeam_WhenTeamIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Team team = null!;
            TeamController testController = SetUp(team: team);

            // Act
            int id = 1;
            var model = new TeamModel();
            var result = await testController.PutTeam(id, model);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find team with Id of {id}");
        }

        [Fact]
        public async Task PutTeam_WhenTeamIsFoundAndSaved_ShouldReturnModelOfTeam()
        {
            // Arrange
            Team team = new();
            int numOfRecordsUpdated = 1;
            var returnModel = new TeamModel();
            TeamController testController = SetUp(
                team: team, numOfRecordsUpdated: numOfRecordsUpdated,
                teamModel: returnModel
            );

            // Act
            int id = 1;
            var model = new TeamModel();
            var result = await testController.PutTeam(id, model);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamModel>(team)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutTeam_WhenTeamIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            Team team = new();
            int numOfRecordsUpdated = 0;
            var returnModel = new TeamModel();
            TeamController testController = SetUp(
                team: team, numOfRecordsUpdated: numOfRecordsUpdated, teamModel: returnModel
            );

            // Act
            int id = 1;
            var model = new TeamModel();
            var result = await testController.PutTeam(id, model);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<TeamModel>(team)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PutTeam_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var model = new TeamModel();
            var result = await testController.PutTeam(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteTeam_WhenTeamIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Team team = null!;
            TeamController testController = SetUp(team: team);

            // Act
            int id = 1;
            var result = await testController.DeleteTeam(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id))
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find team with Id of {id}");
        }

        [Fact]
        public async Task DeleteTeam_WhenTeamIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            Team team = new();
            int numOfRecordsUpdated = 1;
            TeamController testController = SetUp(
                team: team, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteTeam(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync())
                .MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteTeam_WhenTeamIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            Team team = new();
            int numOfRecordsUpdated = 0;
            TeamController testController = SetUp(
                team: team, numOfRecordsUpdated: numOfRecordsUpdated
            );

            // Act
            int id = 1;
            var result = await testController.DeleteTeam(id);

            // Assert
            A.CallTo(() => testController._teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteTeam_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            TeamController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var result = await testController.DeleteTeam(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static TeamController SetUp(
            List<Team>? teams = null, Team? team = null, int? numOfRecordsUpdated = null,
            TeamModel? teamModel = null, Exception? ex = null
        )
        {
            ITeamRepository fakeTeamRepository = SetUpFakeTeamRepository(teams, team, ex);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(numOfRecordsUpdated);
            IMapper fakeMapper = SetUpFakeMapper(teamModel);
            LinkGenerator fakeLinkGenerator = SetUpFakeLinkGenerator();

            return new TeamController(
                fakeTeamRepository, fakeSharedRepository, fakeMapper, fakeLinkGenerator
            );
        }

        private static ITeamRepository SetUpFakeTeamRepository(
            List<Team>? teams, Team? team, Exception? ex
        )
        {
            var fakeTeamRepository = A.Fake<ITeamRepository>();
            if (ex is null)
            {
                A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Returns(teams);
                A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Returns(team);
            }
            else
            {
                A.CallTo(() => fakeTeamRepository.GetTeamsAsync()).Throws(ex);
                A.CallTo(() => fakeTeamRepository.GetTeamAsync(An<int>.Ignored)).Throws(ex);
            }

            return fakeTeamRepository;
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

        private static IMapper SetUpFakeMapper(TeamModel? teamModel)
        {
            var fakeMapper = A.Fake<IMapper>();
            if (teamModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<TeamModel>(A<Team>.Ignored)).Returns(teamModel);
            }

            return fakeMapper;
        }

        private static LinkGenerator SetUpFakeLinkGenerator()
        {
            return A.Fake<LinkGenerator>();
        }
    }
}
