using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    public class SeasonStandingsControllerTest
    {
        [Fact]
        public async Task GetSeasonStandings_WhenNoExceptionIsCaught_ShouldGetSeasonStandings()
        {
            // Arrange
            var seasonStandings = new List<StandingsTeamSeason>();
            SeasonStandingsController testController = SetUp(seasonStandings: seasonStandings);

            // Act
            int seasonYear = 1920;
            int leagueId = 1;
            var result = await testController.GetSeasonStandings(seasonYear, leagueId);

            // Assert
            A.CallTo(() => testController._seasonStandingsRepository.GetSeasonStandingsAsync(seasonYear, leagueId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => testController._mapper.Map<StandingsTeamSeasonModel[]>(seasonStandings))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<ActionResult<StandingsTeamSeasonModel[]>>();
            result.Value.ShouldBe(testController._mapper.Map<StandingsTeamSeasonModel[]>(seasonStandings));
        }

        [Fact]
        public async Task GetSeasonStandings_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            SeasonStandingsController testController = SetUp(ex: ex);

            // Act
            int seasonYear = 1920;
            int leagueId = 1;
            var result = await testController.GetSeasonStandings(seasonYear, leagueId);

            // Assert
            result.Result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result.Result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result.Result).Value.ShouldBe("Database failure");
        }

        private static SeasonStandingsController SetUp(List<StandingsTeamSeason>? seasonStandings = null, Exception? ex = null)
        {
            ISeasonStandingsRepository fakeSeasonStandingsRepository = SetUpFakeSeasonStandingsRepository(seasonStandings, ex);
            IMapper fakeMapper = SetUpFakeMapper();

            return new SeasonStandingsController(fakeSeasonStandingsRepository, fakeMapper);
        }

        private static ISeasonStandingsRepository SetUpFakeSeasonStandingsRepository(List<StandingsTeamSeason>? seasonStandings, Exception? ex)
        {
            var fakeSeasonStandingsRepository = A.Fake<ISeasonStandingsRepository>();
            if (ex is null)
            {
                A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                    .Returns(seasonStandings);
            }
            else
            {
                A.CallTo(() => fakeSeasonStandingsRepository.GetSeasonStandingsAsync(An<int>.Ignored, An<int>.Ignored))
                    .Throws(ex);
            }

            return fakeSeasonStandingsRepository;
        }

        private static IMapper SetUpFakeMapper()
        {
            return A.Fake<IMapper>();
        }
    }
}
