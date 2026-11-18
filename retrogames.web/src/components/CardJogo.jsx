import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

// Só recebe dados por props: não busca nada na API.
export default function CardJogo({ jogo }) {
  const esgotado = jogo.estoque === 0

  return (
    <div className="card h-100 card-jogo">
      <div className="card-body d-flex flex-column">
        <div className="d-flex justify-content-between align-items-start mb-2">
          <span className="badge text-bg-secondary">{jogo.nomePlataforma}</span>
          {esgotado && <span className="badge text-bg-danger">Esgotado</span>}
        </div>

        <h2 className="card-title h6 mb-1">{jogo.nome}</h2>
        <p className="card-text small text-muted mb-2">
          {jogo.anoLancamento} · {jogo.descricaoCondicao}
        </p>

        <div className="mb-3">
          {jogo.generos.map((genero) => (
            <span key={genero} className="badge rounded-pill text-bg-light border me-1">
              {genero}
            </span>
          ))}
        </div>

        {/* mt-auto empurra preço e botão para o fim do cartão: todos ficam alinhados */}
        <div className="mt-auto d-flex justify-content-between align-items-center">
          <span className="fw-bold">{formatarPreco(jogo.preco)}</span>
          <Link className="btn btn-outline-primary btn-sm" to={`/jogos/${jogo.id}`}>
            Ver detalhes
          </Link>
        </div>
      </div>
    </div>
  )
}
