import { useEffect, useState } from 'react'
import { CarrinhoContext } from './carrinhoContext'

const chaveCarrinho = 'retrogames.carrinho'

function carregarItens() {
  try {
    const itens = JSON.parse(localStorage.getItem(chaveCarrinho))
    if (!Array.isArray(itens)) return []

    return itens.filter((item) =>
      Number.isInteger(item?.idJogo) && item.idJogo > 0
      && typeof item.nome === 'string' && typeof item.nomePlataforma === 'string'
      && Number.isFinite(item.preco) && item.preco >= 0
      && Number.isInteger(item.estoque)
      && Number.isInteger(item.quantidade) && item.quantidade >= 1
      && item.quantidade <= Math.min(10, item.estoque),
    )
  } catch {
    return []
  }
}

export default function CarrinhoProvider({ children }) {
  const [itens, setItens] = useState(carregarItens)

  useEffect(() => {
    try {
      localStorage.setItem(chaveCarrinho, JSON.stringify(itens))
    } catch {
      // Com o armazenamento indisponível, o carrinho funciona em memória.
    }
  }, [itens])

  function adicionar(jogo) {
    if (jogo.estoque <= 0 || !jogo.ativo) return

    const { id: idJogo, nome, nomePlataforma, preco, estoque } = jogo
    setItens((atuais) => {
      const existente = atuais.find((item) => item.idJogo === idJogo)
      const quantidade = Math.min(10, estoque, (existente?.quantidade ?? 0) + 1)
      const atualizado = { idJogo, nome, nomePlataforma, preco, estoque, quantidade }

      return existente
        ? atuais.map((item) => item.idJogo === idJogo ? atualizado : item)
        : [...atuais, atualizado]
    })
  }

  function alterarQuantidade(idJogo, quantidade) {
    if (!Number.isInteger(quantidade)) return

    setItens((atuais) => atuais.map((item) => {
      if (item.idJogo !== idJogo) return item
      return { ...item, quantidade: Math.max(1, Math.min(10, item.estoque, quantidade)) }
    }))
  }

  function remover(idJogo) {
    setItens((atuais) => atuais.filter((item) => item.idJogo !== idJogo))
  }

  function limpar() {
    setItens([])
  }

  const quantidadeTotal = itens.reduce((soma, item) => soma + item.quantidade, 0)
  const total = itens.reduce((soma, item) => soma + item.preco * item.quantidade, 0)

  return (
    <CarrinhoContext.Provider value={{ itens, quantidadeTotal, total, adicionar, alterarQuantidade, remover, limpar }}>
      {children}
    </CarrinhoContext.Provider>
  )
}
