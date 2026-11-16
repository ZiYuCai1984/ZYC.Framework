import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // UI/__PROJECT_SHORT_NAME__View.cs loads this address in Debug builds while `npm run dev` is running.
    port: 5173,
    strictPort: true,
  },
})
