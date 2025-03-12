using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.Commands;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application;

public class GameService(IMediator mediator): IGameService
{
    public async Task<List<GameChoiceDto>> GetChoicesAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetChoicesQuery(), cancellationToken);
    }

    public async Task<GameChoiceDto> GetRandomChoiceAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetRandomChoiceQuery(), cancellationToken);
    }

    public async Task<Result<PagedResult<PlayGameResponse>>> GetHistoryAsync(string email, int page, int pageSize, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetGameHistoryQuery(email, page, pageSize), cancellationToken);
    }

    public async Task<Result<PlayGameResponse>> PlayGameAsync(GameMove playerMove, string userId, CancellationToken cancellationToken)
    {
        return await mediator.Send(new PlayGameCommand(playerMove, userId), cancellationToken);
    }
}
