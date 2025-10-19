using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class HomeControllerTest
    {
        [Fact]
        public void Index_ShouldReturnIndexView()
        {
            // Arrange
            var testController = new HomeController();

            // Act
            var result = testController.Index();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }

        [Fact]
        public void Privacy_ShouldReturnPrivacyView()
        {
            // Arrange
            var testController = new HomeController();

            // Act
            var result = testController.Privacy();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }
    }
}
