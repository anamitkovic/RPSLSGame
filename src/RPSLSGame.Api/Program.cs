using FluentValidation;
using FluentValidation.AspNetCore;
using RPSLSGame.Api.Middlewares;
using RPSLSGame.Api.Validators;
using RPSLSGame.Application;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Infrastructure.Extensions;
using RPSLSGame.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GameService).Assembly));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddHttpClient<IRandomNumberService, RandomNumberService>();

builder.Services.AddControllers()
    .AddFluentValidation(fv => 
    {
        fv.RegisterValidatorsFromAssemblyContaining<PlayGameRequestValidator>();
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();