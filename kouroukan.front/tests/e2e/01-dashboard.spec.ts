import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 1 : Login → Dashboard', () => {
  test('affiche le dashboard avec les widgets apres connexion', async ({ page }) => {
    await login(page, 'directeur')

    // Verifier que le dashboard est affiche
    await expect(page).toHaveURL(/dashboard/)

    // Verifier les widgets principaux
    await expect(page.locator('[data-testid="widget-inscriptions"]').or(page.getByText(/inscription/i)).first()).toBeVisible()
    await expect(page.locator('[data-testid="sidebar"]').or(page.getByRole('navigation')).first()).toBeVisible()
  })
})
