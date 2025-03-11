using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests.UnitTests;

public class GetChoicesHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllGameChoices()
    {
        var handler = new GetChoicesHandler();
        var query = new GetChoicesQuery();
        
        var result = await handler.Handle(query, CancellationToken.None);
        var expectedChoices = Enum.GetValues<GameMove>().Select(m => new GameChoiceDto { Id = (int)m, Name = m.ToString() }).ToList();

        Assert.NotNull(result);
        Assert.Equal(expectedChoices.Count, result.Count);

        foreach (var expectedChoice in expectedChoices)
        {
            var actualChoice = result.FirstOrDefault(c => c.Id == expectedChoice.Id);
            Assert.NotNull(actualChoice);
            Assert.Equal(expectedChoice.Name, actualChoice.Name);
        }
    }
}