using Microsoft.AspNetCore.Mvc;
using RPSLSGame.Api.DTOs;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Api.Controllers;

[ApiController]
[Route("api/game")]
public class GameController(IGameService gameService) : ControllerBase
{
    /// <summary>
    /// Returns all possible choices in the game (Rock, Paper, Scissors, Lizard, Spock).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>
    /// A list of possible game choices.
    /// - 200 OK: Successfully retrieved the list of choices.
    /// - 500 Internal Server Error: If an unexpected error occurs.
    /// </returns>
    [HttpGet("choices")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<GameMove>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChoices(CancellationToken cancellationToken = default)
    {
        var choices = await gameService.GetChoicesAsync(cancellationToken);
        return Ok(choices);
    }

    /// <summary>
    /// Returns a randomly selected game choice.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>
    /// A randomly selected game choice.
    /// - 200 OK: Successfully retrieved a random game choice.
    /// - 500 Internal Server Error: If an unexpected error occurs.
    /// </returns>
    [HttpGet("choice")]
    public async Task<IActionResult> GetRandomChoice(CancellationToken cancellationToken = default)
    {
        var choice = await gameService.GetRandomChoiceAsync(cancellationToken);
        return Ok(choice);
    }

    /// <summary>
    /// Plays a game round based on the user's move and returns the result.
    /// </summary>
    /// <param name="request">The request object containing the user's move and email.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>
    /// Returns the result of the game round.
    /// - 200 OK: Successfully played the game and returns the game result.
    /// - 400 Bad Request: If the request is invalid or missing required data.
    /// - 500 Internal Server Error: If an unexpected error occurs.
    /// </returns>
    [HttpPost("play")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PlayGameResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PlayGame([FromBody] PlayGameRequest request, CancellationToken cancellationToken = default)
    {
        var result = await gameService.PlayGameAsync((GameMove)request.Player, request.Email, cancellationToken);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }
    
    /// <summary>
    /// Retrieves a paginated history of games for a given user.
    /// </summary>
    /// <param name="request">The request object containing the user's email.</param>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of records per page (default is 10).</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>
    /// Returns a paginated list of game history results.
    /// - 200 OK: Successfully retrieved game history.
    /// - 400 Bad Request: If the email is invalid or missing.
    /// - 500 Internal Server Error: If an unexpected error occurs.
    /// </returns>
    [HttpPost("history/search")]
    [ProducesResponseType(typeof(PagedResult<PlayGameResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetGameHistory(
        [FromBody] GameHistoryRequest request,
        [FromQuery] int page = 1,  
        [FromQuery] int pageSize = 10,  
        CancellationToken cancellationToken = default)
    {
        var result = await gameService.GetHistory(request.Email, page, pageSize, cancellationToken);
        
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

}