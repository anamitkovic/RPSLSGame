using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Commands;

public record PlayGameCommand(GameMove PlayerMove) : IRequest<Result<GameResult>>;