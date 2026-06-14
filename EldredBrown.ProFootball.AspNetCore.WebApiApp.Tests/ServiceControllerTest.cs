using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FakeItEasy;
using Shouldly;
using Xunit;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class ServiceControllerTest
    {
        [Fact]
        public async Task RunWeeklyUpdate_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            A.CallTo(() => weeklyUpdateService.RunWeeklyUpdate(An<int>.Ignored, An<int>.Ignored)).Throws<Exception>();

            var testController = new ServiceController(weeklyUpdateService);

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = await testController.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result).Value.ShouldBe("Database failure");
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenNoExceptionIsCaught_ShouldReturnOkResult()
        {
            // Arrange
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();

            var testController = new ServiceController(weeklyUpdateService);

            var leagueId = 1;
            var seasonId = 1920;

            // Act
            var result = await testController.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => weeklyUpdateService.RunWeeklyUpdate(leagueId, seasonId)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<OkResult>();
        }
    }
}
