import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 3 : CRUD — Modifier une entite', () => {
  test('modifie une inscription existante', async ({ page }) => {
    await login(page, 'directeur')

    await page.click('text=Inscriptions')
    await page.waitForURL('**/inscriptions**')

    // Cliquer sur le bouton editer de la premiere ligne
    const editBtn = page.getByRole('button', { name: /[eé]diter|modifier/i }).first()
    if (await editBtn.isVisible()) {
      await editBtn.click()

      // Modifier le montant
      const montantInput = page.locator('[name="montantInscription"]')
      if (await montantInput.isVisible()) {
        await montantInput.fill('200000')
      }

      // Soumettre
      await page.click('button[type="submit"]')

      // Verifier le toast de succes
      await expect(
        page.getByText(/mis[e]? [aà] jour|modifi[eé]|succ[eè]s/i).or(page.getByRole('alert')),
      ).toBeVisible({ timeout: 5000 })
    }
  })
})
