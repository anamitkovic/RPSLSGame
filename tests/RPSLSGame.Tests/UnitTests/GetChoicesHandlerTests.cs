using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Tests.UnitTests;

public class GetChoicesHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnAllGameChoices()
    {
        // Arrange
        var handler = new GetChoicesHandler();
        var query = new GetChoicesQuery();
    
        var expectedChoices = Enum.GetValues<GameMove>()
            .Select(m => new GameChoiceDto { Id = (int)m, Name = m.ToString() })
            .OrderBy(c => c.Id)
            .ToList();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedChoices.Count, result.Count);
        
        Assert.Equal(expectedChoices.Select(c => c.Id),
            result.OrderBy(c => c.Id).Select(c => c.Id)
        );

        Assert.Equal(expectedChoices.Select(c => c.Name.ToLower()),
            result.OrderBy(c => c.Id).Select(c => c.Name)
        );
    }

}