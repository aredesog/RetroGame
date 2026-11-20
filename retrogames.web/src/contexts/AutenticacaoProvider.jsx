import { useEffect, useState } from 'react'
import { autenticacaoApi } from '../api/servicos'
import { AutenticacaoContext } from './autenticacaoContext'

export default function AutenticacaoProvider({ children }) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)

  useEffect(() => {
    let ignorar = false

    function limparSessao() {
      // Uma resposta antiga do eu não deve restaurar uma sessão já expirada.
      ignorar = true
      setUsuario(null)
      setCarregando(false)
    }

    window.addEventListener('sessao:expirada', limparSessao)

    autenticacaoApi.usuarioAtual()
      .then((dados) => {
        if (!ignorar) setUsuario(dados)
      })
      .catch(() => {
        if (!ignorar) setUsuario(null)
      })
      .finally(() => {
        if (!ignorar) setCarregando(false)
      })

    return () => {
      ignorar = true
      window.removeEventListener('sessao:expirada', limparSessao)
    }
  }, [])

  async function entrar(dados) {
    const usuarioLogado = await autenticacaoApi.entrar(dados)
    setUsuario(usuarioLogado)
  }

  async function cadastrar(dados) {
    const usuarioCadastrado = await autenticacaoApi.cadastrar(dados)
    setUsuario(usuarioCadastrado)
  }

  async function sair() {
    await autenticacaoApi.sair()
    setUsuario(null)
  }

  return (
    <AutenticacaoContext.Provider value={{
      usuario,
      carregando,
      estaAutenticado: usuario !== null,
      ehAdministrador: usuario?.perfil === 'Administrador',
      entrar,
      cadastrar,
      sair,
    }}>
      {children}
    </AutenticacaoContext.Provider>
  )
}
