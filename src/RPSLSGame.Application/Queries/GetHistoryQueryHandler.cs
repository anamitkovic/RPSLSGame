using CSharpFunctionalExtensions;
using MediatR;
using RPSLSGame.Application.DTOs;
using RPSLSGame.Application.Extensions;
using RPSLSGame.Application.Interfaces;

namespace RPSLSGame.Application.Queries;

public class GetGameHistoryHandler(IGameResultRepository gameResultRepository)
    : IRequestHandler<GetGameHistoryQuery, Result<PagedResult<PlayGameResponse>>>
{
    public async Task<Result<PagedResult<PlayGameResponse>>> Handle(GetGameHistoryQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return Result.Failure<PagedResult<PlayGameResponse>>("Email is required.");
        }

        var gameResults = await gameResultRepository.GetHistoryAsync(request.Email, request.Page, request.PageSize, cancellationToken);

        var response = gameResults.Items.Select(gr => gr.MapToResponse()).ToList();

        return Result.Success(new PagedResult<PlayGameResponse>(response, gameResults.TotalCount, request.Page, request.PageSize));
    }


}