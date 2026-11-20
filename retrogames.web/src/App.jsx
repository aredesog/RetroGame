import { Route, Routes } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import Catalogo from './pages/Catalogo/Catalogo.jsx'
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx'
import NaoEncontrada from './pages/NaoEncontrada.jsx'
import Login from './pages/Login/Login.jsx'

export default function App() {
  return (
    <Routes>
      {/* Todas as páginas aparecem dentro do Layout, no lugar do <Outlet /> */}
      <Route element={<Layout />}>
        <Route path="/" element={<Catalogo />} />
        <Route path="/login" element={<Login />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}
