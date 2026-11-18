// Anterior, números e Próxima. Com uma página só (ou nenhuma), não mostra nada.
export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) {
    return null
  }

  const numeros = Array.from({ length: totalPaginas }, (_, i) => i + 1)

  return (
    <nav aria-label="Páginas do catálogo">
      <ul className="pagination justify-content-center">
        <li className={`page-item ${pagina === 1 ? 'disabled' : ''}`}>
          <button type="button" className="page-link" onClick={() => aoMudar(pagina - 1)}>
            Anterior
          </button>
        </li>

        {numeros.map((numero) => (
          <li key={numero} className={`page-item ${numero === pagina ? 'active' : ''}`}>
            <button
              type="button"
              className="page-link"
              onClick={() => aoMudar(numero)}
              aria-current={numero === pagina ? 'page' : undefined}
            >
              {numero}
            </button>
          </li>
        ))}

        <li className={`page-item ${pagina === totalPaginas ? 'disabled' : ''}`}>
          <button type="button" className="page-link" onClick={() => aoMudar(pagina + 1)}>
            Próxima
          </button>
        </li>
      </ul>
    </nav>
  )
}
