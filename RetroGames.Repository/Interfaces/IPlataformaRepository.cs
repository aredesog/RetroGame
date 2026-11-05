using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

/// <summary>Acesso a dados da tabela tb_plataforma.</summary>
public interface IPlataformaRepository
{
    /// <summary>Todas as plataformas, ordenadas por nome.</summary>
    Task<List<Plataforma>> ListarAsync();

    /// <summary>A plataforma com o id informado, ou nulo se não existir.</summary>
    Task<Plataforma?> BuscarPorIdAsync(int id);

    /// <summary>
    /// Se já existe plataforma com o nome informado.
    /// idIgnorar serve para a alteração não acusar a própria plataforma.
    /// </summary>
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null);

    /// <summary>Se algum jogo usa a plataforma (consulta tb_jogo).</summary>
    Task<bool> PossuiJogosAsync(int id);

    /// <summary>Grava uma nova plataforma e devolve o id gerado pelo banco.</summary>
    Task<int> InserirAsync(Plataforma plataforma);

    /// <summary>Atualiza nome, fabricante e ano.</summary>
    Task AlterarAsync(Plataforma plataforma);

    /// <summary>Apaga a plataforma pelo id.</summary>
    Task ExcluirAsync(int id);
}
