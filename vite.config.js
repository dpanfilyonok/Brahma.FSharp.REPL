import { defineConfig } from 'vite'

export default defineConfig({
    root: "src/Client.React",
    build: {
        outDir: "../public",
        emptyOutDir: true,
        sourcemap: true
    }
});
