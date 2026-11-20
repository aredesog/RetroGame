import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { obterErrosDosCampos, obterMensagemErro } from '../../api/erros'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'
import useAutenticacao from '../../hooks/useAutenticacao'
import { obterDestinoInterno } from '../../lib/navegacao'

export default function Cadastro() {
  const { cadastrar, carregando } = useAutenticacao()
  const navegar = useNavigate()
  const [parametros] = useSearchParams()
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState(null)
  const [errosDosCampos, setErrosDosCampos] = useState({})

  async function enviar(evento) {
    evento.preventDefault()
    const campos = new FormData(evento.currentTarget)
    setEnviando(true)
    setErro(null)
    setErrosDosCampos({})

    try {
      await cadastrar({
        nome: campos.get('nome').trim(),
        email: campos.get('email').trim(),
        senha: campos.get('senha'),
      })
      navegar(obterDestinoInterno(parametros.get('voltarPara')), { replace: true })
    } catch (falha) {
      const erros = obterErrosDosCampos(falha)
      setErrosDosCampos(erros)
      if (Object.keys(erros).length === 0) setErro(obterMensagemErro(falha))
    } finally {
      setEnviando(false)
    }
  }

  if (carregando) return <Carregando texto="Verificando sessão..." />

  return (
    <div className="row justify-content-center">
      <div className="col-sm-8 col-md-6 col-lg-4">
        <h1 className="h3 mb-3">Criar conta</h1>
        {erro && <MensagemErro mensagem={erro} />}
        <form onSubmit={enviar} noValidate aria-busy={enviando}>
          <div className="mb-3">
            <label className="form-label" htmlFor="nome">Nome</label>
            <input
              className={`form-control${errosDosCampos.nome ? ' is-invalid' : ''}`}
              id="nome" name="nome" autoComplete="name" required
              aria-invalid={Boolean(errosDosCampos.nome)}
              aria-describedby={errosDosCampos.nome ? 'erro-nome' : undefined}
            />
            {errosDosCampos.nome && <div id="erro-nome" className="invalid-feedback">{errosDosCampos.nome}</div>}
          </div>
          <div className="mb-3">
            <label className="form-label" htmlFor="email">E-mail</label>
            <input
              className={`form-control${errosDosCampos.email ? ' is-invalid' : ''}`}
              id="email" name="email" type="email" autoComplete="email" required
              aria-invalid={Boolean(errosDosCampos.email)}
              aria-describedby={errosDosCampos.email ? 'erro-email' : undefined}
            />
            {errosDosCampos.email && <div id="erro-email" className="invalid-feedback">{errosDosCampos.email}</div>}
          </div>
          <div className="mb-3">
            <label className="form-label" htmlFor="senha">Senha</label>
            <input
              className={`form-control${errosDosCampos.senha ? ' is-invalid' : ''}`}
              id="senha" name="senha" type="password" autoComplete="new-password" required
              aria-invalid={Boolean(errosDosCampos.senha)}
              aria-describedby={errosDosCampos.senha ? 'regra-senha erro-senha' : 'regra-senha'}
            />
            {errosDosCampos.senha && <div id="erro-senha" className="invalid-feedback">{errosDosCampos.senha}</div>}
            <div id="regra-senha" className="form-text">Use pelo menos 6 caracteres.</div>
          </div>
          <button className="btn btn-primary" type="submit" disabled={enviando}>
            {enviando ? 'Cadastrando...' : 'Cadastrar'}
          </button>
        </form>
      </div>
    </div>
  )
}
