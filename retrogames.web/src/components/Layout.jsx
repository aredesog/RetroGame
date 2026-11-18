import { Link, NavLink, Outlet } from 'react-router-dom'

export default function Layout() {
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
            </ul>
          </div>
        </div>
      </nav>

      <main className="container py-4 flex-grow-1">
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
