using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Extensions;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Commands;

public class PlayGameHandler(IRandomNumberService randomNumberService, IGameResultRepository gameResultRepository) : IRequestHandler<PlayGameCommand, Result<PlayGameResponse>>
{
    private const string Win = "win";
    private const string Lose = "lose";
    private const string Tie = "tie";
    
    public async Task<Result<PlayGameResponse>> Handle(PlayGameCommand request, CancellationToken cancellationToken)
    {
        var randomNumber = await randomNumberService.GetRandomNumberAsync(cancellationToken); 
        
        var moves = Enum.GetValues<GameMove>();
        var computerMove = moves[(randomNumber - 1) % moves.Length];

        var result = DetermineWinner(request.PlayerMove, computerMove);
        var gameResult = new GameResult(request.Email, request.PlayerMove, computerMove, result);

        await gameResultRepository.AddGameResultAsync(gameResult, cancellationToken);
        
        return Result.Success(gameResult.MapToResponse());
    }

    private static string DetermineWinner(GameMove player, GameMove computer)
    {
        if (player == computer) return Tie;

        return GameRules.WinningMoves[player].Contains(computer) ? Win : Lose;
    }
}