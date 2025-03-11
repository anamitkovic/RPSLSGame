using Moq;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests;

public class GetRandomChoiceQueryHandlerTests
{
    private readonly Mock<IRandomNumberService> _randomNumberServiceMock = new();

    [Fact]
    public async Task Handle_ShouldReturnValidRandomChoice()
    {
        _randomNumberServiceMock.Setup(x => x.GetRandomNumberAsync(CancellationToken.None)).ReturnsAsync(5); 

        var handler = new GetRandomChoiceHandler(_randomNumberServiceMock.Object);
        var query = new GetRandomChoiceQuery();
        
        var result = await handler.Handle(query, CancellationToken.None);
        
        var validChoices = Enum.GetValues<GameMove>().Select(m => (int)m).ToList();
        
        Assert.Contains(result.Id, validChoices);
        Assert.NotEmpty(result.Name);
    }
}