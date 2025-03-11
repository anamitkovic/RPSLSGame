using Moq;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests;

public class GetGameHistoryQueryHandlerTests
{
    private readonly Mock<IGameResultRepository> _gameResultRepositoryMock;
    private readonly GetGameHistoryHandler _handler;

    public GetGameHistoryQueryHandlerTests()
    {
        _gameResultRepositoryMock = new Mock<IGameResultRepository>();
        _handler = new GetGameHistoryHandler(_gameResultRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidEmail_ReturnsPagedResult()
    {
        var request = new GetGameHistoryQuery("user@example.com", 1, 2);
        var gameResults = new List<GameResult>
        {
            new(GameMove.Rock, GameMove.Scissors, "win") { Email = "user@example.com" },
            new(GameMove.Paper, GameMove.Rock, "win") { Email = "user@example.com" }
        };

        _gameResultRepositoryMock
            .Setup(repo => repo.GetHistoryAsync(request.Email, request.Page, request.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<GameResult>(gameResults, 5, request.Page, request.PageSize));
        
        var result = await _handler.Handle(request, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(5, result.Value.TotalCount);
        Assert.Equal(GameMove.Rock, result.Value.Items[0].Player);
        Assert.Equal(GameMove.Scissors, result.Value.Items[0].Computer);
        Assert.Equal("win", result.Value.Items[0].Results);
    }

    [Fact]
    public async Task Handle_PaginationWorksCorrectly()
    {
        var request = new GetGameHistoryQuery("user@example.com", 2, 2);
        var gameResults = new List<GameResult>
        {
            new(GameMove.Scissors, GameMove.Paper, "win") { Email = "user@example.com" },
            new(GameMove.Lizard, GameMove.Spock, "lose") { Email = "user@example.com" }
        };

        _gameResultRepositoryMock
            .Setup(repo => repo.GetHistoryAsync(request.Email, request.Page, request.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<GameResult>(gameResults, 5, request.Page, request.PageSize));
        
        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(5, result.Value.TotalCount);
        Assert.Equal(GameMove.Scissors, result.Value.Items[0].Player);
        Assert.Equal(GameMove.Paper, result.Value.Items[0].Computer);
        Assert.Equal("win", result.Value.Items[0].Results);
        Assert.True(result.Value.HasMore);
    }
}
