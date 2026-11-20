import { useContext } from 'react'
import { AutenticacaoContext } from '../contexts/autenticacaoContext'

export default function useAutenticacao() {
  return useContext(AutenticacaoContext)
}
