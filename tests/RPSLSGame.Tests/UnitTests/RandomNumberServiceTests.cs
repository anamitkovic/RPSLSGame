using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Infrastructure.Services;

namespace RPSLSGame.Tests.UnitTests;

public class RandomNumberServiceTests
{
    [Fact]
    public async Task GetRandomNumberAsync_ShouldRetryOnFailure()
    {
        var mockHandler = new Mock<HttpMessageHandler>();

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddHttpClient<IRandomNumberService, RandomNumberService>()
            .ConfigureHttpClient(client => { client.BaseAddress = new Uri("https://fake-api.com/random"); })
            .ConfigurePrimaryHttpMessageHandler(() => mockHandler.Object)
            .AddStandardResilienceHandler()
            .Services
            .BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IRandomNumberService>();

        await Assert.ThrowsAsync<HttpRequestException>(async () =>
            await service.GetRandomNumberAsync(CancellationToken.None));

        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(4),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }

    [Fact]
    public async Task GetRandomNumberAsync_ShouldReturnNumberOnSuccess()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var logger = new Mock<ILogger<RandomNumberService>>().Object;

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{ \"random_number\": 42 }")
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://fake-api.com/random")
        };

        var service = new RandomNumberService(httpClient, logger);

        // Act
        var result = await service.GetRandomNumberAsync(CancellationToken.None);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task GetRandomNumberAsync_ShouldRetryOnTransientError()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        var logger = new Mock<ILogger<RandomNumberService>>().Object;

        mockHandler.Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError })
            .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError })
            .ReturnsAsync(new HttpResponseMessage
                { StatusCode = HttpStatusCode.OK, Content = new StringContent("{ \"random_number\": 33 }") });

        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddHttpClient<IRandomNumberService, RandomNumberService>()
            .ConfigureHttpClient(client => { client.BaseAddress = new Uri("https://fake-api.com/random"); })
            .ConfigurePrimaryHttpMessageHandler(() => mockHandler.Object)
            .AddStandardResilienceHandler()
            .Services
            .BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IRandomNumberService>();

        // Act
        var result = await service.GetRandomNumberAsync(CancellationToken.None);

        // Assert
        Assert.Equal(33, result);
        mockHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(3),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}