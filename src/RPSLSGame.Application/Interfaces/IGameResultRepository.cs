using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Interfaces;

public interface IGameResultRepository
{
    Task AddGameResultAsync(GameResult gameResult, CancellationToken cancellationToken);

    Task<PagedResult<GameResult>> GetHistoryAsync(string userId, int page, int pageSize,
        CancellationToken cancellationToken);
}