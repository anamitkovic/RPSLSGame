using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Extensions;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Commands;

public class PlayGameHandler(IRandomNumberService randomNumberService, IGameResultRepository gameResultRepository) : IRequestHandler<PlayGameCommand, Result<PlayGameResponse>>
{
    public async Task<Result<PlayGameResponse>> Handle(PlayGameCommand request, CancellationToken cancellationToken)
    {
        var randomNumber = await randomNumberService.GetRandomNumberAsync(cancellationToken);

        var moves = Enum.GetValues<GameMove>();
        var computerMove = moves[(randomNumber - 1) % moves.Length];

        var result = DetermineWinner(request.PlayerMove, computerMove);
        var gameResult = new GameResult(request.PlayerMove, computerMove, result.ToString().ToLower());

        if (string.IsNullOrEmpty(request.Email)) return Result.Success(gameResult.MapToResponse());

        gameResult = gameResult with { Email = request.Email };
        
        await gameResultRepository.AddGameResultAsync(gameResult, cancellationToken);
        
        return Result.Success(gameResult.MapToResponse());
    }

    private static GameResultType DetermineWinner(GameMove player, GameMove computer)
    {
        if (player == computer) return GameResultType.Tie;

        return GameRules.WinningMoves[player].Contains(computer) ? GameResultType.Win : GameResultType.Lose;
    }
}