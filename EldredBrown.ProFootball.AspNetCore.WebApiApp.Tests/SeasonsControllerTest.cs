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
    public class SeasonsControllerTest
    {
        [Fact]
        public async Task GetSeasons_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetSeasons();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetSeasons_WhenNoExceptionIsCaught_ShouldGetSeasons()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            var seasons = new List<Season>();
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).Returns(seasons);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            // Act
            var result = await testController.GetSeasons();

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<SeasonModel[]>(seasons)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<SeasonModel[]>>();
            result.Value.ShouldBe(mapper.Map<SeasonModel[]>(seasons));
        }

        [Fact]
        public async Task GetSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            SeasonModel? seasonModel = new SeasonModel();
            A.CallTo(() => mapper.Map<SeasonModel>(A<Season>.Ignored)).Returns(seasonModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetSeason_WhenSeasonIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetSeason(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetSeason_WhenSeasonIsNotNull_ShouldReturnSeasonModelOfDesiredSeason()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();

            var mapper = A.Fake<IMapper>();
            SeasonModel? seasonModel = new SeasonModel();
            A.CallTo(() => mapper.Map<SeasonModel>(A<Season>.Ignored)).Returns(seasonModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.GetSeason(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<SeasonModel>(season)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<SeasonModel>();
        }

        [Fact]
        public async Task PutSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new SeasonModel();

            // Act
            var result = await testController.PutSeason(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task PutSeason_WhenSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new SeasonModel();

            // Act
            var result = await testController.PutSeason(id, model);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find season with ID of {id}");
        }

        [Fact]
        public async Task PutSeason_WhenSeasonIsFoundAndSaved_ShouldReturnModelOfSeason()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var returnModel = new SeasonModel();
            A.CallTo(() => mapper.Map<SeasonModel>(season)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new SeasonModel();

            // Act
            var result = await testController.PutSeason(id, model);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<SeasonModel>(season)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutSeason_WhenSeasonIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var returnModel = new SeasonModel();
            A.CallTo(() => mapper.Map<SeasonModel>(season)).Returns(returnModel);

            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;
            var model = new SeasonModel();

            // Act
            var result = await testController.PutSeason(id, model);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map(model, season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<SeasonModel>(season)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Throws<Exception>();

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteSeason_WhenSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = null;
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteSeason(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find season with ID of {id}");
        }

        [Fact]
        public async Task DeleteSeason_WhenSeasonIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(1);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteSeason(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteSeason_WhenSeasonIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            var seasonRepository = A.Fake<ISeasonRepository>();
            Season? season = new Season();
            A.CallTo(() => seasonRepository.GetSeasonAsync(A<int>.Ignored)).Returns(season);

            var sharedRepository = A.Fake<ISharedRepository>();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).Returns(0);

            var mapper = A.Fake<IMapper>();
            var linkGenerator = A.Fake<LinkGenerator>();

            var testController = new SeasonsController(seasonRepository, sharedRepository, mapper, linkGenerator);

            int id = 1;

            // Act
            var result = await testController.DeleteSeason(id);

            // Assert
            A.CallTo(() => seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }
    }
}
