import { Navigate, Outlet, useLocation } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao'
import { Carregando } from './MensagensEstado'

export default function RotaProtegida({ somenteAdministrador = false }) {
  const { carregando, estaAutenticado, ehAdministrador } = useAutenticacao()
  const { pathname, search, hash } = useLocation()

  if (carregando) return <Carregando texto="Verificando sessão..." />

  if (!estaAutenticado) {
    const parametros = new URLSearchParams({ voltarPara: pathname + search + hash })
    return <Navigate to={`/login?${parametros}`} replace />
  }

  if (somenteAdministrador && !ehAdministrador) return <Navigate to="/" replace />

  return <Outlet />
}
