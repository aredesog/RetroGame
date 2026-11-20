import api from './api'

// Uma função por endpoint. Cada uma devolve só o "data" do envelope
// { responseCode, data, errorMessage }; erro HTTP vira exceção (tratada com erros.js).

export const jogoApi = {
  // filtros: { busca, idPlataforma, idGenero, pagina, tamanhoPagina }. Os vazios não vão na URL.
  listar: async (filtros) => (await api.get('/Jogo', { params: filtros })).data.data,
  buscarPorId: async (id) => (await api.get(`/Jogo/${id}`)).data.data,
}

export const plataformaApi = {
  listar: async () => (await api.get('/Plataforma')).data.data,
}

export const generoApi = {
  listar: async () => (await api.get('/Genero')).data.data,
}

export const condicaoJogoApi = {
  listar: async () => (await api.get('/CondicaoJogo')).data.data,
}

export const autenticacaoApi = {
  entrar: async (dados) => (await api.post('/Autenticacao/login', dados)).data.data,
  sair: async () => (await api.post('/Autenticacao/logout')).data.data,
  usuarioAtual: async () => (await api.get('/Autenticacao/eu')).data.data,
}
