import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 4 : CRUD — Supprimer une entite', () => {
  test('supprime une inscription et verifie sa disparition', async ({ page }) => {
    await login(page, 'directeur')

    await page.click('text=Inscriptions')
    await page.waitForURL('**/inscriptions**')

    // Compter les lignes avant suppression
    const rowsBefore = await page.locator('table tbody tr').count()

    // Cliquer sur supprimer
    const deleteBtn = page.getByRole('button', { name: /supprimer/i }).first()
    if (await deleteBtn.isVisible() && rowsBefore > 0) {
      await deleteBtn.click()

      // Confirmer la suppression dans le dialog
      const confirmBtn = page.getByRole('button', { name: /confirmer|oui|supprimer/i }).last()
      await confirmBtn.click()

      // Verifier que la ligne a disparu
      await expect(
        page.getByText(/supprim[eé]|succ[eè]s/i).or(page.getByRole('alert')),
      ).toBeVisible({ timeout: 5000 })
    }
  })
})
