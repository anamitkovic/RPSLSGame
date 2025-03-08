using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.Commands;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Application.Queries;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application;

public class GameService(IMediator mediator)
    : IGameService
{
    private readonly List<GameResult> _gameHistory = new();

    public async Task<List<GameChoiceDto>> GetChoicesAsync()
    {
        return await mediator.Send(new GetChoicesQuery());
    }

    public async Task<GameChoiceDto> GetRandomChoiceAsync()
    {
        return await mediator.Send(new GetRandomChoiceQuery());
    }

    public async Task<Result<GameResult>> PlayGameAsync(GameMove playerMove)
    {
        return await mediator.Send(new PlayGameCommand(playerMove));
    }

    public async Task<List<GameResult>> GetRecentGamesAsync(int count)
    {
        return await Task.FromResult(_gameHistory.TakeLast(count).ToList());
    }
}
