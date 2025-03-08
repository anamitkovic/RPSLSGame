namespace RPSLSGame.Domain.Models;

public class GameResult(GameMove playerMove, GameMove computerMove, string result)
{
    public GameMove Player { get; } = playerMove;
    public GameMove Computer { get; } = computerMove;
    public string Result { get; } = result;
}
