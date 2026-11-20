import { useContext } from 'react'
import { CarrinhoContext } from '../contexts/carrinhoContext'

export default function useCarrinho() {
  return useContext(CarrinhoContext)
}
