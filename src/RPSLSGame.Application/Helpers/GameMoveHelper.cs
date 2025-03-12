using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Helpers;

public static class GameMoveHelper
{
    private static readonly List<GameChoiceDto> GameChoices = Enum.GetValues<GameMove>()
        .Select(move => new GameChoiceDto { Id = (int)move, Name = move.ToString() })
        .ToList();

    public static IReadOnlyList<GameChoiceDto> Choices => GameChoices;
}