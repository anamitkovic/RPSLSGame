using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using RPSLSGame.Application.Interfaces;

namespace RPSLSGame.Infrastructure.Services;

public class RandomNumberService(HttpClient httpClient, ILogger<RandomNumberService> logger) : IRandomNumberService
{
    public async Task<int> GetRandomNumberAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Requesting a random number from external service...");
        var response = await httpClient.GetAsync("", cancellationToken);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<RandomNumberResponse>(cancellationToken);

        if (data is { RandomNumber: > 0 })
        {
            logger.LogInformation("Successfully retrieved random number: {RandomNumber}", data);
            return data.RandomNumber;
        }

        logger.LogWarning("External service returned null or invalid value. Falling back to random generation.");
        return new Random().Next(1, 100);

    }

    private record RandomNumberResponse([property: JsonPropertyName("random_number")] int RandomNumber);

}