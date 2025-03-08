namespace RPSLSGame.Domain.Models;

public static class GameRules
{
    public static readonly Dictionary<GameMove, HashSet<GameMove>> WinningMoves = new()
    {
        { GameMove.Rock, [GameMove.Scissors, GameMove.Lizard] },
        { GameMove.Paper, [GameMove.Rock, GameMove.Spock] },
        { GameMove.Scissors, [GameMove.Paper, GameMove.Lizard] },
        { GameMove.Lizard, [GameMove.Spock, GameMove.Paper] },
        { GameMove.Spock, [GameMove.Rock, GameMove.Scissors] }
    };
}
