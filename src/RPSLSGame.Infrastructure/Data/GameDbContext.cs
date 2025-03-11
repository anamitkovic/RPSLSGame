using Microsoft.EntityFrameworkCore;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Infrastructure.Data;

public class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options)
{
    public DbSet<GameResult> GameResults { get; set; }
}