import axios from 'axios'

// Único arquivo que importa o Axios. Os componentes usam as funções do servicos.js.
const api = axios.create({
  baseURL: '/api',        // o proxy do Vite repassa para http://localhost:5080
  withCredentials: true,  // envia o cookie de sessão (retrogames.sessao) junto
})

api.interceptors.response.use(
  (resposta) => resposta,
  (erro) => {
    const caminho = erro.config?.url?.split('?')[0]
    const respostaEsperada = caminho === '/Autenticacao/eu' || caminho === '/Autenticacao/login'

    if (erro.response?.status === 401 && !respostaEsperada) {
      window.dispatchEvent(new Event('sessao:expirada'))
    }

    return Promise.reject(erro)
  },
)

export default api
