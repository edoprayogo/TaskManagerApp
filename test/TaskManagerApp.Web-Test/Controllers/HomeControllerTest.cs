using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerApp.Web.Controllers;
using TaskManagerApp.Web.Models;

namespace TaskManagerApp.Web_Test.Controllers;

public class HomeControllerTest
{
    private readonly HomeController _controller;

    public HomeControllerTest()
    {
        var loggerMock = new Mock<ILogger<HomeController>>();
        _controller = new HomeController(loggerMock.Object);
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.NotNull(result);
    }
}
