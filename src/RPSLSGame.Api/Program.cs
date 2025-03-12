using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RPSLSGame.Api.Middlewares;
using RPSLSGame.Api.Validators;
using RPSLSGame.Application;
using RPSLSGame.Application.Extensions;
using RPSLSGame.Application.Interfaces;
using RPSLSGame.Infrastructure.Data;
using RPSLSGame.Infrastructure.Extensions;
using RPSLSGame.Infrastructure.Repositories;
using RPSLSGame.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddHttpClient<IRandomNumberService, RandomNumberService>();
builder.Services.AddScoped<IGameResultRepository, GameResultRepository>();
builder.Services.AddControllers()
    .AddFluentValidation(fv => 
    {
        fv.RegisterValidatorsFromAssemblyContaining<PlayGameRequestValidator>();
        fv.RegisterValidatorsFromAssemblyContaining<GameHistoryRequestValidator>();
    });

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var connectionString = builder.Configuration.GetConnectionString("Postgres");

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowSpecificOrigin");

app.Run();
public partial class Program { }