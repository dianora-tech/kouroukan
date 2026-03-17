import { test, expect } from '@playwright/test'

test.describe('Scenario 5 : Acces sans auth → redirection', () => {
  test('redirige vers /connexion sans authentification', async ({ page }) => {
    // Acceder directement a une page protegee
    await page.goto('/inscriptions')

    // Doit etre redirige vers /connexion
    await expect(page).toHaveURL(/connexion/)
  })

  test('redirige vers /connexion pour /dashboard', async ({ page }) => {
    await page.goto('/dashboard')
    await expect(page).toHaveURL(/connexion/)
  })

  test('redirige vers /connexion pour /finances', async ({ page }) => {
    await page.goto('/finances')
    await expect(page).toHaveURL(/connexion/)
  })
})
