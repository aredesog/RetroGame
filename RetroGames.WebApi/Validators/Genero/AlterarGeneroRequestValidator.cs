using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Genero;

public class AlterarGeneroRequestValidator : AbstractValidator<AlterarGeneroRequestApiModel>
{
    public AlterarGeneroRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(50).WithMessage("O nome deve ter até 50 caracteres.");
    }
}
