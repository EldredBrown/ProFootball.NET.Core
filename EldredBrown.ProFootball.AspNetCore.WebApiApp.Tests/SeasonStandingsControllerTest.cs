using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class SeasonStandingsControllerTest
    {
        [Fact]
        public async Task GetSeasonStandings_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandingsAsync(A<int>.Ignored)).Throws<Exception>();

            var mapper = A.Fake<IMapper>();

            var testController = new SeasonStandingsController(seasonStandingsRepository, mapper);

            int seasonYear = 1920;

            // Act
            var result = await testController.GetSeasonStandings(seasonYear);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetSeasonStandings_WhenNoExceptionIsCaught_ShouldGetSeasonStandings()
        {
            // Arrange
            var seasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<SeasonTeamStanding>();
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandingsAsync(A<int>.Ignored)).Returns(seasonStandings);

            var mapper = A.Fake<IMapper>();

            var testController = new SeasonStandingsController(seasonStandingsRepository, mapper);

            int seasonYear = 1920;

            // Act
            var result = await testController.GetSeasonStandings(seasonYear);

            // Assert
            A.CallTo(() => seasonStandingsRepository.GetSeasonStandingsAsync(seasonYear))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<SeasonTeamStandingModel[]>(seasonStandings)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<SeasonTeamStandingModel[]>>();
            result.Value.ShouldBe(mapper.Map<SeasonTeamStandingModel[]>(seasonStandings));
        }
    }
}
