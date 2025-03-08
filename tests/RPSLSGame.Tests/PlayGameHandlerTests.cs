using Moq;
using RPSLSGame.Application.Commands;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests;

public class PlayGameHandlerTests
{
    private readonly Mock<IRandomNumberService> _randomNumberServiceMock;
    private readonly PlayGameHandler _handler;

    public PlayGameHandlerTests()
    {
        _randomNumberServiceMock = new Mock<IRandomNumberService>();
        _handler = new PlayGameHandler(_randomNumberServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTie_WhenPlayerAndComputerChooseSameMove()
    {
        // Arrange
        var playerMove = GameMove.Rock;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync()).ReturnsAsync(1); // Rock

        var command = new PlayGameCommand(playerMove);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess); // Proveravamo da li je rezultat uspešan
        Assert.Equal("tie", result.Value.Result);
        Assert.Equal(GameMove.Rock, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }

    [Fact]
    public async Task Handle_ShouldReturnWin_WhenPlayerBeatsComputer()
    {
        // Arrange
        var playerMove = GameMove.Rock;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync()).ReturnsAsync(3); // Scissors

        var command = new PlayGameCommand(playerMove);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("win", result.Value.Result);
        Assert.Equal(GameMove.Scissors, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }

    [Fact]
    public async Task Handle_ShouldReturnLose_WhenPlayerLosesToComputer()
    {
        // Arrange
        const GameMove playerMove = GameMove.Scissors;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync()).ReturnsAsync(1); // Rock

        var command = new PlayGameCommand(playerMove);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("lose", result.Value.Result);
        Assert.Equal(GameMove.Rock, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }
    
}