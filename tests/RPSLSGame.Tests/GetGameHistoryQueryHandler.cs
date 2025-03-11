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
        var request = new GetGameHistoryQuery("user@example.com" ,1,  2);
        var gameResults = new List<GameResult>
        {
            new("user@example.com", GameMove.Rock, GameMove.Scissors, "Win"),
            new("user@example.com", GameMove.Paper, GameMove.Rock, "Win")
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
        Assert.Equal("Win", result.Value.Items[0].Result);
    }

    [Fact]
    public async Task Handle_EmptyEmail_ReturnsFailure()
    {
        var request = new GetGameHistoryQuery("", 1, 10);
        
        var result = await _handler.Handle(request, CancellationToken.None);
        
        Assert.True(result.IsFailure);
        Assert.Equal("Email is required.", result.Error);
    }

    [Fact]
    public async Task Handle_PaginationWorksCorrectly()
    {
        var request = new GetGameHistoryQuery("user@example.com",  2, 2);
        var gameResults = new List<GameResult>
        {
            new("user@example.com", GameMove.Scissors, GameMove.Paper, "Win"),
            new("user@example.com", GameMove.Lizard, GameMove.Spock, "Lose")
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
        Assert.Equal("Win", result.Value.Items[0].Result);
        Assert.True(result.Value.HasMore);
    }
}