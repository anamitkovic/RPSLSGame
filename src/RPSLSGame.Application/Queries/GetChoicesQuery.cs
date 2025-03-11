using MediatR;
using RPSLSGame.Application.DTOs;

namespace RPSLSGame.Application.Queries;

public record GetChoicesQuery : IRequest<List<GameChoiceDto>>;