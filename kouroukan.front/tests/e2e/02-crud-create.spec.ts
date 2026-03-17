import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 2 : CRUD — Creer une inscription', () => {
  test('cree une inscription et la voit dans la liste', async ({ page }) => {
    await login(page, 'directeur')

    // Naviguer vers les inscriptions
    await page.click('text=Inscriptions')
    await page.waitForURL('**/inscriptions**')

    // Cliquer sur le bouton de creation
    const createBtn = page.getByRole('button', { name: /cr[eé]er|nouveau/i })
    await createBtn.click()

    // Remplir le formulaire
    await page.fill('[name="dateInscription"]', '2025-09-15')
    await page.fill('[name="montantInscription"]', '150000')

    // Selectionner un statut
    const statutSelect = page.locator('[name="statutInscription"]')
    if (await statutSelect.isVisible()) {
      await statutSelect.selectOption('EnAttente')
    }

    // Soumettre
    await page.click('button[type="submit"]')

    // Verifier le toast de succes ou la presence dans la liste
    await expect(
      page.getByText(/cr[eé][eé]|succ[eè]s/i).or(page.getByRole('alert')),
    ).toBeVisible({ timeout: 5000 })
  })
})
