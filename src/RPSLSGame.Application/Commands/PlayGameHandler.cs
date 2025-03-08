using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Commands;

public class PlayGameHandler(IRandomNumberService randomNumberService) : IRequestHandler<PlayGameCommand, Result<GameResult>>
{
    private const string Win = "win";
    private const string Lose = "lose";
    private const string Tie = "tie";
    
    public async Task<Result<GameResult>> Handle(PlayGameCommand request, CancellationToken cancellationToken)
    {
        var randomNumber = await randomNumberService.GetRandomNumberAsync(); 
        
        var moves = Enum.GetValues<GameMove>();
        var computerMove = moves[(randomNumber - 1) % moves.Length];

        var result = DetermineWinner(request.PlayerMove, computerMove);
        var gameResult = new GameResult(request.PlayerMove, computerMove, result);

        return Result.Success(gameResult);
    }

    private static string DetermineWinner(GameMove player, GameMove computer)
    {
        if (player == computer) return Tie;

        return GameRules.WinningMoves[player].Contains(computer) ? Win : Lose;
    }
}