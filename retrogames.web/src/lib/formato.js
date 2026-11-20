const moedaBrasileira = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' })
const dataHoraBrasileira = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' })

// 199.9 -> "R$ 199,90"
export function formatarPreco(valor) {
  return moedaBrasileira.format(valor)
}

export function formatarDataHora(iso) {
  return dataHoraBrasileira.format(new Date(iso))
}
