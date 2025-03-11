using RPSLSGame.Api.DTOs;

namespace RPSLSGame.Api.Validators;

using FluentValidation;

public class GameHistoryRequestValidator : AbstractValidator<GameHistoryRequest>
{
    public GameHistoryRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");
    }
}
