import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { jogoApi } from '../../api/servicos'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import Paginacao from '../../components/Paginacao'

export default function Jogos() {
  const [texto, setTexto] = useState('')
  const [busca, setBusca] = useState('')
  const [pagina, setPagina] = useState(1)
  const [resultado, setResultado] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const [erroOperacao, setErroOperacao] = useState(null)
  const [desativando, setDesativando] = useState(false)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ignorar = false
    setCarregando(true)
    setErro(null)
    jogoApi.listar({ busca: busca || undefined, pagina, incluirInativos: true })
      .then((dados) => {
        if (!ignorar) setResultado(dados)
      })
      .catch((falha) => {
        if (!ignorar) setErro(obterMensagemErro(falha))
      })
      .finally(() => {
        if (!ignorar) setCarregando(false)
      })
    return () => { ignorar = true }
  }, [busca, pagina, tentativa])

  async function desativar(jogo) {
    if (!window.confirm(`Desativar o jogo "${jogo.nome}"?`)) return

    setDesativando(true)
    setErroOperacao(null)
    try {
      await jogoApi.desativar(jogo.id)
      setCarregando(true)
      setTentativa((valor) => valor + 1)
    } catch (falha) {
      setErroOperacao(obterMensagemErro(falha))
    } finally {
      setDesativando(false)
    }
  }

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1>Jogos</h1>
        <Link to="/admin/jogos/novo" className="btn btn-primary">Novo jogo</Link>
      </div>
      <form className="mb-3" onSubmit={(evento) => {
        evento.preventDefault()
        setBusca(texto.trim())
        setPagina(1)
      }}>
        <label htmlFor="busca-jogos" className="form-label">Nome do jogo</label>
        <div className="input-group">
          <input id="busca-jogos" className="form-control" type="search" value={texto} onChange={(evento) => setTexto(evento.target.value)} />
          <button className="btn btn-outline-primary" type="submit">Buscar</button>
        </div>
      </form>
      {erroOperacao && <MensagemErro mensagem={erroOperacao} />}
      {carregando && <Carregando texto="Carregando jogos..." />}
      {erro && <MensagemErro mensagem={erro} aoTentarNovamente={() => setTentativa((valor) => valor + 1)} />}
      {!carregando && !erro && resultado && (
        <>
          {resultado.itens.length === 0 ? (
            <EstadoVazio titulo="Nenhum jogo encontrado" />
          ) : (
            <div className="table-responsive">
              <table className="table table-striped align-middle">
                <thead><tr><th>Nome</th><th>Plataforma</th><th>Situação</th><th>Ações</th></tr></thead>
                <tbody>
                  {resultado.itens.map((jogo) => (
                    <tr key={jogo.id}>
                      <td>{jogo.nome}</td>
                      <td>{jogo.nomePlataforma}</td>
                      <td>{jogo.ativo ? 'Ativo' : 'Inativo'}</td>
                      <td>
                        <Link className="btn btn-sm btn-outline-primary me-2" to={`/admin/jogos/${jogo.id}/editar`}>Editar</Link>
                        {jogo.ativo && (
                          <button className="btn btn-sm btn-outline-danger" type="button" disabled={desativando} onClick={() => desativar(jogo)}>
                            Desativar
                          </button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
          <Paginacao pagina={resultado.pagina} totalPaginas={resultado.totalPaginas} aoMudar={setPagina} />
        </>
      )}
    </>
  )
}
