import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import { resolve } from 'path'

export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '~': resolve(__dirname, './app'),
      '#app': resolve(__dirname, './node_modules/nuxt/dist/app'),
      '#imports': resolve(__dirname, './.nuxt/imports.d.ts'),
    },
  },
  test: {
    globals: true,
    environment: 'happy-dom',
    include: ['**/*.{test,spec}.{ts,tsx}'],
    exclude: ['node_modules', '.nuxt', 'dist', 'tests/e2e/**'],
    setupFiles: ['./tests/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'text-summary', 'lcov', 'html'],
      include: [
        'app/core/stores/**',
        'app/core/api/**',
        'app/core/auth/**',
        'app/modules/**/stores/**',
        'app/modules/**/composables/**',
        'app/modules/**/components/**',
        'app/shared/components/**',
      ],
      exclude: ['**/*.d.ts', '**/*.types.ts'],
      thresholds: {
        lines: 80,
        functions: 80,
        branches: 75,
        statements: 80,
      },
    },
  },
})
