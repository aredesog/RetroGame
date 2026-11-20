// Transforma qualquer falha de chamada à API numa mensagem para mostrar na tela.
export function obterMensagemErro(erro) {
  // Sem resposta: a API está fora do ar ou a rede caiu.
  if (!erro?.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  // Com resposta: a API já manda a mensagem pronta no envelope.
  return erro.response.data?.errorMessage ?? 'Ocorreu um erro inesperado. Tente novamente mais tarde.'
}
export function obterErrosDosCampos(erro) {
  const resposta = erro?.response
  const envelope = resposta?.data
  if (resposta?.status !== 400 || envelope?.responseCode !== 3 || !envelope.data) return {}

  // A API agrupa as mensagens de cada campo em uma lista.
  return Object.fromEntries(
    Object.entries(envelope.data).map(([campo, mensagens]) => [campo, mensagens.join(' ')]),
  )
}
