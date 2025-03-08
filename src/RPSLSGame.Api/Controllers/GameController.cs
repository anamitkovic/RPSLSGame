using Microsoft.AspNetCore.Mvc;
using RPSLSGame.Api.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController(IGameService gameService) : ControllerBase
{
    /// <summary>
    /// Returns all possible choices (Rock, Paper, Scissors, Lizard, Spock).
    /// </summary>
    [HttpGet("choices")]
    public async Task<IActionResult> GetChoices()
    {
        var choices = await gameService.GetChoicesAsync();
        return Ok(choices);
    }

    /// <summary>
    /// Returns a random choice.
    /// </summary>
    [HttpGet("choice")]
    public async Task<IActionResult> GetRandomChoice()
    {
        var choice = await gameService.GetRandomChoiceAsync();
        return Ok(choice);
    }

    /// <summary>
    /// Plays a game round and returns the result.
    /// </summary>
    [HttpPost("play")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(GameResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PlayGame([FromBody] PlayGameRequest request)
    {
        var result = await gameService.PlayGameAsync((GameMove)request.Player);
        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }
}