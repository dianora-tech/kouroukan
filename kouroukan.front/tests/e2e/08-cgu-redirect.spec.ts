import { test, expect } from '@playwright/test'
import { loginWithoutCgu } from './helpers/auth'

test.describe('Scenario 8 : CGU non acceptees → redirection', () => {
  test('redirige vers /support/cgu si CGU non acceptees apres login', async ({ page }) => {
    await loginWithoutCgu(page)

    // Doit etre redirige vers la page CGU
    await expect(page).toHaveURL(/support\/cgu/, { timeout: 10_000 })
  })
})
