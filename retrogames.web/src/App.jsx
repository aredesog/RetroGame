import { Route, Routes } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import Catalogo from './pages/Catalogo/Catalogo.jsx'
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx'
import NaoEncontrada from './pages/NaoEncontrada.jsx'
import Login from './pages/Login/Login.jsx'
import RotaProtegida from './components/RotaProtegida.jsx'

export default function App() {
  return (
    <Routes>
      {/* Todas as páginas aparecem dentro do Layout, no lugar do <Outlet /> */}
      <Route element={<Layout />}>
        <Route path="/" element={<Catalogo />} />
        <Route path="/login" element={<Login />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route path="/admin" element={<RotaProtegida somenteAdministrador />}>
          <Route path="jogos" element={<h1>Jogos</h1>} />
          <Route path="jogos/novo" element={<h1>Novo jogo</h1>} />
          <Route path="jogos/:id/editar" element={<h1>Editar jogo</h1>} />
          <Route path="plataformas" element={<h1>Plataformas</h1>} />
        </Route>
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}
