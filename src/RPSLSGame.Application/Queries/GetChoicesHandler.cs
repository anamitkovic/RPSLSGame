using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Queries;

public class GetChoicesHandler : IRequestHandler<GetChoicesQuery, List<GameChoiceDto>>
{
    public async Task<List<GameChoiceDto>> Handle(GetChoicesQuery request, CancellationToken cancellationToken)
    {
        var choices = Enum.GetValues<GameMove>()
            .Select(move => new GameChoiceDto { Id = (int)move, Name = move.ToString() })
            .ToList();

        return await Task.FromResult(choices);
    }
}