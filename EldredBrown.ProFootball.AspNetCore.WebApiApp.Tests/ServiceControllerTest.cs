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
        public async Task RunWeeklyUpdate_WhenNoExceptionIsCaught_ShouldReturnOkResult()
        {
            // Arrange
            ServiceController testController = SetUp();

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            var result = await testController.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            A.CallTo(() => testController._weeklyUpdateService.RunWeeklyUpdate(leagueId, seasonId))
                .MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<OkResult>();
        }

        [Fact]
        public async Task RunWeeklyUpdate_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var ex = new Exception();
            ServiceController testController = SetUp(ex: ex);

            // Act
            var leagueId = 1;
            var seasonId = 1920;
            var result = await testController.RunWeeklyUpdate(leagueId, seasonId);

            // Assert
            result.ShouldBeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result).Value.ShouldBe("Database failure");
        }

        private static ServiceController SetUp(Exception? ex = null)
        {
            IWeeklyUpdateService fakeWeeklyUpdateService = SetUpFakeWeeklyUpdateService(ex);

            return new ServiceController(fakeWeeklyUpdateService);
        }

        private static IWeeklyUpdateService SetUpFakeWeeklyUpdateService(Exception? ex)
        {
            var fakeWeeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            if (ex is not null)
            {
                A.CallTo(() => fakeWeeklyUpdateService.RunWeeklyUpdate(An<int>.Ignored, An<int>.Ignored)).Throws(ex);
            }

            return fakeWeeklyUpdateService;
        }
    }
}
