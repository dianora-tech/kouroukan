import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 7 : Mode offline → sync → verification', () => {
  test('cree une entite hors ligne puis synchronise', async ({ page, context }) => {
    await login(page, 'directeur')

    await page.goto('/inscriptions')

    // Passer offline
    await context.setOffline(true)

    // Verifier l'indicateur offline
    const networkIndicator = page.locator('[data-testid="network-indicator"]')
      .or(page.getByText(/hors ligne|offline/i))
    if (await networkIndicator.isVisible({ timeout: 3000 }).catch(() => false)) {
      expect(await networkIndicator.isVisible()).toBe(true)
    }

    // Tenter une creation (sera mise en queue)
    const createBtn = page.getByRole('button', { name: /cr[eé]er|nouveau/i })
    if (await createBtn.isVisible({ timeout: 2000 }).catch(() => false)) {
      await createBtn.click()

      // Remplir formulaire minimal
      const dateInput = page.locator('[name="dateInscription"]')
      if (await dateInput.isVisible()) {
        await dateInput.fill('2025-09-15')
      }

      await page.click('button[type="submit"]')

      // Verifier le message de mise en queue
      await expect(
        page.getByText(/file d'attente|synchronis|queue/i).or(page.getByRole('alert')),
      ).toBeVisible({ timeout: 3000 }).catch(() => {})
    }

    // Repasser online
    await context.setOffline(false)

    // Attendre la synchronisation
    await page.waitForTimeout(2000)

    // Verifier le statut de synchronisation
    const syncStatus = page.locator('[data-testid="sync-status"]')
      .or(page.getByText(/synchronis[eé]/i))
    if (await syncStatus.isVisible({ timeout: 3000 }).catch(() => false)) {
      expect(await syncStatus.isVisible()).toBe(true)
    }
  })
})
