using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.DTOs;

namespace RPSLSGame.Application.Queries;

public record GetGameHistoryQuery(string Email, int Page = 1, int PageSize = 10) 
    : IRequest<Result<PagedResult<PlayGameResponse>>>;