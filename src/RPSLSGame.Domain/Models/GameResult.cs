using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPSLSGame.Domain.Models;

public record GameResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public Guid Id { get; set; } 
    public string? Email { get; set; }
    public GameMove PlayerMove { get; set; }
    public GameMove ComputerMove { get; set; }
    public string Result { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Protected constructor is required for EF Core migrations.
    /// EF needs a parameterless constructor to materialize objects from the database.
    /// </summary>
    protected GameResult() {}
    public GameResult(GameMove playerMove, GameMove computerMove, string result)
    {
        PlayerMove = playerMove;
        ComputerMove = computerMove;
        Result = result;
    }
}
