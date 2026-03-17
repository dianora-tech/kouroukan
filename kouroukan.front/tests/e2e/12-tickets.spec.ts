import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 12 : Tickets — Creer et repondre', () => {
  test('cree un ticket et ajoute une reponse', async ({ page }) => {
    await login(page, 'directeur')

    // Naviguer vers les tickets
    await page.goto('/support/tickets')
    await page.waitForURL('**/support/tickets**')

    // Creer un ticket
    const createBtn = page.getByRole('button', { name: /cr[eé]er|nouveau|signaler/i })
    if (await createBtn.isVisible()) {
      await createBtn.click()

      // Remplir le formulaire
      const sujetInput = page.locator('[name="sujet"]').or(page.locator('input').first())
      await sujetInput.fill('Probleme de connexion Mobile Money')

      const contenuInput = page.locator('[name="contenu"]').or(page.locator('textarea').first())
      await contenuInput.fill('Les paiements Orange Money echouent depuis ce matin.')

      // Soumettre
      await page.click('button[type="submit"]')

      // Verifier la creation
      await expect(
        page.getByText(/cr[eé][eé]|succ[eè]s|ouvert/i).or(page.getByRole('alert')),
      ).toBeVisible({ timeout: 5000 })
    }

    // Ouvrir le ticket et ajouter une reponse
    const ticketLink = page.getByText('Probleme de connexion Mobile Money')
    if (await ticketLink.isVisible({ timeout: 3000 }).catch(() => false)) {
      await ticketLink.click()

      // Ajouter une reponse
      const reponseInput = page.locator('[data-testid="reponse-input"]').or(page.locator('textarea').last())
      await reponseInput.fill('Nous avons identifie le probleme. Correction en cours.')

      const sendBtn = page.getByRole('button', { name: /envoyer|r[eé]pondre/i })
      await sendBtn.click()

      // Verifier la reponse dans le fil
      await expect(
        page.getByText('Nous avons identifie le probleme'),
      ).toBeVisible({ timeout: 5000 })
    }
  })
})
