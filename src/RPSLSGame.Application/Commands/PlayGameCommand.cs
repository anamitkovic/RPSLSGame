using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Application.Commands;

public record PlayGameCommand(GameMove PlayerMove, string Email) : IRequest<Result<PlayGameResponse>>;