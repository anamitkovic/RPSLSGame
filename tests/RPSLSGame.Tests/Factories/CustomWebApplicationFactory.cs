using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RPSLSGame.Infrastructure.Data;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Infrastructure.Services;

namespace RPSLSGame.Tests.Factories;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private WireMockServer _wireMockServer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Start WireMock server
        _wireMockServer = WireMockServer.Start(9090);

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var testConfig = new Dictionary<string, string>
            {
                { "ExternalServices:RandomNumberApi", "http://localhost:9090/random" }
            };

            config.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            // Configure HttpClient for RandomNumberService
            services.AddHttpClient<IRandomNumberService, RandomNumberService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:9090/random");
            });
            
            services.AddDbContext<GameDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryGameTestDb");
            });
        });
    }

    public void SetupValidResponse()
    {
        _wireMockServer?.Reset();
    
        _wireMockServer?.Given(
            Request.Create().WithPath("/random").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(200)
                .WithBody(_ => 
                {
                    var randomValue = new Random().Next(1, 100);
                    var responseJson = JsonSerializer.Serialize(new { random_number = randomValue });
                    return responseJson;
                })
                .WithHeader("Content-Type", "application/json")
        );
        
    }
    
    public void SetupServiceUnavailable()
    {
        _wireMockServer?.Reset();
    
        _wireMockServer?.Given(
            Request.Create().WithPath("/random").UsingGet()
        ).RespondWith(
            Response.Create()
                .WithStatusCode(503)
                .WithBody("Service Unavailable")
                .WithHeader("Content-Type", "text/plain")
        );
        
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _wireMockServer?.Stop();
            _wireMockServer?.Dispose();
        }
        
        base.Dispose(disposing);
    }
}