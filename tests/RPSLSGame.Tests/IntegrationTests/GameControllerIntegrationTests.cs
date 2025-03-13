using System.Net;
using System.Net.Http.Json;
using System.Text;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;
using RPSLSGame.Api.DTOs;
using RPSLSGame.Tests.Factories;

namespace RPSLSGame.Tests.IntegrationTests;

public class GameControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetChoices_ShouldReturnAllGameMoves()
    {
        // Arrange
        var expectedGameMoves = new List<GameMove>
        {
            GameMove.Rock,
            GameMove.Paper,
            GameMove.Scissors,
            GameMove.Lizard,
            GameMove.Spock
        };

        // Act
        var response = await _client.GetAsync("/choices");
        var choices = await response.Content.ReadFromJsonAsync<List<GameChoiceDto>>();
        
        // Assert
        Assert.NotNull(choices);
        Assert.Equal(expectedGameMoves.Count, choices!.Count);

        var gameMoves = choices.Select(c => (GameMove)c.Id).ToList();

        Assert.Equal(expectedGameMoves, gameMoves);
    }

    [Fact]
    public async Task GetRandomChoice_ShouldReturnValidMove()
    {
        // Act
        factory.SetupValidResponse();
        var response = await _client.GetAsync("/choice");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<GameChoiceDto>();
        Assert.NotNull(result);
        Assert.True(Enum.IsDefined(typeof(GameMove), result!.Id));
    }

    [Theory]
    [InlineData(GameMove.Rock)]
    [InlineData(GameMove.Paper)]
    [InlineData(GameMove.Spock)]
    [InlineData(GameMove.Scissors)]
    [InlineData(GameMove.Lizard)]
    public async Task PlayGame_ShouldReturnValidResponse_WhenCalledWithValidData(GameMove playerMove)
    {
        //Arrange
        factory.SetupValidResponse();
        var request = new PlayGameRequest((int)playerMove); 
        
        //Act
        var response = await _client.PostAsJsonAsync("/play", request);
        
        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<PlayGameResponse>();
        Assert.NotNull(result);
        Assert.Equal(playerMove, result!.Player);
        Assert.False(string.IsNullOrEmpty(result.Results)); 
    }
    
    [Theory]
    [InlineData(GameMove.Rock, "user1@example.com")]
    [InlineData(GameMove.Paper, "user2@example.com")]
    [InlineData(GameMove.Scissors, "user3@example.com")]
    [InlineData(GameMove.Lizard, "user4@example.com")]
    [InlineData(GameMove.Spock, "user5@example.com")]
    public async Task PlayGame_ShouldReturnValidResponse_WhenCalledWithRegisteredUser(GameMove playerMove, string email)
    {
        // Arrange
        factory.SetupValidResponse();
        var request = new PlayGameRequest((int)playerMove, email);
        
        // Act
        var response = await _client.PostAsJsonAsync("/play", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PlayGameResponse>();
        Assert.NotNull(result);
        Assert.Equal(playerMove, result!.Player);
        Assert.False(string.IsNullOrEmpty(result.Results));
    }

    [Theory]
    [InlineData(-1)]  
    [InlineData(6)] 
    [InlineData(100)]
    [InlineData(999)] 
    public async Task PlayGame_ShouldReturnBadRequest_WhenInvalidMoveIsProvided(int playerMove)
    {
        // Arrange
        var invalidRequest = new { Player = playerMove };
        
        // Act
        var response = await _client.PostAsJsonAsync("/play", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(GameMove.Rock)]
    [InlineData(GameMove.Paper)]
    [InlineData(GameMove.Scissors)]
    [InlineData(GameMove.Lizard)]
    [InlineData(GameMove.Spock)]
    public async Task PlayGame_ShouldReturnServiceUnavailable_WhenRandomNumberApiFails(GameMove playerMove)
    {
        // Arrange
        factory.SetupServiceUnavailable();
    
        // Act
        var response = await _client.PostAsJsonAsync("/play", new PlayGameRequest((int)playerMove));
    
        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }
    
    [Theory]
    [InlineData(null)]  
    [InlineData("{}")]
    public async Task PlayGame_ShouldReturnBadRequest_WhenRequestIsNullOrEmpty(string? requestBody)
    {
        // Arrange
        HttpContent content = requestBody == null 
            ? new StringContent("", Encoding.UTF8, "application/json")
            : new StringContent(requestBody, Encoding.UTF8, "application/json");
    
        // Act
        var response = await _client.PostAsync("/play", content);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }



    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]     
    public async Task GetGameHistory_ShouldReturnBadRequest_WhenEmailIsInvalid(string? email)
    {
        // Arrange
        var request = new GameHistoryRequest(email);
        const int page = 1;
        const int pageSize = 10;
    
        // Act
        var response = await _client.PostAsJsonAsync($"/history/search?page={page}&pageSize={pageSize}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(-1, -5)] 
    [InlineData(-1, 10)]
    [InlineData(1, -5)] 
    [InlineData(0, 0)]
    public async Task GetGameHistory_ShouldReturnBadRequest_WhenPageParametersAreInvalid(int page, int pageSize)
    {
        // Arrange
        var request = new GameHistoryRequest("user@example.com");
    
        // Act
        var response = await _client.PostAsJsonAsync($"/history/search?page={page}&pageSize={pageSize}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Theory]
    [InlineData("nonexistentuser@example.com")] 
    [InlineData("anotheruser@example.com")]   
    [InlineData("randomuser123@example.com")]
    public async Task GetGameHistory_ShouldReturnEmpty_WhenNoGamesExistForUser(string email)
    {
        // Arrange
        var request = new GameHistoryRequest(email);
    
        // Act
        var response = await _client.PostAsJsonAsync("/history/search?page=1&pageSize=10", request);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<PlayGameResponse>>();
    
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

}