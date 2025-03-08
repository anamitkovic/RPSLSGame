using CSharpFunctionalExtensions;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Interfaces;

public interface IGameService
{
    Task<Result<GameResult>> PlayGameAsync(GameMove playerMove);
    Task<List<GameChoiceDto>> GetChoicesAsync();
    Task<GameChoiceDto> GetRandomChoiceAsync();
    Task<List<GameResult>> GetRecentGamesAsync(int count);
}