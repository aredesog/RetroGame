import { useEffect, useState } from 'react'
import { Link, useLocation } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { pedidoApi } from '../../api/servicos'
import { Carregando, EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import { formatarDataHora, formatarPreco } from '../../lib/formato'

const coresPorSituacao = {
  1: 'text-bg-warning',
  2: 'text-bg-success',
  3: 'text-bg-primary',
  4: 'text-bg-danger',
}

export default function MeusPedidos() {
  const { state } = useLocation()
  const [pedidos, setPedidos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ignorar = false

    pedidoApi.listarMeus()
      .then((dados) => {
        if (!ignorar) setPedidos(dados)
      })
      .catch((falha) => {
        if (!ignorar) setErro(obterMensagemErro(falha))
      })
      .finally(() => {
        if (!ignorar) setCarregando(false)
      })

    return () => { ignorar = true }
  }, [tentativa])

  function tentarNovamente() {
    setErro(null)
    setCarregando(true)
    setTentativa((atual) => atual + 1)
  }

  return (
    <>
      <h1 className="h3 mb-3">Meus pedidos</h1>
      {state?.pedidoRealizado && <div className="alert alert-success" role="status">Pedido realizado com sucesso!</div>}
      {carregando && <Carregando texto="Carregando pedidos..." />}
      {erro && <MensagemErro mensagem={erro} aoTentarNovamente={tentarNovamente} />}
      {!carregando && !erro && pedidos.length === 0 && (
        <EstadoVazio titulo="Você ainda não fez pedidos">
          <Link className="btn btn-primary mt-2" to="/">Ver catálogo</Link>
        </EstadoVazio>
      )}
      {!carregando && !erro && pedidos.map((pedido) => (
        <article className="card mb-3" key={pedido.id}>
          <div className="card-body">
            <div className="d-flex flex-wrap align-items-baseline gap-2">
              <h2 className="h5 mb-0">Pedido #{pedido.id}</h2>
              <span className={`badge ${coresPorSituacao[pedido.idSituacao] ?? 'text-bg-secondary'}`}>{pedido.descricaoSituacao}</span>
            </div>
            <p className="text-muted small mt-1">
              <time dateTime={pedido.data}>{formatarDataHora(pedido.data)}</time>
            </p>
            <ul className="list-group list-group-flush mb-3">
              {pedido.itens.map((item) => (
                <li className="list-group-item px-0 d-flex flex-wrap justify-content-between gap-1" key={item.idJogo}>
                  <span className="text-break">{item.nomeJogo}</span>
                  <span>{item.quantidade} × {formatarPreco(item.valorUnitario)}</span>
                </li>
              ))}
            </ul>
            <div className="d-flex flex-wrap justify-content-between align-items-center gap-2">
              <strong>Total: {formatarPreco(pedido.total)}</strong>
              <Link className="btn btn-outline-primary btn-sm" to={`/meus-pedidos/${pedido.id}`}>Ver detalhes</Link>
            </div>
          </div>
        </article>
      ))}
    </>
  )
}
