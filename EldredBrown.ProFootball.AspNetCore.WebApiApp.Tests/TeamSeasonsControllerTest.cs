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
    public class TeamSeasonsControllerTest
    {
        [Fact]
        public async Task GetTeamSeasons_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsAsync()).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper,
                linkGenerator);

            // Act
            var result = await testController.GetTeamSeasons();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeasons_WhenNoExceptionIsCaught_ShouldGetTeamSeasons()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            var teamSeasons = new List<TeamSeason>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsAsync()).Returns(teamSeasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetTeamSeasons();

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamSeasonModel[]>(teamSeasons)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<TeamSeasonModel[]>>();
            result.Value.ShouldBe(mapper.Map<TeamSeasonModel[]>(teamSeasons));
        }

        [Fact]
        public async Task GetTeamSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            TeamSeasonModel? teamSeasonModel = new TeamSeasonModel();
            A.CallTo(() => mapper.Map<TeamSeasonModel>(A<TeamSeason>.Ignored)).Returns(teamSeasonModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetTeamSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetTeamSeason_WhenTeamSeasonIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetTeamSeason(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetTeamSeason_WhenTeamSeasonIsNotNull_ShouldReturnTeamSeasonModelOfDesiredTeamSeason()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            TeamSeasonModel? teamSeasonModel = new TeamSeasonModel();
            A.CallTo(() => mapper.Map<TeamSeasonModel>(A<TeamSeason>.Ignored)).Returns(teamSeasonModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetTeamSeason(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamSeasonModel>(teamSeason)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<TeamSeasonModel>();
        }

        [Fact]
        public async Task PutTeamSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamSeasonModel();

            // Act
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutTeamSeason_WhenTeamSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamSeasonModel();

            // Act
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find teamSeason with ID of {id}");
        }

        [Fact]
        public async Task PutTeamSeason_WhenTeamSeasonIsFoundAndSaved_ShouldReturnModelOfTeamSeason()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var returnModel = new TeamSeasonModel();
            A.CallTo(() => mapper.Map<TeamSeasonModel>(teamSeason)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamSeasonModel();

            // Act
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamSeasonModel>(teamSeason)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutTeamSeason_WhenTeamSeasonIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var returnModel = new TeamSeasonModel();
            A.CallTo(() => mapper.Map<TeamSeasonModel>(teamSeason)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new TeamSeasonModel();

            // Act
            var result = await testController.PutTeamSeason(id, model);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, teamSeason)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<TeamSeasonModel>(teamSeason)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenTeamSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = null;
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find teamSeason with ID of {id}");
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenTeamSeasonIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteTeamSeason_WhenTeamSeasonIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var teamSeasonRepository = A.Fake<ITeamSeasonRepository>();
            TeamSeason? teamSeason = new TeamSeason();
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(A<int>.Ignored)).Returns(teamSeason);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new TeamSeasonsController(teamSeasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteTeamSeason(id);

            // Assert
            A.CallTo(() => teamSeasonRepository.GetTeamSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
