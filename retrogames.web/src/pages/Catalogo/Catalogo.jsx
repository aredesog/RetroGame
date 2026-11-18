import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { generoApi, jogoApi, plataformaApi } from '../../api/servicos'
import CardJogo from '../../components/CardJogo'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import Paginacao from '../../components/Paginacao'

const TAMANHO_PAGINA = 12

export default function Catalogo() {
  // Os filtros moram na URL (?busca=mario&idPlataforma=2&pagina=2):
  // dá para compartilhar o link, e o botão Voltar do navegador restaura o filtro anterior.
  const [parametros, setParametros] = useSearchParams()
  const busca = parametros.get('busca') ?? ''
  const idPlataforma = parametros.get('idPlataforma') ?? ''
  const idGenero = parametros.get('idGenero') ?? ''
  const pagina = Number(parametros.get('pagina')) || 1

  const [textoBusca, setTextoBusca] = useState(busca)
  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const [tentativa, setTentativa] = useState(0) // muda no "Tentar novamente" para refazer a busca

  // Mantém a caixa de busca igual à URL (por exemplo, depois de apertar Voltar).
  useEffect(() => {
    setTextoBusca(busca)
  }, [busca])

  // Listas dos filtros: carregam uma vez só ([] = sem dependências).
  useEffect(() => {
    Promise.all([plataformaApi.listar(), generoApi.listar()])
      .then(([listaPlataformas, listaGeneros]) => {
        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
      })
      .catch(() => {
        // Se falhar, os filtros ficam vazios; o erro aparece na busca dos jogos.
      })
  }, [])

  // Jogos: recarregam sempre que um filtro da URL muda.
  useEffect(() => {
    let ignorar = false // evita mostrar a resposta de uma busca antiga que chegou atrasada

    setCarregando(true)
    setErro(null)

    jogoApi
      .listar({
        busca: busca || undefined, // undefined não entra na URL da requisição
        idPlataforma: idPlataforma || undefined,
        idGenero: idGenero || undefined,
        pagina,
        tamanhoPagina: TAMANHO_PAGINA,
      })
      .then((dados) => {
        if (!ignorar) setResultado(dados)
      })
      .catch((falha) => {
        if (!ignorar) setErro(obterMensagemErro(falha))
      })
      .finally(() => {
        if (!ignorar) setCarregando(false)
      })

    return () => {
      ignorar = true
    }
  }, [busca, idPlataforma, idGenero, pagina, tentativa])

  // Trocar um filtro volta para a página 1; valor vazio sai da URL.
  function alterarFiltro(nome, valor) {
    const novos = new URLSearchParams(parametros)
    if (valor) {
      novos.set(nome, valor)
    } else {
      novos.delete(nome)
    }
    novos.delete('pagina')
    setParametros(novos)
  }

  function mudarPagina(novaPagina) {
    const novos = new URLSearchParams(parametros)
    novos.set('pagina', novaPagina)
    setParametros(novos)
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  function aoBuscar(evento) {
    evento.preventDefault() // impede o formulário de recarregar a página
    alterarFiltro('busca', textoBusca.trim())
  }

  function limparFiltros() {
    setParametros(new URLSearchParams())
  }

  const temFiltro = Boolean(busca || idPlataforma || idGenero)

  return (
    <>
      <h1 className="h3 mb-4">Catálogo</h1>

      <form className="row g-2 mb-4" onSubmit={aoBuscar}>
        <div className="col-12 col-md-5">
          <div className="input-group">
            <input
              type="search"
              className="form-control"
              placeholder="Buscar pelo nome"
              aria-label="Buscar pelo nome"
              value={textoBusca}
              onChange={(e) => setTextoBusca(e.target.value)}
            />
            <button type="submit" className="btn btn-primary">
              Buscar
            </button>
          </div>
        </div>

        <div className="col-6 col-md-3">
          <select
            className="form-select"
            aria-label="Filtrar por plataforma"
            value={idPlataforma}
            onChange={(e) => alterarFiltro('idPlataforma', e.target.value)}
          >
            <option value="">Todas as plataformas</option>
            {plataformas.map((p) => (
              <option key={p.id} value={p.id}>
                {p.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-6 col-md-3">
          <select
            className="form-select"
            aria-label="Filtrar por gênero"
            value={idGenero}
            onChange={(e) => alterarFiltro('idGenero', e.target.value)}
          >
            <option value="">Todos os gêneros</option>
            {generos.map((g) => (
              <option key={g.id} value={g.id}>
                {g.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-md-1 d-grid">
          <button type="button" className="btn btn-outline-secondary" onClick={limparFiltros} disabled={!temFiltro}>
            Limpar
          </button>
        </div>
      </form>

      {carregando && <Carregando texto="Carregando jogos..." />}

      {!carregando && erro && <MensagemErro mensagem={erro} aoTentarNovamente={() => setTentativa((t) => t + 1)} />}

      {!carregando && !erro && resultado && resultado.itens.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado">
          {temFiltro && <p>Tente outra busca ou limpe os filtros.</p>}
        </EstadoVazio>
      )}

      {!carregando && !erro && resultado && resultado.itens.length > 0 && (
        <>
          <p className="text-muted small">
            {resultado.totalItens} {resultado.totalItens === 1 ? 'jogo encontrado' : 'jogos encontrados'}
          </p>

          <div className="row row-cols-1 row-cols-sm-2 row-cols-lg-3 row-cols-xl-4 g-3 mb-4">
            {resultado.itens.map((jogo) => (
              <div className="col" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
          </div>

          <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas} aoMudar={mudarPagina} />
        </>
      )}
    </>
  )
}
