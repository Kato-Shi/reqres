import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// Vite config with a proxy so the frontend can call the ASP.NET API without CORS headaches during local dev.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:7216',
        changeOrigin: true,
        secure: false // allow self-signed dev certificate from Visual Studio
      }
    }
  }
});
