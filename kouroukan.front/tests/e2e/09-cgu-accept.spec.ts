import { test, expect } from '@playwright/test'
import { loginWithoutCgu } from './helpers/auth'

test.describe('Scenario 9 : Page CGU → scroll → accepter → redirection', () => {
  test('accepte les CGU apres avoir scrolle et coche', async ({ page }) => {
    await loginWithoutCgu(page)
    await page.waitForURL(/support\/cgu/, { timeout: 10_000 })

    // Verifier que le contenu CGU est affiche
    const cguContent = page.locator('[data-testid="cgu-content"]')
      .or(page.getByText(/conditions/i))
    await expect(cguContent.first()).toBeVisible()

    // Scroller jusqu'en bas du conteneur CGU
    const scrollContainer = page.locator('[data-testid="cgu-scroll"]').or(page.locator('.overflow-y-auto')).first()
    if (await scrollContainer.isVisible()) {
      await scrollContainer.evaluate(el => el.scrollTop = el.scrollHeight)
    }

    // Cocher la checkbox
    const checkbox = page.locator('input[type="checkbox"]').first()
    await checkbox.check()

    // Cliquer sur Accepter
    const acceptBtn = page.getByRole('button', { name: /accepter/i })
    await expect(acceptBtn).toBeEnabled()
    await acceptBtn.click()

    // Doit etre redirige vers le dashboard
    await expect(page).toHaveURL(/dashboard/, { timeout: 10_000 })
  })
})
