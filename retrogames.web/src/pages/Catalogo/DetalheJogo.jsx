import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { jogoApi } from '../../api/servicos'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'
import { formatarPreco } from '../../lib/formato'

export default function DetalheJogo() {
  const { id } = useParams() // o número de /jogos/:id

  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const [semConexao, setSemConexao] = useState(false)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ignorar = false

    setCarregando(true)
    setErro(null)

    jogoApi
      .buscarPorId(id)
      .then((dados) => {
        if (!ignorar) setJogo(dados)
      })
      .catch((falha) => {
        if (ignorar) return
        // 404 traz "Jogo não encontrado." no envelope; sem resposta é falha de conexão.
        setErro(obterMensagemErro(falha))
        setSemConexao(!falha.response)
      })
      .finally(() => {
        if (!ignorar) setCarregando(false)
      })

    return () => {
      ignorar = true
    }
  }, [id, tentativa])

  const voltar = (
    <Link to="/" className="btn btn-link px-0 mb-3">
      ← Voltar ao catálogo
    </Link>
  )

  if (carregando) {
    return <Carregando texto="Carregando jogo..." />
  }

  if (erro) {
    return (
      <>
        {voltar}
        {/* "Tentar novamente" só faz sentido se o servidor não respondeu; para 404, repetir não muda nada */}
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={semConexao ? () => setTentativa((t) => t + 1) : undefined}
        />
      </>
    )
  }

  const esgotado = jogo.estoque === 0

  return (
    <>
      {voltar}

      <div className="card">
        <div className="card-body">
          <span className="badge text-bg-secondary mb-2">{jogo.nomePlataforma}</span>
          <h1 className="h3">{jogo.nome}</h1>
          <p className="text-muted">
            {jogo.anoLancamento} · {jogo.descricaoCondicao}
          </p>

          <div className="mb-3">
            {jogo.generos.map((genero) => (
              <span key={genero.id} className="badge rounded-pill text-bg-light border me-1">
                {genero.nome}
              </span>
            ))}
          </div>

          {/* ds_jogo é opcional: sem descrição, o parágrafo simplesmente não aparece */}
          {jogo.descricao && <p>{jogo.descricao}</p>}

          <p className="h4 fw-bold mb-1">{formatarPreco(jogo.preco)}</p>
          {esgotado ? (
            <span className="badge text-bg-danger">Esgotado</span>
          ) : (
            <p className="text-muted small mb-0">
              {jogo.estoque} {jogo.estoque === 1 ? 'unidade' : 'unidades'} em estoque
            </p>
          )}
        </div>
      </div>
    </>
  )
}
