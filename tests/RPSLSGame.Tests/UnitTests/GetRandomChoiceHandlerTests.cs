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
    
    [Theory]
    [InlineData(1)]  
    [InlineData(5)] 
    [InlineData(100)]
    public async Task Handle_ShouldReturnValidChoice(int randomNumber)
    {
        // Arrange
        _randomNumberServiceMock
            .Setup(x => x.GetRandomNumberAsync(CancellationToken.None))
            .ReturnsAsync(randomNumber);

        var query = new GetRandomChoiceQuery();
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        var moves = Enum.GetValues<GameMove>().ToArray();
        var expectedMove = moves[(Math.Abs(randomNumber) - 1) % moves.Length]; 

        Assert.Equal((int)expectedMove, result.Id);
        Assert.Equal(expectedMove.ToString().ToLower(), result.Name);
    }
}