// @ts-check
import { createConfigForNuxt } from '@nuxt/eslint-config/flat'

export default createConfigForNuxt({
  features: {
    tooling: true,
    stylistic: true,
  },
  dirs: {
    src: ['./app'],
  },
}).append(
  // Ignore test and config files from strict rules
  {
    ignores: ['tests/**', 'vitest.config.ts'],
  },
  // Downgrade pre-existing stylistic/strict violations to warnings
  {
    rules: {
      // Vue template false-positives for variables used only in templates
      '@typescript-eslint/no-unused-vars': 'warn',
      // Stylistic rules — warn instead of error for pre-existing code
      '@stylistic/quote-props': 'warn',
      'vue/singleline-html-element-content-newline': 'warn',
      // TypeScript strict rules with many pre-existing violations
      '@typescript-eslint/unified-signatures': 'warn',
      '@typescript-eslint/no-invalid-void-type': 'warn',
      '@typescript-eslint/no-explicit-any': 'warn',
      // Import ordering
      'import/order': 'warn',
      'import/first': 'warn',
      'import/no-duplicates': 'warn',
      // Stylistic
      '@stylistic/member-delimiter-style': 'warn',
      '@stylistic/brace-style': 'warn',
      // Nuxt config order
      'nuxt/nuxt-config-keys-order': 'warn',
      // Unicorn
      'unicorn/prefer-node-protocol': 'warn',
    },
  },
)
