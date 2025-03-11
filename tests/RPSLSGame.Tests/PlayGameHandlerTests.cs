using Moq;
using RPSLSGame.Application.Commands;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests;

public class PlayGameHandlerTests
{
    private readonly Mock<IRandomNumberService> _randomNumberServiceMock;
    private readonly Mock<IGameResultRepository> _gameRepositoryMock;
    private readonly PlayGameHandler _handler;

    public PlayGameHandlerTests()
    {
        _randomNumberServiceMock = new Mock<IRandomNumberService>();
        _gameRepositoryMock = new Mock<IGameResultRepository>();
        _handler = new PlayGameHandler(_randomNumberServiceMock.Object, _gameRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTie_WhenPlayerAndComputerChooseSameMove()
    {

        const GameMove playerMove = GameMove.Rock;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(1);

        var command = new PlayGameCommand(playerMove, "1123");
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal("tie", result.Value.Result);
        Assert.Equal(GameMove.Rock, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }

    [Fact]
    public async Task Handle_ShouldReturnWin_WhenPlayerBeatsComputer()
    {
        const GameMove playerMove = GameMove.Rock;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(3); 

        var command = new PlayGameCommand(playerMove, "1123");

        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal("win", result.Value.Result);
        Assert.Equal(GameMove.Scissors, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }

    [Fact]
    public async Task Handle_ShouldReturnLose_WhenPlayerLosesToComputer()
    {
        const GameMove playerMove = GameMove.Scissors;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(1);

        var command = new PlayGameCommand(playerMove, "1123");
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal("lose", result.Value.Result);
        Assert.Equal(GameMove.Rock, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }
    
}