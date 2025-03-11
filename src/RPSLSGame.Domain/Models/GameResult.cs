namespace RPSLSGame.Domain.Models;

public class GameResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; }
    public GameMove Player { get; set; }
    public GameMove Computer { get; set; }
    public string Result { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    private GameResult(){}
    public GameResult(string email, GameMove playerMove, GameMove computerMove, string result)
    {
        Player = playerMove;
        Computer = computerMove;
        Result = result;
        Email = email;
    }
}
