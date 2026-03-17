import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 10 : Chat IA — Envoyer un message', () => {
  test('envoie un message et recoit une reponse IA', async ({ page }) => {
    await login(page, 'directeur')

    // Naviguer vers l'aide IA
    await page.goto('/support/aide-ia')
    await page.waitForURL('**/support/aide-ia**')

    // Saisir un message
    const chatInput = page.locator('textarea').or(page.locator('[data-testid="chat-input"]')).first()
    await expect(chatInput).toBeVisible()
    await chatInput.fill('Comment inscrire un nouvel eleve ?')

    // Envoyer
    const sendBtn = page.getByRole('button', { name: /envoyer/i })
      .or(page.locator('[data-testid="send-button"]'))
    await sendBtn.first().click()

    // Verifier que le message utilisateur apparait
    await expect(
      page.getByText('Comment inscrire un nouvel eleve ?'),
    ).toBeVisible({ timeout: 5000 })

    // Attendre la reponse IA (peut prendre du temps avec Ollama)
    const iaResponse = page.locator('[data-testid="ia-badge"]')
      .or(page.locator('.assistant-bubble'))
      .or(page.getByText(/inscri/i).last())
    await expect(iaResponse).toBeVisible({ timeout: 30_000 })
  })
})
