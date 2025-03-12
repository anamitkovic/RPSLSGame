using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Infrastructure.Services;

namespace RPSLSGame.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var randomNumberApiUrl = configuration["ExternalServices:RandomNumberApi"];
        
        if (string.IsNullOrWhiteSpace(randomNumberApiUrl))
        {
            throw new InvalidOperationException("RandomNumberApi URL is not configured properly.");
        }
        
        services.AddHttpClient<IRandomNumberService, RandomNumberService>(client =>
            {
                client.BaseAddress = new Uri(randomNumberApiUrl);
            })
            .AddResilienceHandler("randomNumberPolicy", builder =>
            {
                builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2), 
                    BackoffType = DelayBackoffType.Exponential, 
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .HandleResult(response => !response.IsSuccessStatusCode)
                });

                builder.AddTimeout(TimeSpan.FromSeconds(5));

                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
                {
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 2,
                    BreakDuration = TimeSpan.FromSeconds(15)
                });
            });
        
    }
}