using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators.Autenticacao;

// Só formato. E-mail já cadastrado é regra de negócio (106) e fica no AutenticacaoService.
// Tamanhos iguais às colunas de tb_usuario: no_usuario VARCHAR(100), ds_email VARCHAR(150).
public class CadastrarUsuarioRequestValidator : AbstractValidator<CadastrarUsuarioRequestApiModel>
{
    // A tela de cadastro (Pessoa 4) mostra esta mesma regra embaixo do campo de senha.
    public const int TamanhoMinimoSenha = 6;

    public CadastrarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(100).WithMessage("O nome deve ter até 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Informe o e-mail.")
            .MaximumLength(150).WithMessage("O e-mail deve ter até 150 caracteres.")
            .EmailAddress().WithMessage("E-mail inválido.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("Informe a senha.")
            .MinimumLength(TamanhoMinimoSenha)
            .WithMessage($"A senha deve ter pelo menos {TamanhoMinimoSenha} caracteres.");
    }
}
