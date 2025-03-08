using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using RPSLSGame.Application.Interfaces;

namespace RPSLSGame.Infrastructure.Services;

public class RandomNumberService(HttpClient httpClient, ILogger<RandomNumberService> logger) : IRandomNumberService
{
    public async Task<int> GetRandomNumberAsync()
    {
        var response = await httpClient.GetAsync("");
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<RandomNumberResponse>();
        return data?.RandomNumber ?? new Random().Next(1, 100);
    }

    private class RandomNumberResponse
    {
        [JsonPropertyName("random_number")] public int RandomNumber { get; set; }
    }
}