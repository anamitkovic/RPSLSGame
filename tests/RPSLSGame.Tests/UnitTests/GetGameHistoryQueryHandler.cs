using Moq;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests.UnitTests;

public class GetGameHistoryQueryHandlerTests
{
    private readonly Mock<IGameResultRepository> _gameResultRepositoryMock;
    private readonly GetGameHistoryHandler _handler;

    public GetGameHistoryQueryHandlerTests()
    {
        _gameResultRepositoryMock = new Mock<IGameResultRepository>();
        _handler = new GetGameHistoryHandler(_gameResultRepositoryMock.Object);
    }

    [Theory]
    [InlineData("user@example.com", 1, 2)]
    [InlineData("user@example.com", 2, 2)]
    [InlineData("user@example.com", 1, 5)] 
    public async Task Handle_ValidEmail_ReturnsPagedResult(string email, int page, int pageSize)
    {
        // Arrange
        var gameResults = new List<GameResult>
        {
            new(GameMove.Rock, GameMove.Scissors, "win") { Email = email },
            new(GameMove.Paper, GameMove.Rock, "win") { Email = email }
        };

        _gameResultRepositoryMock
            .Setup(repo => repo.GetHistoryAsync(email, page, pageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<GameResult>(gameResults, 5, page, pageSize));

        var request = new GetGameHistoryQuery(email, page, pageSize);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(gameResults.Count, result.Value.Items.Count);
        Assert.Equal(5, result.Value.TotalCount);
        
        for (var i = 0; i < gameResults.Count; i++)
        {
            Assert.Equal(gameResults[i].PlayerMove, result.Value.Items[i].Player);
            Assert.Equal(gameResults[i].ComputerMove, result.Value.Items[i].Computer);
            Assert.Equal(gameResults[i].Result, result.Value.Items[i].Results);
        }
    }
}