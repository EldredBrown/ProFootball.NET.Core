using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.Controllers;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.Tests.ControllerTests
{
    public class AdminControllerTest
    {
        [Fact]
        public void Index_ShouldReturnAdminIndexView()
        {
            // Arrange
            var testController = new AdminController();

            // Act
            var result = testController.Index();

            // Assert
            result.ShouldBeOfType<ViewResult>();
        }
    }
}
