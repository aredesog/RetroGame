using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

public class InserirPedidoRequestValidator : AbstractValidator<InserirPedidoRequestApiModel>
{
    public InserirPedidoRequestValidator()
    {
        RuleFor(pedido => pedido.Itens)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe pelo menos um item.")
            .Must(itens => itens.All(item => item is not null))
                .WithMessage("Informe um jogo e a quantidade em cada item.")
            .Must(itens => itens.Select(item => item.IdJogo).Distinct().Count() == itens.Count)
                .WithMessage("Informe cada jogo apenas uma vez.");

        RuleForEach(pedido => pedido.Itens).ChildRules(item =>
        {
            item.RuleFor(valor => valor.IdJogo).GreaterThan(0).WithMessage("Informe um jogo válido.");
            item.RuleFor(valor => valor.Quantidade).InclusiveBetween(1, 10)
                .WithMessage("A quantidade deve estar entre 1 e 10.");
        });
    }
}
