using System;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers;
using EldredBrown.ProFootball.NETCore.Services;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Tests
{
    public class ServicesControllerTest
    {
        [Fact]
        public async Task RunWeeklyUpdate_WhenExceptionIsCaught_ShouldReturnInternalServerError()
        {
            // Arrange
            var weeklyUpdateService = A.Fake<IWeeklyUpdateService>();
            A.CallTo(() => weeklyUpdateService.RunWeeklyUpdate(A<int>.Ignored)).Throws<Exception>();

            var testController = new ServicesController(weeklyUpdateService);

            int year = 1920;

            // Act
            var result = await testController.RunWeeklyUpdate(year);

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

            var testController = new ServicesController(weeklyUpdateService);

            int year = 1920;

            // Act
            var result = await testController.RunWeeklyUpdate(year);

            // Assert
            A.CallTo(() => weeklyUpdateService.RunWeeklyUpdate(year)).MustHaveHappenedOnceExactly();
            result.ShouldBeOfType<OkResult>();
        }
    }
}
