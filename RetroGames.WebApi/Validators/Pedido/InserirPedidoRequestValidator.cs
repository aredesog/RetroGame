using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Pedido;

// Só formato. Jogo indisponível (110) e estoque (111) dependem do banco e ficam no PedidoService.
public class InserirPedidoRequestValidator : AbstractValidator<InserirPedidoRequestApiModel>
{
    public InserirPedidoRequestValidator()
    {
        // Cascade Stop: com "itens": null, para no NotEmpty e não roda o Must (que daria erro 500).
        RuleFor(x => x.Itens)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Informe ao menos um item.")
            // tb_item_pedido tem UNIQUE (co_pedido, co_jogo): o mesmo jogo vai uma vez, com a quantidade somada.
            .Must(itens => itens.Select(i => i.IdJogo).Distinct().Count() == itens.Count)
            .WithMessage("O mesmo jogo não pode aparecer duas vezes no pedido.");

        RuleForEach(x => x.Itens).ChildRules(item =>
        {
            item.RuleFor(i => i.IdJogo)
                .GreaterThan(0).WithMessage("Informe o jogo.");

            item.RuleFor(i => i.Quantidade)
                .InclusiveBetween(1, 10).WithMessage("A quantidade deve ser de 1 a 10.");
        });
    }
}
