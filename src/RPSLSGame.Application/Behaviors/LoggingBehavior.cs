using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace RPSLSGame.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogInformation($"[START] Handling {requestName} with request: {@request}");

            var stopwatch = Stopwatch.StartNew();
            var response = await next();
            stopwatch.Stop();

            logger.LogInformation($"[END] {requestName} handled in {stopwatch.ElapsedMilliseconds} ms with response: {@response}");

            return response;
        }
    }
}