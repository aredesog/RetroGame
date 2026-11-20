import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { pedidoApi } from '../../api/servicos'
import { EstadoVazio, MensagemErro } from '../../components/MensagensEstado'
import useCarrinho from '../../hooks/useCarrinho'
import { formatarPreco } from '../../lib/formato'

export default function Carrinho() {
  const { itens, total, alterarQuantidade, remover, limpar } = useCarrinho()
  const navegar = useNavigate()
  const [revisando, setRevisando] = useState(false)
  const [finalizando, setFinalizando] = useState(false)
  const [erro, setErro] = useState(null)

  async function finalizar() {
    setFinalizando(true)
    setErro(null)

    try {
      await pedidoApi.finalizar(itens)
      limpar()
      navegar('/meus-pedidos', { replace: true, state: { pedidoRealizado: true } })
    } catch (falha) {
      if (falha.response?.status !== 401) setErro(obterMensagemErro(falha))
      setRevisando(false)
    } finally {
      setFinalizando(false)
    }
  }

  return (
    <>
      <h1 className="h3 mb-3">Carrinho</h1>
      {erro && <MensagemErro mensagem={erro} />}
      {itens.length === 0 ? (
        <EstadoVazio titulo="Seu carrinho está vazio">
          <Link className="btn btn-primary mt-2" to="/">Ver catálogo</Link>
        </EstadoVazio>
      ) : (
        <section aria-label={revisando ? 'Resumo da compra' : 'Itens do carrinho'} aria-busy={finalizando}>
          {revisando && <h2 className="h5">Resumo da compra</h2>}
          <ul className="list-group mb-3">
            {itens.map((item) => (
              <li className="list-group-item py-3" key={item.idJogo}>
                <div className="row align-items-center gy-2">
                  <div className="col-md text-break">
                    <h3 className="h6 mb-1">{item.nome}</h3>
                    <p className="text-muted small mb-0">{item.nomePlataforma} · {formatarPreco(item.preco)} por unidade</p>
                  </div>
                  <div className="col-auto">
                    {revisando ? (
                      <span>Quantidade: {item.quantidade}</span>
                    ) : (
                      <div className="d-flex align-items-center gap-2" role="group" aria-label={`Quantidade de ${item.nome}`}>
                        <button
                          className="btn btn-outline-secondary btn-sm"
                          type="button"
                          aria-label={`Diminuir quantidade de ${item.nome}`}
                          disabled={item.quantidade === 1}
                          onClick={() => alterarQuantidade(item.idJogo, item.quantidade - 1)}
                        >−</button>
                        <span aria-live="polite">{item.quantidade}</span>
                        <button
                          className="btn btn-outline-secondary btn-sm"
                          type="button"
                          aria-label={`Aumentar quantidade de ${item.nome}`}
                          disabled={item.quantidade >= Math.min(10, item.estoque)}
                          onClick={() => alterarQuantidade(item.idJogo, item.quantidade + 1)}
                        >+</button>
                      </div>
                    )}
                  </div>
                  <div className="col-auto">
                    <span className="d-block text-muted small">Subtotal</span>
                    <strong>{formatarPreco(item.preco * item.quantidade)}</strong>
                  </div>
                  {!revisando && (
                    <div className="col-auto">
                      <button
                        className="btn btn-outline-danger btn-sm"
                        type="button"
                        aria-label={`Remover ${item.nome}`}
                        onClick={() => remover(item.idJogo)}
                      >Remover</button>
                    </div>
                  )}
                </div>
              </li>
            ))}
          </ul>
          <div className="d-flex flex-wrap justify-content-between align-items-center gap-3">
            <p className="h5 mb-0">Total: {formatarPreco(total)}</p>
            {revisando ? (
              <div className="d-flex flex-wrap gap-2">
                <button className="btn btn-outline-secondary" type="button" disabled={finalizando} onClick={() => setRevisando(false)}>
                  Cancelar
                </button>
                <button className="btn btn-primary" type="button" onClick={finalizar} disabled={finalizando}>
                  {finalizando ? 'Finalizando...' : 'Confirmar'}
                </button>
              </div>
            ) : (
              <button className="btn btn-primary" type="button" onClick={() => {
                setErro(null)
                setRevisando(true)
              }}>
                Finalizar compra
              </button>
            )}
          </div>
        </section>
      )}
    </>
  )
}
