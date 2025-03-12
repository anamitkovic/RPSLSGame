using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Helpers;

public static class GameMoveHelper
{
    private static readonly Lazy<IReadOnlyList<GameChoiceDto>> GameChoices = new(() =>
        Enum.GetValues<GameMove>()
            .Select(move => new GameChoiceDto { Id = (int)move, Name = move.ToString().ToLower() })
            .ToList()
            .AsReadOnly());

    public static IReadOnlyList<GameChoiceDto> Choices => GameChoices.Value;
}