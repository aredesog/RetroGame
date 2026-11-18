import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    strictPort: true, // se a 5173 estiver ocupada, avisa em vez de trocar de porta sozinho
    // Tudo que começa com /api vai para a API. Para o navegador, front-end e API
    // ficam na mesma origem: não precisa de CORS e o cookie SameSite=Strict funciona.
    proxy: {
      '/api': 'http://localhost:5080',
    },
  },
})
