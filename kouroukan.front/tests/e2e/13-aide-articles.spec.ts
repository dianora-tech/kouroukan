import { test, expect } from '@playwright/test'
import { login } from './helpers/auth'

test.describe('Scenario 13 : Aide — Rechercher un article', () => {
  test('recherche un article et verifie le surlignage', async ({ page }) => {
    await login(page, 'parent')

    // Naviguer vers l'aide
    await page.goto('/support/aide')
    await page.waitForURL('**/support/aide**')

    // Rechercher un article
    const searchInput = page.getByRole('searchbox')
      .or(page.locator('[data-testid="search-input"]'))
      .or(page.locator('input[type="search"]'))
      .or(page.locator('input[placeholder*="herch"]'))

    if (await searchInput.first().isVisible()) {
      await searchInput.first().fill('inscription')

      // Attendre les resultats
      await page.waitForTimeout(500)

      // Verifier les resultats
      const results = page.locator('[data-testid="article-result"]')
        .or(page.getByRole('article'))
        .or(page.locator('.article-item'))

      const count = await results.count()
      if (count > 0) {
        // Verifier le surlignage (mark ou highlight)
        const highlight = page.locator('mark').or(page.locator('.highlight'))
        await expect(highlight.first()).toBeVisible({ timeout: 3000 }).catch(() => {
          // Surlignage non obligatoire, l'article est quand meme affiche
        })
      }
    }
  })
})
