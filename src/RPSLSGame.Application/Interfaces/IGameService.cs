using CSharpFunctionalExtensions;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Interfaces;

public interface IGameService
{
    Task<Result<PlayGameResponse>> PlayGameAsync(GameMove playerMove, string email, CancellationToken cancellationToken);
    Task<List<GameChoiceDto>> GetChoicesAsync(CancellationToken cancellationToken);
    Task<GameChoiceDto> GetRandomChoiceAsync(CancellationToken cancellationToken);
    Task<Result<PagedResult<PlayGameResponse>>> GetHistoryAsync(string email, int page, int pageSize, CancellationToken cancellationToken);
}