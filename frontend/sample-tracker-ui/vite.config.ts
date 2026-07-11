import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  build: {
    outDir: '../../backend/SampleTracker.API/wwwroot',
    emptyOutDir: true,
  },
  server: {
    // Moved off Vite's default 5173 to avoid colliding with DungeonMaster and
    // Cognima Avatar Studio (both also defaulted to 5173). See CowLauncher ports registry.
    port: 5178,
    strictPort: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5294',
        changeOrigin: true,
      }
    }
  }
})
