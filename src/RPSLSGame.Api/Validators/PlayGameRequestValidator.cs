using FluentValidation;
using RPSLSGame.Api.DTOs;
using RPSLSGame.Domain.Models;

namespace RPSLSGame.Api.Validators;

public class PlayGameRequestValidator : AbstractValidator<PlayGameRequest>
{
    public PlayGameRequestValidator()
    {
        RuleFor(x => x.Player)
            .Must(value => Enum.IsDefined(typeof(GameMove), value))
            .WithMessage("Invalid move. Allowed values: Rock, Paper, Scissors, Lizard, Spock.");
        
        RuleFor(x => x.Email)
            .NotEmpty().When(x => x.Email != null)
            .EmailAddress()
            .WithMessage("Invalid email format.");
    }
}