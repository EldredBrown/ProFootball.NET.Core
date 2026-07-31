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
    public class SeasonControllerTest
    {
        [Fact]
        public async Task GetSeasons_WhenNoExceptionIsCaught_ShouldGetSeasons()
        {
            // Arrange
            var seasons = new List<Season>();
            SeasonController testController = SetUp(seasons: seasons);

            // Act
            var result = await testController.GetSeasons();

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonsAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<SeasonModel[]>(seasons)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<SeasonModel[]>>();
            result.Value.ShouldBe(testController._mapper.Map<SeasonModel[]>(seasons));
        }

        [Fact]
        public async Task GetSeasons_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            SeasonController testController = SetUp(ex: ex);

            // Act
            var result = await testController.GetSeasons();

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetSeason_WhenSeasonIsNull_ShouldReturnNotFound()
        {
            // Arrange
            Season season = null!;
            SeasonController testController = SetUp(season: season);

            // Act
            int id = 1;
            var result = await testController.GetSeason(id);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetSeason_WhenSeasonIsNotNull_ShouldReturnSeasonModelOfDesiredSeason()
        {
            // Arrange
            Season season = new();
            SeasonModel? seasonModel = new();
            SeasonController testController = SetUp(season: season, seasonModel: seasonModel);

            // Act
            int id = 1;
            var result = await testController.GetSeason(id);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<SeasonModel>(season)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBeOfType<SeasonModel>();
        }

        [Fact]
        public async Task GetSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var seasonModel = new SeasonModel();
            var ex = new Exception();
            SeasonController testController = SetUp(seasonModel: seasonModel, ex: ex);

            // Act
            int id = 1;
            var result = await testController.GetSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Theory(Skip = "Will explore how to set up fake link generator.")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task PostSeason_WhenLocationIsNullEmptyOrWhiteSpace_ShouldReturnBadRequest(string? location)
        {
            // Arrange
            SeasonController testController = SetUp(location: location);

            // Act
            var model = new SeasonModel();
            var result = await testController.PostSeason(model);

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PutSeason_WhenSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Season season = null!;
            SeasonController testController = SetUp(season: season);

            // Act
            int year = 1919;
            var model = new SeasonModel();
            var result = await testController.PutSeason(year, model);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(year)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find season with Year of {year}");
        }

        [Fact]
        public async Task PutSeason_WhenSeasonIsFoundAndSaved_ShouldReturnModelOfSeason()
        {
            // Arrange
            Season season = new();
            int numRecordsChanged = 1;
            SeasonModel returnModel = new();
            SeasonController testController = SetUp(
                season: season, numRecordsChanged: numRecordsChanged, seasonModel: returnModel
            );

            // Act
            int id = 1;
            var model = new SeasonModel();
            var result = await testController.PutSeason(id, model);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<SeasonModel>(season)).MustHaveHappenedOnceExactly();
            result.Value.ShouldBe(returnModel);
        }

        [Fact]
        public async Task PutSeason_WhenSeasonIsFoundAndNotSaved_ShouldReturnBadRequestResult()
        {
            // Arrange
            Season season = new();
            int numRecordsChanged = 0;
            var returnModel = new SeasonModel();
            SeasonController testController = SetUp(season: season, numRecordsChanged: numRecordsChanged, seasonModel: returnModel);

            // Act
            int id = 1;
            var model = new SeasonModel();
            var result = await testController.PutSeason(id, model);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map(model, season)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<SeasonModel>(season)).MustNotHaveHappened();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PutSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            SeasonController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var model = new SeasonModel();
            var result = await testController.PutSeason(id, model);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task DeleteSeason_WhenSeasonIsNotFound_ShouldReturnNotFoundResult()
        {
            // Arrange
            Season season = null!;
            SeasonController testController = SetUp(season: season);

            // Act
            int year = 1919;
            var result = await testController.DeleteSeason(year);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(year)).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
            ((NotFoundObjectResult)result.Result).Value.ShouldBe($"Could not find season with Year of {year}");
        }

        [Fact]
        public async Task DeleteSeason_WhenSeasonIsFoundAndDeleted_ShouldReturnOk()
        {
            // Arrange
            Season season = new();
            int numRecordsChanged = 1;
            SeasonController testController = SetUp(season: season, numRecordsChanged: numRecordsChanged);

            // Act
            int id = 1;
            var result = await testController.DeleteSeason(id);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteSeason_WhenSeasonIsFoundAndNotDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            Season season = new();
            SeasonController testController = SetUp(season: season);

            // Act
            int id = 1;
            var result = await testController.DeleteSeason(id);

            // Assert
            A.CallTo(() => testController._seasonRepository.GetSeasonAsync(id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._sharedRepository.SaveChangesAsync()).MustHaveHappenedOnceExactly();
            result.Result.ShouldBeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task DeleteSeason_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            SeasonController testController = SetUp(ex: ex);

            // Act
            int id = 1;
            var result = await testController.DeleteSeason(id);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static SeasonController SetUp(
            List<Season>? seasons = null, Season? season = null,
            int? numRecordsChanged = null, SeasonModel? seasonModel = null, string? location = null,
            Exception? ex = null
        )
        {
            ISeasonRepository fakeSeasonRepository = SetUpFakeSeasonRepository(seasons, season, ex);
            ISharedRepository fakeSharedRepository = SetUpFakeSharedRepository(numRecordsChanged);
            IMapper fakeMapper = SetUpFakeMapper(seasonModel);
            LinkGenerator fakeLinkGenerator = SetUpFakeLinkGenerator(location);

            return new SeasonController(fakeSeasonRepository, fakeSharedRepository, fakeMapper, fakeLinkGenerator);
        }

        private static ISeasonRepository SetUpFakeSeasonRepository(List<Season>? seasons, Season? season, Exception? ex)
        {
            var fakeSeasonRepository = A.Fake<ISeasonRepository>();
            if (ex is null)
            {
                if (seasons is not null)
                {
                    A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Returns([.. seasons]);
                }
                A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Returns(season);
            }
            else
            {
                A.CallTo(() => fakeSeasonRepository.GetSeasonsAsync()).Throws(ex);
                A.CallTo(() => fakeSeasonRepository.GetSeasonAsync(An<int>.Ignored)).Throws(ex);
            }

            return fakeSeasonRepository;
        }

        private static ISharedRepository SetUpFakeSharedRepository(int? numRecordsChanged)
        {
            var fakeSharedRepository = A.Fake<ISharedRepository>();
            if (numRecordsChanged.HasValue)
            {
                A.CallTo(() => fakeSharedRepository.SaveChangesAsync()).Returns(numRecordsChanged.Value);
            }

            return fakeSharedRepository;
        }

        private static IMapper SetUpFakeMapper(SeasonModel? seasonModel)
        {
            var fakeMapper = A.Fake<IMapper>();
            if (seasonModel is not null)
            {
                A.CallTo(() => fakeMapper.Map<SeasonModel>(A<Season>.Ignored)).Returns(seasonModel);
            }

            return fakeMapper;
        }

        private static LinkGenerator SetUpFakeLinkGenerator(string? location)
        {
            var fakeLinkGenerator = A.Fake<LinkGenerator>();
            // TODO - 2026.08.01 - Figure out how to intercept GetPathByAction.
            //
            //A.CallTo(() => fakeLinkGenerator.GetPathByAction("GetSeason", "Seasons", new { year = -1 }))
            //    .Returns(location);

            return fakeLinkGenerator;
        }
    }
}
