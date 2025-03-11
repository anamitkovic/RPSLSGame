using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;
using RPSLSGame.Api.DTOs;

namespace RPSLSGame.Tests.IntegrationTests;

public class GameControllerIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetChoices_ShouldReturnAllGameMoves()
    {
        var response = await _client.GetAsync("/choices");
    
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var choices = await response.Content.ReadFromJsonAsync<List<GameChoiceDto>>();
        Assert.NotNull(choices);
        Assert.Equal(5, choices!.Count);
        
        var gameMoves = choices.Select(c => (GameMove)c.Id).ToList();

        Assert.Contains(GameMove.Rock, gameMoves);
        Assert.Contains(GameMove.Paper, gameMoves);
        Assert.Contains(GameMove.Scissors, gameMoves);
        Assert.Contains(GameMove.Lizard, gameMoves);
        Assert.Contains(GameMove.Spock, gameMoves);
    }

    
    [Fact]
    public async Task GetRandomChoice_ShouldReturnValidMove()
    {
        var response = await _client.GetAsync("/choice");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GameChoiceDto>();
        Assert.NotNull(result);
        Assert.True(Enum.IsDefined(typeof(GameMove), result.Id));
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
        var invalidRequest = new { player = 6, email = "user@example.com" };

        var response = await _client.PostAsJsonAsync("/play", invalidRequest);

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
}
