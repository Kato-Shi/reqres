import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Vite config with a proxy so the frontend can call the ASP.NET API without CORS headaches during local dev.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5135',
        changeOrigin: true
      }
    }
  }
});
