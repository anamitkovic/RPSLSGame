using Moq;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests.UnitTests;

public class GetRandomChoiceQueryHandlerTests
{
    private readonly Mock<IRandomNumberService> _randomNumberServiceMock = new();
    private readonly GetRandomChoiceHandler _handler;

    public GetRandomChoiceQueryHandlerTests()
    {
        _handler = new GetRandomChoiceHandler(_randomNumberServiceMock.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnValidRandomChoice()
    {
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(5); 
        var query = new GetRandomChoiceQuery();
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        var validChoices = Enum.GetValues<GameMove>().Select(m => (int)m).ToList();
        
        Assert.Contains(result.Id, validChoices);
        Assert.NotEmpty(result.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnFirstChoice_WhenRandomNumberIsOne()
    {
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(1);
        var query = new GetRandomChoiceQuery();
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        Assert.Equal((int)GameMove.Rock, result.Id);
        Assert.Equal(GameMove.Rock.ToString(), result.Name);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidChoice_WhenRandomNumberIsOutOfBounds()
    {
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(100); 
        var query = new GetRandomChoiceQuery();
        
        var result = await _handler.Handle(query, CancellationToken.None);
        
        var moves = Enum.GetValues<GameMove>().ToArray();
        var expectedMove = moves[(100 - 1) % moves.Length]; 

        Assert.Equal((int)expectedMove, result.Id);
        Assert.Equal(expectedMove.ToString(), result.Name);
    }

    

}
