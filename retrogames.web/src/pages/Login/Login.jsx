import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { obterMensagemErro } from '../../api/erros'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'
import useAutenticacao from '../../hooks/useAutenticacao'

export default function Login() {
  const { entrar, carregando } = useAutenticacao()
  const navegar = useNavigate()
  const [parametros] = useSearchParams()
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [enviando, setEnviando] = useState(false)
  const [erro, setErro] = useState(null)

  async function enviar(evento) {
    evento.preventDefault()
    setEnviando(true)
    setErro(null)

    try {
      await entrar({ email: email.trim(), senha })
      const voltarPara = parametros.get('voltarPara')
      const destinoInterno = voltarPara?.startsWith('/') && !voltarPara.startsWith('//')
        && !voltarPara.includes('\\')
        && new URL(voltarPara, window.location.origin).origin === window.location.origin
      navegar(destinoInterno ? voltarPara : '/', { replace: true })
    } catch (falha) {
      setErro(obterMensagemErro(falha))
    } finally {
      setEnviando(false)
    }
  }

  if (carregando) return <Carregando texto="Verificando sessão..." />

  return (
    <div className="row justify-content-center">
      <div className="col-sm-8 col-md-6 col-lg-4">
        <h1 className="h3 mb-3">Entrar</h1>
        {erro && <MensagemErro mensagem={erro} />}
        <form onSubmit={enviar}>
          <div className="mb-3">
            <label className="form-label" htmlFor="email">E-mail</label>
            <input className="form-control" id="email" type="email" autoComplete="username" value={email} onChange={(evento) => setEmail(evento.target.value)} required />
          </div>
          <div className="mb-3">
            <label className="form-label" htmlFor="senha">Senha</label>
            <input className="form-control" id="senha" type="password" autoComplete="current-password" value={senha} onChange={(evento) => setSenha(evento.target.value)} required />
          </div>
          <button className="btn btn-primary" type="submit" disabled={enviando}>
            {enviando ? 'Entrando...' : 'Entrar'}
          </button>
        </form>
      </div>
    </div>
  )
}
