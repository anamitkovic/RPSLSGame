using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Queries;

public class GetRandomChoiceHandler(IRandomNumberService randomNumberService)
    : IRequestHandler<GetRandomChoiceQuery, GameChoiceDto>
{
    public async Task<GameChoiceDto> Handle(GetRandomChoiceQuery request, CancellationToken cancellationToken)
    {
        var choices = Enum.GetValues<GameMove>().ToArray();
        
        var randomNumber = await randomNumberService.GetRandomNumberAsync();
        var randomMove = choices[(randomNumber - 1) % choices.Length];

        return new GameChoiceDto { Id = (int)randomMove, Name = randomMove.ToString() };
    }
}