using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;
using RPSLSGame.Api.DTOs;
using RPSLSGame.Infrastructure.Data;

namespace RPSLSGame.Tests.IntegrationTests;

public class GameControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetChoices_ShouldReturnAllGameMoves()
    {
        var expectedGameMoves = new List<GameMove>
        {
            GameMove.Rock,
            GameMove.Paper,
            GameMove.Scissors,
            GameMove.Lizard,
            GameMove.Spock
        };

        var response = await _client.GetAsync("/choices");
        var choices = await response.Content.ReadFromJsonAsync<List<GameChoiceDto>>();
        
        Assert.NotNull(choices);
        Assert.Equal(expectedGameMoves.Count, choices!.Count);

        var gameMoves = choices.Select(c => (GameMove)c.Id).ToList();

        Assert.Equal(expectedGameMoves, gameMoves);
    }

    [Fact]
    public async Task GetRandomChoice_ShouldReturnValidMove()
    {
        var response = await _client.GetAsync("/choice");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GameChoiceDto>();
        Assert.NotNull(result);
        Assert.True(Enum.IsDefined(typeof(GameMove), result!.Id));
    }

    [Fact]
    public async Task PlayGame_ShouldReturnValidResponse_WhenCalledWithValidData()
    {
        var request = new PlayGameRequest((int)GameMove.Rock); 
        
        var response = await _client.PostAsJsonAsync("/play", request);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PlayGameResponse>();
        Assert.NotNull(result);
        Assert.Equal(GameMove.Rock, result!.Player);
        Assert.False(string.IsNullOrEmpty(result.Results)); 
    }
    
    [Fact]
    public async Task PlayGame_ShouldReturnValidResponse_WhenCalledWithRegisteredUser()
    {
        var request = new PlayGameRequest((int)GameMove.Paper, "user@example.com");

        var response = await _client.PostAsJsonAsync("/play", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PlayGameResponse>();
        Assert.NotNull(result);
        Assert.Equal(GameMove.Paper, result!.Player);
        Assert.False(string.IsNullOrEmpty(result.Results));
    }
    
    [Fact]
    public async Task PlayGame_ShouldReturnBadRequest_WhenInvalidMoveIsProvided()
    {
        const int playerMove = 6;
        var invalidRequest = new { Player = playerMove };

        var response = await _client.PostAsJsonAsync("/play", invalidRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PlayGame_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        var response = await _client.PostAsJsonAsync<object>("/play", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetGameHistory_ShouldReturnBadRequest_WhenEmailIsMissing()
    {
        var request = new GameHistoryRequest("");
        const int page = 1;
        const int pageSize = 10;
        
        var response = await _client.PostAsJsonAsync($"/history/search?page={page}&pageSize={pageSize}", request);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetGameHistory_ShouldReturnBadRequest_WhenPageParametersAreNegative()
    {
        var request = new GameHistoryRequest("user@example.com");
        
        var response = await _client.PostAsJsonAsync("/history/search?page=-1&pageSize=-5", request);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetGameHistory_ShouldReturnEmpty_WhenNoGamesExistForUser()
    {
        var request = new GameHistoryRequest("nonexistentuser@example.com");
        
        var response = await _client.PostAsJsonAsync("/history/search?page=1&pageSize=10", request);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PlayGameResponse>>();
        
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }
}

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Find and remove ALL EF Core related services
            var descriptors = services.Where(
                d => d.ServiceType.Namespace.Contains("EntityFrameworkCore") ||
                     d.ServiceType == typeof(DbContextOptions<GameDbContext>) ||
                     d.ServiceType == typeof(GameDbContext)).ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<GameDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryGameTestDb");
            });
            
            services.AddSingleton<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();
        });
    }
}