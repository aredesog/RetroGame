import { useState } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { obterMensagemErro } from '../api/erros'
import useAutenticacao from '../hooks/useAutenticacao'
import { MensagemErro } from './MensagensEstado'

export default function Layout() {
  const { usuario, carregando, estaAutenticado, ehAdministrador, sair } = useAutenticacao()
  const navegar = useNavigate()
  const [saindo, setSaindo] = useState(false)
  const [erro, setErro] = useState(null)

  async function encerrarSessao() {
    setSaindo(true)
    setErro(null)
    try {
      await sair()
      navegar('/')
    } catch (falha) {
      if (falha.response?.status === 401) navegar('/')
      else setErro(obterMensagemErro(falha))
    } finally {
      setSaindo(false)
    }
  }

  return (
    <>
      <nav className="navbar navbar-expand-md navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand fw-bold" to="/">
            RetroGames
          </Link>

          {/* Em tela estreita, os links somem e aparece este botão */}
          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuPrincipal"
            aria-controls="menuPrincipal"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div className="collapse navbar-collapse" id="menuPrincipal">
            <ul className="navbar-nav me-auto">
              <li className="nav-item">
                {/* NavLink troca de página sem recarregar e marca o link ativo; "end" evita marcar em /jogos/... */}
                <NavLink className="nav-link" to="/" end>
                  Catálogo
                </NavLink>
              </li>
              {ehAdministrador && (
                <li className="nav-item dropdown">
                  <button className="nav-link dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                    Administração
                  </button>
                  <ul className="dropdown-menu">
                    <li><NavLink className="dropdown-item" to="/admin/jogos">Jogos</NavLink></li>
                    <li><NavLink className="dropdown-item" to="/admin/plataformas">Plataformas</NavLink></li>
                  </ul>
                </li>
              )}
            </ul>
            {!carregando && (
              estaAutenticado ? (
                <div className="d-flex align-items-center gap-3">
                  <span className="navbar-text">Olá, {usuario.nome}</span>
                  <button className="btn btn-outline-light btn-sm" type="button" onClick={encerrarSessao} disabled={saindo}>
                    {saindo ? 'Saindo...' : 'Sair'}
                  </button>
                </div>
              ) : (
                <NavLink className="nav-link text-light" to="/login">Entrar</NavLink>
              )
            )}
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
        {erro && estaAutenticado && <MensagemErro mensagem={erro} />}
        <Outlet />
      </main>

      <footer className="bg-light border-top py-3">
        <div className="container text-center text-muted small">
          RetroGames · Loja de jogos retrô usados
        </div>
      </footer>
    </>
  )
}
