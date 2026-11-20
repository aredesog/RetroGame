using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Genero;

// Só formato. Nome repetido é regra de negócio (107) e fica no GeneroService.
// Tamanho igual à coluna no_genero VARCHAR(50).
public class InserirGeneroRequestValidator : AbstractValidator<InserirGeneroRequestApiModel>
{
    public InserirGeneroRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(50).WithMessage("O nome deve ter até 50 caracteres.");
    }
}
