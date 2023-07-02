import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { viteSingleFile } from 'vite-plugin-singlefile';

// https://vitejs.dev/config/
export default defineConfig({
  build: {
    outDir: '../EchoV3/Resources/Ui',
    rollupOptions: {
      output: {
        entryFileNames: '[name].js',
      },
    },
  },
  plugins: [react(), viteSingleFile()],
  test: {
    environment: 'jsdom',
    setupFiles: './tests/setup.ts',
    globals: true,
  },
});
