import { defineConfig } from 'vite'

export default defineConfig({
    root: "src/Client",
    build: {
        outDir: "../public",
        emptyOutDir: true,
        sourcemap: true
    }
});
