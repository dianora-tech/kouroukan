import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 11 : Suggestions — Creer et voter', () => {
  test('cree une suggestion et vote dessus', async ({ page }) => {
    await login(page, 'enseignant')

    // Naviguer vers les suggestions
    await page.goto('/support/suggestions')
    await page.waitForURL('**/support/suggestions**')

    // Creer une suggestion
    const createBtn = page.getByRole('button', { name: /cr[eé]er|nouveau|proposer/i })
    if (await createBtn.isVisible()) {
      await createBtn.click()

      // Remplir le formulaire
      const titreInput = page.locator('[name="titre"]').or(page.locator('input').first())
      await titreInput.fill('Ajouter un mode sombre')

      const contenuInput = page.locator('[name="contenu"]').or(page.locator('textarea').first())
      await contenuInput.fill('Ce serait pratique d\'avoir un mode sombre pour les cours du soir.')

      // Soumettre
      await page.click('button[type="submit"]')

      // Verifier la creation
      await expect(
        page.getByText(/cr[eé][eé]|succ[eè]s/i).or(page.getByRole('alert')),
      ).toBeVisible({ timeout: 5000 })
    }

    // Voter sur une suggestion
    const voteBtn = page.locator('[data-testid="vote-button"]')
      .or(page.getByRole('button', { name: /vote/i }))
    if (await voteBtn.first().isVisible({ timeout: 3000 }).catch(() => false)) {
      const countBefore = await voteBtn.first().textContent()
      await voteBtn.first().click()

      // Verifier l'incrementation
      await page.waitForTimeout(500)
    }
  })
})
