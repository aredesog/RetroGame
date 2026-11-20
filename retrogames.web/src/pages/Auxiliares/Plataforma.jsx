import { useEffect, useState } from 'react'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { plataformaApi } from '../../api/servicos'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'

const formularioVazio = { nome: '', fabricante: '', anoLancamento: '' }
const campos = [
  { nome: 'nome', rotulo: 'Nome', tipo: 'text' },
  { nome: 'fabricante', rotulo: 'Fabricante', tipo: 'text' },
  { nome: 'anoLancamento', rotulo: 'Ano de lançamento', tipo: 'number' },
]

export default function Plataforma() {
  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erroLista, setErroLista] = useState(null)
  const [erroOperacao, setErroOperacao] = useState(null)
  const [errosCampos, setErrosCampos] = useState({})
  const [dados, setDados] = useState(formularioVazio)
  const [idEdicao, setIdEdicao] = useState(null)
  const [ocupado, setOcupado] = useState(false)

  async function carregar() {
    setCarregando(true)
    setErroLista(null)
    try {
      setPlataformas(await plataformaApi.listar())
    } catch (falha) {
      setErroLista(obterMensagemErro(falha))
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregar()
  }, [])

  function definirEdicao(plataforma) {
    setIdEdicao(plataforma?.id ?? null)
    setDados(plataforma ? {
      nome: plataforma.nome,
      fabricante: plataforma.fabricante,
      anoLancamento: String(plataforma.anoLancamento),
    } : formularioVazio)
    setErrosCampos({})
    setErroOperacao(null)
  }

  async function salvar(evento) {
    evento.preventDefault()
    setOcupado(true)
    setErroOperacao(null)
    setErrosCampos({})

    try {
      const plataforma = { ...dados, anoLancamento: Number(dados.anoLancamento) }
      if (idEdicao === null) await plataformaApi.inserir(plataforma)
      else await plataformaApi.alterar(idEdicao, plataforma)
      definirEdicao(null)
      await carregar()
    } catch (falha) {
      const mensagens = obterErrosDosCampos(falha)
      setErrosCampos(mensagens)
      if (Object.keys(mensagens).length === 0) setErroOperacao(obterMensagemErro(falha))
    } finally {
      setOcupado(false)
    }
  }

  async function excluir(plataforma) {
    if (!window.confirm(`Excluir a plataforma "${plataforma.nome}"?`)) return

    setOcupado(true)
    setErroOperacao(null)
    try {
      await plataformaApi.excluir(plataforma.id)
      if (idEdicao === plataforma.id) definirEdicao(null)
      await carregar()
    } catch (falha) {
      setErroOperacao(obterMensagemErro(falha))
    } finally {
      setOcupado(false)
    }
  }

  return (
    <>
      <h1>Plataformas</h1>
      {erroOperacao && <MensagemErro mensagem={erroOperacao} />}
      <form className="mb-4" onSubmit={salvar} noValidate>
        <fieldset disabled={ocupado}>
          <legend className="h5">{idEdicao === null ? 'Nova plataforma' : 'Editar plataforma'}</legend>
          <div className="row g-3 mb-3">
            {campos.map(({ nome, rotulo, tipo }) => (
              <div className="col-md-4" key={nome}>
                <label className="form-label" htmlFor={nome}>{rotulo}</label>
                <input
                  id={nome}
                  type={tipo}
                  className={`form-control${errosCampos[nome] ? ' is-invalid' : ''}`}
                  value={dados[nome]}
                  onChange={(evento) => setDados({ ...dados, [nome]: evento.target.value })}
                  aria-invalid={Boolean(errosCampos[nome])}
                  aria-describedby={errosCampos[nome] ? `${nome}-erro` : undefined}
                />
                {errosCampos[nome] && (
                  <div id={`${nome}-erro`} className="invalid-feedback">{errosCampos[nome]}</div>
                )}
              </div>
            ))}
          </div>
          <button type="submit" className="btn btn-primary">Salvar</button>
          {idEdicao !== null && (
            <button type="button" className="btn btn-outline-secondary ms-2" onClick={() => definirEdicao(null)}>
              Cancelar
            </button>
          )}
        </fieldset>
      </form>

      {carregando && <Carregando />}
      {erroLista && <MensagemErro mensagem={erroLista} aoTentarNovamente={carregar} />}
      {!carregando && !erroLista && plataformas.length === 0 && (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada" />
      )}
      {!carregando && !erroLista && plataformas.length > 0 && (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr><th>Nome</th><th>Fabricante</th><th>Ano</th><th>Ações</th></tr>
            </thead>
            <tbody>
              {plataformas.map((plataforma) => (
                <tr key={plataforma.id}>
                  <td>{plataforma.nome}</td>
                  <td>{plataforma.fabricante}</td>
                  <td>{plataforma.anoLancamento}</td>
                  <td>
                    <button type="button" className="btn btn-sm btn-outline-primary me-2" disabled={ocupado} onClick={() => definirEdicao(plataforma)}>
                      Editar
                    </button>
                    <button type="button" className="btn btn-sm btn-outline-danger" disabled={ocupado} onClick={() => excluir(plataforma)}>
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}
