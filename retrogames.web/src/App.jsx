import { Route, Routes } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import Catalogo from './pages/Catalogo/Catalogo.jsx'
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx'
import NaoEncontrada from './pages/NaoEncontrada.jsx'
import Login from './pages/Login/Login.jsx'
import Cadastro from './pages/Cadastro/Cadastro.jsx'
import Carrinho from './pages/Carrinho/Carrinho.jsx'
import MeusPedidos from './pages/MeusPedidos/MeusPedidos.jsx'
import RotaProtegida from './components/RotaProtegida.jsx'
import Plataforma from './pages/Auxiliares/Plataforma.jsx'
import Jogos from './pages/Administracao/Jogos.jsx'
import FormularioJogo from './pages/Administracao/FormularioJogo.jsx'

export default function App() {
  return (
    <Routes>
      {/* Todas as páginas aparecem dentro do Layout, no lugar do <Outlet /> */}
      <Route element={<Layout />}>
        <Route path="/" element={<Catalogo />} />
        <Route path="/login" element={<Login />} />
        <Route path="/cadastro" element={<Cadastro />} />
        <Route path="/jogos/:id" element={<DetalheJogo />} />
        <Route element={<RotaProtegida />}>
          <Route path="/carrinho" element={<Carrinho />} />
          <Route path="/meus-pedidos" element={<MeusPedidos />} />
        </Route>
        <Route path="/admin" element={<RotaProtegida somenteAdministrador />}>
          <Route path="jogos" element={<Jogos />} />
          <Route path="jogos/novo" element={<FormularioJogo key="novo" />} />
          <Route path="jogos/:id/editar" element={<FormularioJogo key="editar" />} />
          <Route path="plataformas" element={<Plataforma />} />
        </Route>
        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}
