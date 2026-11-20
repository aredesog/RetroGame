import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { condicaoJogoApi, generoApi, jogoApi, plataformaApi } from '../../api/servicos'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'

const campos = [
  ['nome', 'Nome', 'text'],
  ['descricao', 'Descrição', 'textarea'],
  ['anoLancamento', 'Ano de lançamento', 'number'],
  ['preco', 'Preço', 'number'],
  ['estoque', 'Estoque', 'number'],
]

const formularioVazio = {
  nome: '', descricao: '', anoLancamento: '', preco: '', estoque: '',
  idPlataforma: '', idCondicaoJogo: '', idsGeneros: [], ativo: true,
}

export default function FormularioJogo() {
  const { id } = useParams()
  const navegar = useNavigate()
  const [dados, setDados] = useState(formularioVazio)
  const [opcoes, setOpcoes] = useState(null)
  const [erroCarga, setErroCarga] = useState(null)
  const [erro, setErro] = useState(null)
  const [errosCampos, setErrosCampos] = useState({})
  const [salvando, setSalvando] = useState(false)
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ignorar = false
    setOpcoes(null)
    setErroCarga(null)
    setErro(null)
    setErrosCampos({})

    Promise.all([
      plataformaApi.listar(),
      condicaoJogoApi.listar(),
      generoApi.listar(),
      id ? jogoApi.buscarPorId(id) : null,
    ]).then(([plataformas, condicoes, generos, jogo]) => {
      if (ignorar) return
      setDados(jogo ? {
        nome: jogo.nome,
        descricao: jogo.descricao ?? '',
        anoLancamento: String(jogo.anoLancamento),
        preco: String(jogo.preco),
        estoque: String(jogo.estoque),
        idPlataforma: String(jogo.idPlataforma),
        idCondicaoJogo: String(jogo.idCondicaoJogo),
        idsGeneros: jogo.generos.map((genero) => genero.id),
        ativo: jogo.ativo,
      } : formularioVazio)
      setOpcoes({ plataformas, condicoes, generos })
    }).catch((falha) => {
      if (!ignorar) setErroCarga(obterMensagemErro(falha))
    })

    return () => { ignorar = true }
  }, [id, tentativa])

  function atualizar(campo, valor) {
    setDados((atual) => ({ ...atual, [campo]: valor }))
  }

  function alternarGenero(idGenero, marcado) {
    setDados((atual) => ({
      ...atual,
      idsGeneros: marcado
        ? [...atual.idsGeneros, idGenero]
        : atual.idsGeneros.filter((idAtual) => idAtual !== idGenero),
    }))
  }

  async function salvar(evento) {
    evento.preventDefault()
    setSalvando(true)
    setErro(null)
    setErrosCampos({})

    const jogo = {
      nome: dados.nome,
      descricao: dados.descricao || null,
      anoLancamento: Number(dados.anoLancamento),
      preco: Number(dados.preco),
      estoque: Number(dados.estoque),
      idPlataforma: Number(dados.idPlataforma),
      idCondicaoJogo: Number(dados.idCondicaoJogo),
      idsGeneros: dados.idsGeneros,
    }

    try {
      if (id) await jogoApi.alterar(id, { ...jogo, ativo: dados.ativo })
      else await jogoApi.inserir(jogo)
      navegar('/admin/jogos', { replace: true })
    } catch (falha) {
      const mensagens = obterErrosDosCampos(falha)
      setErrosCampos(mensagens)
      if (Object.keys(mensagens).length === 0) setErro(obterMensagemErro(falha))
    } finally {
      setSalvando(false)
    }
  }

  function atributosCampo(campo, classe) {
    return {
      id: campo,
      className: errosCampos[campo] ? `${classe} is-invalid` : classe,
      value: dados[campo],
      onChange: (evento) => atualizar(campo, evento.target.value),
      'aria-invalid': Boolean(errosCampos[campo]),
      'aria-describedby': errosCampos[campo] ? `${campo}-erro` : undefined,
    }
  }

  function mensagemCampo(campo) {
    return errosCampos[campo] && (
      <div id={`${campo}-erro`} className="invalid-feedback d-block">{errosCampos[campo]}</div>
    )
  }

  return (
    <>
      <h1>{id ? 'Editar jogo' : 'Novo jogo'}</h1>
      {erroCarga && <MensagemErro mensagem={erroCarga} aoTentarNovamente={() => setTentativa((valor) => valor + 1)} />}
      {!opcoes && !erroCarga && <Carregando />}
      {erro && <MensagemErro mensagem={erro} />}
      {opcoes && (
        <form onSubmit={salvar} noValidate>
          <fieldset disabled={salvando}>
            {campos.map(([campo, rotulo, tipo]) => (
              <div className="mb-3" key={campo}>
                <label htmlFor={campo} className="form-label">{rotulo}</label>
                {tipo === 'textarea' ? (
                  <textarea {...atributosCampo(campo, 'form-control')} rows={3} />
                ) : (
                  <input {...atributosCampo(campo, 'form-control')} type={tipo} step={campo === 'preco' ? '0.01' : undefined} />
                )}
                {mensagemCampo(campo)}
              </div>
            ))}
            {[
              ['idPlataforma', 'Plataforma', opcoes.plataformas, 'nome'],
              ['idCondicaoJogo', 'Condição', opcoes.condicoes, 'descricao'],
            ].map(([campo, rotulo, itens, chaveRotulo]) => (
              <div className="mb-3" key={campo}>
                <label htmlFor={campo} className="form-label">{rotulo}</label>
                <select {...atributosCampo(campo, 'form-select')}>
                  <option value="">Selecione</option>
                  {itens.map((item) => <option key={item.id} value={item.id}>{item[chaveRotulo]}</option>)}
                </select>
                {mensagemCampo(campo)}
              </div>
            ))}
            <fieldset className="mb-3" aria-describedby={errosCampos.idsGeneros ? 'idsGeneros-erro' : undefined}>
              <legend className="fs-6">Gêneros</legend>
              {opcoes.generos.map((genero) => (
                <label className="form-check form-check-inline" key={genero.id}>
                  <input
                    className={`form-check-input${errosCampos.idsGeneros ? ' is-invalid' : ''}`}
                    type="checkbox"
                    checked={dados.idsGeneros.includes(genero.id)}
                    onChange={(evento) => alternarGenero(genero.id, evento.target.checked)}
                  />
                  <span className="form-check-label">{genero.nome}</span>
                </label>
              ))}
              {mensagemCampo('idsGeneros')}
            </fieldset>
            {id && (
              <label className="form-check mb-3">
                <input className="form-check-input" type="checkbox" checked={dados.ativo} onChange={(evento) => atualizar('ativo', evento.target.checked)} />
                <span className="form-check-label">Ativo</span>
              </label>
            )}
            <button className="btn btn-primary" type="submit">{salvando ? 'Salvando...' : 'Salvar'}</button>
          </fieldset>
        </form>
      )}
      <Link className="btn btn-outline-secondary mt-3" to="/admin/jogos">Voltar aos jogos</Link>
    </>
  )
}
