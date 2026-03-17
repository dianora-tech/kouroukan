import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 14 : Widget aide flottant', () => {
  test('cliquer sur le FAB affiche les 3 liens', async ({ page }) => {
    await login(page, 'directeur')

    // Trouver le bouton d'aide flottant (FAB)
    const fab = page.locator('button[aria-label]').filter({ hasText: /aide|help/i })
      .or(page.locator('.fixed.bottom-4.right-4 button'))
      .or(page.locator('[data-testid="floating-help-button"]'))

    await expect(fab.first()).toBeVisible()

    // Cliquer dessus
    await fab.first().click()

    // Verifier les 3 liens
    const panel = page.locator('.fixed.bottom-4.right-4 a')
      .or(page.locator('[data-testid="help-panel"] a'))

    await expect(panel).toHaveCount(3, { timeout: 3000 })

    // Verifier les URLs des liens
    const links = await panel.all()
    const hrefs = await Promise.all(links.map(l => l.getAttribute('href')))

    expect(hrefs).toContain('/support/aide-ia')
    expect(hrefs).toContain('/support/aide')
    expect(hrefs).toContain('/support/tickets')
  })
})
