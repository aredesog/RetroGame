using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Pedido;

// Só formato. Se a situação existe de fato (112), quem confere é o PedidoService.
public class AlterarSituacaoPedidoRequestValidator : AbstractValidator<AlterarSituacaoPedidoRequestApiModel>
{
    public AlterarSituacaoPedidoRequestValidator()
    {
        // 0 é o valor padrão do int quando o campo não vem no JSON.
        RuleFor(x => x.IdSituacao)
            .GreaterThan(0).WithMessage("Informe a situação.");
    }
}
