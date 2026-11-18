import axios from 'axios'

// Único arquivo que importa o Axios. Os componentes usam as funções do servicos.js.
const api = axios.create({
  baseURL: '/api',        // o proxy do Vite repassa para http://localhost:5080
  withCredentials: true,  // envia o cookie de sessão (retrogames.sessao) junto
})

export default api
