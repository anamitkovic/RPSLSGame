using Microsoft.EntityFrameworkCore;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;
using RPSLSGame.Infrastructure.Data;

namespace RPSLSGame.Infrastructure.Repositories;

public class GameResultRepository(GameDbContext dbContext) : IGameResultRepository
{
    public async Task AddGameResultAsync(GameResult gameResult, CancellationToken cancellationToken)
    {
        await dbContext.GameResults.AddAsync(gameResult, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<PagedResult<GameResult>> GetHistoryAsync(string email, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.GameResults
            .Where(gr => gr.Email == email)
            .OrderByDescending(gr => gr.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var results = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<GameResult>(results, totalCount, page, pageSize);
    }

}
