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
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class SeasonStandingsControllerTest
    {
        [Fact]
        public async Task GetSeasonStandings_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Throws<Exception>();

            var mapper = A.Fake<IMapper>();

            var testController = new SeasonStandingsController(fakeSeasonStandingsRepository, mapper);

            int seasonYear = 1920;
            int leagueId = 1;

            // Act
            var result = await testController.GetSeasonStandings(seasonYear, leagueId);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task GetSeasonStandings_WhenNoExceptionIsCaught_ShouldGetSeasonStandings()
        {
            // Arrange
            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            var seasonStandings = new List<StandingsTeamSeason>();
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                .Returns(seasonStandings);

            var mapper = A.Fake<IMapper>();

            var testController = new SeasonStandingsController(fakeSeasonStandingsRepository, mapper);

            int seasonYear = 1920;
            int leagueId = 1;

            // Act
            var result = await testController.GetSeasonStandings(seasonYear, leagueId);

            // Assert
            A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(seasonYear, leagueId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => mapper.Map<StandingsTeamSeasonModel[]>(seasonStandings)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<StandingsTeamSeasonModel[]>>();
            result.Value.ShouldBe(mapper.Map<StandingsTeamSeasonModel[]>(seasonStandings));
        }
    }
}
