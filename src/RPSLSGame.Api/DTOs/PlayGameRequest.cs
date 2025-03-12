namespace RPSLSGame.Api.DTOs;

/// <summary>
/// Represents a request to play the game.
/// </summary>
/// <param name="Player">The player's move, represented as an integer.</param>
/// <param name="Email">
/// Optional user identifier for tracking and enabling bonus features.
/// If provided, the game result may be stored for personalized scoreboard.
/// </param>
public record PlayGameRequest(int Player, string? Email = null);

