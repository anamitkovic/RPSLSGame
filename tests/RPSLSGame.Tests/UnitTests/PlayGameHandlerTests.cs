using Moq;
using RPSLSGame.Application.Commands;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;
namespace RPSLSGame.Tests.UnitTests;

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

        var command = new PlayGameCommand(playerMove);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal("tie", result.Value.Results);
        Assert.Equal(GameMove.Rock, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }

    [Fact]
    public async Task Handle_ShouldReturnWin_WhenPlayerBeatsComputer()
    {
        const GameMove playerMove = GameMove.Rock;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(3); 

        var command = new PlayGameCommand(playerMove);

        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal("win", result.Value.Results);
        Assert.Equal(GameMove.Scissors, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }

    [Fact]
    public async Task Handle_ShouldReturnLose_WhenPlayerLosesToComputer()
    {
        const GameMove playerMove = GameMove.Scissors;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(1);

        var command = new PlayGameCommand(playerMove);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal("lose", result.Value.Results);
        Assert.Equal(GameMove.Rock, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }
    
    [Theory]
    [InlineData(GameMove.Rock, GameMove.Rock, "tie")]
    [InlineData(GameMove.Paper, GameMove.Paper, "tie")]
    [InlineData(GameMove.Scissors, GameMove.Scissors, "tie")]
    [InlineData(GameMove.Lizard, GameMove.Lizard, "tie")]
    [InlineData(GameMove.Spock, GameMove.Spock, "tie")]
    public async Task Handle_ShouldReturnTie_ForAllMoves(GameMove playerMove, GameMove computerMove, string expectedResult)
    {
        var randomNumber = (int)computerMove;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(randomNumber);

        var command = new PlayGameCommand(playerMove);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult, result.Value.Results);
        Assert.Equal(computerMove, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }
    
    [Theory]
    [InlineData(GameMove.Rock, GameMove.Scissors, "win")]
    [InlineData(GameMove.Rock, GameMove.Lizard, "win")]
    [InlineData(GameMove.Paper, GameMove.Rock, "win")]
    [InlineData(GameMove.Paper, GameMove.Spock, "win")]
    [InlineData(GameMove.Scissors, GameMove.Paper, "win")]
    [InlineData(GameMove.Scissors, GameMove.Lizard, "win")]
    [InlineData(GameMove.Lizard, GameMove.Paper, "win")]
    [InlineData(GameMove.Lizard, GameMove.Spock, "win")]
    [InlineData(GameMove.Spock, GameMove.Rock, "win")]
    [InlineData(GameMove.Spock, GameMove.Scissors, "win")]
    public async Task Handle_ShouldReturnWin_ForAllWinningCombinations(GameMove playerMove, GameMove computerMove, string expectedResult)
    {
        var randomNumber = (int)computerMove;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(randomNumber);

        var command = new PlayGameCommand(playerMove);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult, result.Value.Results);
        Assert.Equal(computerMove, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }
    
    [Theory]
    [InlineData(GameMove.Rock, GameMove.Paper, "lose")]
    [InlineData(GameMove.Rock, GameMove.Spock, "lose")]
    [InlineData(GameMove.Paper, GameMove.Scissors, "lose")]
    [InlineData(GameMove.Paper, GameMove.Lizard, "lose")]
    [InlineData(GameMove.Scissors, GameMove.Rock, "lose")]
    [InlineData(GameMove.Scissors, GameMove.Spock, "lose")]
    [InlineData(GameMove.Lizard, GameMove.Rock, "lose")]
    [InlineData(GameMove.Lizard, GameMove.Scissors, "lose")]
    [InlineData(GameMove.Spock, GameMove.Paper, "lose")]
    [InlineData(GameMove.Spock, GameMove.Lizard, "lose")]
    public async Task Handle_ShouldReturnLose_ForAllLosingCombinations(GameMove playerMove, GameMove computerMove, string expectedResult)
    {
        var randomNumber = (int)computerMove;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(randomNumber);

        var command = new PlayGameCommand(playerMove);
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedResult, result.Value.Results);
        Assert.Equal(computerMove, result.Value.Computer);
        Assert.Equal(playerMove, result.Value.Player);
    }

    [Fact]
    public async Task Handle_ShouldNotSaveGameResult_WhenEmailIsNotProvided()
    {
        const GameMove playerMove = GameMove.Rock;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(2);

        var command = new PlayGameCommand(playerMove);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _gameRepositoryMock.Verify(x => x.AddGameResultAsync(It.IsAny<GameResult>(), It.IsAny<CancellationToken>()), Times.Never);
    }

   
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("another@test.org")]
    public async Task Handle_ShouldSaveGameResult_WhenEmailIsProvided(string email)
    {
        // Arrange
        const GameMove playerMove = GameMove.Paper;
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(3);

        var command = new PlayGameCommand(playerMove, email);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("lose", result.Value.Results);

        _gameRepositoryMock.Verify(x => x.AddGameResultAsync(
                It.Is<GameResult>(gr => 
                    gr.PlayerMove == playerMove && 
                    gr.ComputerMove == GameMove.Scissors && 
                    gr.Result == "lose" && 
                    gr.Email == email), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}