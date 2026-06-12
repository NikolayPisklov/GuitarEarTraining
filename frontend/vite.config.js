import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, '..', '')

  return {
    envDir: '..',
    plugins: [vue()],
    define: {
      __BACKEND_API_URL__: JSON.stringify(env.BACKEND_API_URL ?? ''),
    },
  }
})
