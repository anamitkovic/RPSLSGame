using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Helpers;
using RPSLSGame.Application.Interfaces;

namespace RPSLSGame.Application.Queries;

public class GetRandomChoiceHandler(IRandomNumberService randomNumberService)
    : IRequestHandler<GetRandomChoiceQuery, GameChoiceDto>
{
    public async Task<GameChoiceDto> Handle(GetRandomChoiceQuery request, CancellationToken cancellationToken)
    {
        var choices = GameMoveHelper.Choices;
    
        var randomNumber = await randomNumberService.GetRandomNumberAsync(cancellationToken);
        var randomMove = choices[(randomNumber - 1) % choices.Count];

        return randomMove;
    }
}