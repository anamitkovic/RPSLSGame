using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.DTOs;

public class PlayGameResponse
{
    public GameMove Player { get; set; }
    public GameMove Computer { get; set; }
    public string Results { get; set; }
}
