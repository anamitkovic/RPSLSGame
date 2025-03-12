using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Helpers;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Queries;

public class GetChoicesHandler : IRequestHandler<GetChoicesQuery, List<GameChoiceDto>>
{
    public async Task<List<GameChoiceDto>> Handle(GetChoicesQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(GameMoveHelper.Choices.ToList());
    }
}