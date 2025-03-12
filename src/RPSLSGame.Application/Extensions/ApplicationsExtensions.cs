using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RPSLSGame.Application.Behaviors;

namespace RPSLSGame.Application.Extensions;

public static class ApplicationsExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }
}