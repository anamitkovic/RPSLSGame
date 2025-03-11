using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Extensions;

public static class GameResultExtensions
{
    public static PlayGameResponse MapToResponse(this GameResult gameResult)
    {
        return new PlayGameResponse
        {
            Player = gameResult.Player,
            Computer = gameResult.Computer,
            Result = gameResult.Result
        };
    }
}