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
    public class TeamsControllerTest
    {
        [Fact]
        public async Task GetTeams_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => teamRepository.GetTeamsAsync()).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetTeams();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeams_WhenNoExceptionIsCaught_ShouldGetTeams()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            var teams = new List<Team>();
            A.CallTo(() => teamRepository.GetTeamsAsync()).Returns(teams);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetTeams();

            // Assert
            A.CallTo(() => teamRepository.GetTeamsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamModel[]>(teams)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<TeamModel[]>>();
            result.Value.ShouldBe(mapper.Map<TeamModel[]>(teams));
        }

        [Fact]
        public async Task GetTeam_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            TeamModel? teamModel = new TeamModel();
            A.CallTo(() => mapper.Map<TeamModel>(A<Team>.Ignored)).Returns(teamModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetTeam(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeam_WhenTeamIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetTeam(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetTeam_WhenTeamIsNotNull_ShouldReturnTeamModelOfDesiredTeam()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            TeamModel? teamModel = new TeamModel();
            A.CallTo(() => mapper.Map<TeamModel>(A<Team>.Ignored)).Returns(teamModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetTeam(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamModel>(team)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamModel>();
        }

        [Fact]
        public async Task PutTeam_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamModel();

            // Act
            var result = await testController.PutTeam(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutTeam_WhenTeamIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamModel();

            // Act
            var result = await testController.PutTeam(id, model);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find team with ID of {id}");
        }

        [Fact]
        public async Task PutTeam_WhenTeamIsFoundAndSaved_ShouldReturnModelOfTeam()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var returnModel = new TeamModel();
            A.CallTo(() => mapper.Map<TeamModel>(team)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamModel();

            // Act
            var result = await testController.PutTeam(id, model);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamModel>(team)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutTeam_WhenTeamIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var returnModel = new TeamModel();
            A.CallTo(() => mapper.Map<TeamModel>(team)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamModel();

            // Act
            var result = await testController.PutTeam(id, model);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, team)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamModel>(team)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteTeam_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeam(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteTeam_WhenTeamIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = null;
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeam(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find team with ID of {id}");
        }

        [Fact]
        public async Task DeleteTeam_WhenTeamIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeam(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteTeam_WhenTeamIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var teamRepository = A.Fake<ITeamRepository>();
            Team? team = new Team();
            A.CallTo(() => teamRepository.GetTeamAsync(A<int>.Ignored)).Returns(team);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamsController(teamRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeam(id);

            // Assert
            A.CallTo(() => teamRepository.GetTeamAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
